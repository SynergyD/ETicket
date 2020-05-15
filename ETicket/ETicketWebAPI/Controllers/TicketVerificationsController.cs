﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

using ETicket.ApplicationServices.Services.Interfaces;
using ETicket.DataAccess.Domain.Entities;
using ETicket.WebAPI.Models.TicketVerification;

namespace ETicket.WebAPI.Controllers
{
    [Route("api/ticketverifications")]
    [ApiController]
    public class TicketVerificationsController : ControllerBase
    {
        private readonly ITicketVerificationService verificationService;

        public TicketVerificationsController(ITicketVerificationService ticketVerificationService)
        {
            this.verificationService = ticketVerificationService;
        }

        [HttpGet("ticket/{ticketId}")]
        public IActionResult GetTicketVerificationHistory(Guid ticketId)
        {
            var ticketVerification = verificationService
                     .GetTicketVerifications()
                     .Where(t => t.TicketId == ticketId)
                     .OrderByDescending(t => t.VerificationUTCDate);

            return Ok(ticketVerification);
        }

        [HttpPost]
        public IActionResult VerifyTicket([FromBody]VerifyTicketRequest request)
        {
            return Ok(verificationService.VerifyTicket(
                request.TicketId, request.TransportId, request.Longitude, request.Latitude));
        }
    }
}
