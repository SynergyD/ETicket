﻿using ETicket.DataAccess.Domain.Entities;
using ETicket.DataAccess.Domain.Interfaces;
using ETicket.WebAPI.Models.TicketVerification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETicket.WebAPI.Services.TicketVerifyService
{
    public class TicketVerifyService : ITicketVerifyService
    {
        #region private members 

        private readonly IUnitOfWork unitOfWork;
        private static string ticketNotFoundError = "Ticket was not found";
        private static string transportNotFoundError = "Transport was not found";
        private static string stationNotFoundError = "Station was not found";
        private static string ticketNotActivatedError = "Ticket not activated";
        private static string unknownError = "Something went wrong";

        #endregion


        public TicketVerifyService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;           
        }

        //TODO Separate into two methods Verify and Activate 
        public VerifyTicketResponse VerifyTicket(VerifyTicketRequest request)
        {
            var ticket = GetTicket(request.TicketId);

            if (ticket == null)
            {
                return new VerifyTicketResponse { ErrorMessage = ticketNotFoundError };
            }
            if (ticket.ActivatedUTCDate == null)
            {
                return new VerifyTicketResponse { ErrorMessage = ticketNotActivatedError };
            }

            var transport = GetTransport(request.TransportId);

            if (transport == null)
            {
                return new VerifyTicketResponse { ErrorMessage = transportNotFoundError };
            }

            var station = GetNearestStationOnRoute(
                    transport.RouteId, request.Latitude, request.Longitude);

            if (station == null)
            {
                return new VerifyTicketResponse { ErrorMessage = stationNotFoundError };
            }

            //Compare ticket area with train station
            //Check for expired ticket
            if (ticket.TicketArea.Any(t => t.AreaId == station.Area.Id)
                && ticket.ExpirationUTCDate > DateTime.UtcNow)
            {
                return new VerifyTicketResponse { IsValid = true };
            }

            return new VerifyTicketResponse { ErrorMessage = unknownError};
        }

        private void SaveTicketVerification(
            Guid ticketId,
            long transportId,
            int stationId,
            bool isVerified
        )
        {
            var ticketVerification = new TicketVerification()
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                TransportId = transportId,
                StationId = stationId,
                IsVerified = isVerified
            };

            unitOfWork.TicketVerifications.Create(ticketVerification);
        }

        private Ticket GetTicket(Guid ticketId)
        {
            return unitOfWork
                    .Tickets
                    .GetAll()
                    .Include(t => t.TicketArea)
                    .FirstOrDefault(t => t.Id == ticketId);
        }

        private Transport GetTransport(long transportId)
        {
            return unitOfWork.Transports.Get(transportId);
        }

        private Station GetNearestStationOnRoute(
            int routeId,
            float latitude,
            float longitude
        )
        {
            return unitOfWork
                    .RouteStation
                    .GetAll()
                    .Where(t => t.Route.Id == routeId)
                    .Include(t => t.Station.Area)
                    .Select(t => t.Station)
                    .OrderBy(t => Math.Sqrt(Math.Pow(t.Latitude * latitude, 2) + Math.Pow(t.Longitude * longitude, 2)))
                    .FirstOrDefault();
        }
    }
}
