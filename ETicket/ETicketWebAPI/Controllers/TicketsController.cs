﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using log4net;
using ETicket.ApplicationServices.Services.Interfaces;
using ETicket.WebAPI.Models.TicketVerification;

namespace ETicket.WebAPI.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController : BaseAPIController
    {
        #region Private members

        private readonly ITicketService ticketService;
        private readonly ITicketVerificationService verificationService;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion
        public TicketsController(ITicketService ticketService, ITicketVerificationService verificationService)
        {
            this.ticketService = ticketService;
            this.verificationService = verificationService;
        }

        // GET: api/tickets/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTicket(Guid id)
        {
            log.Info(nameof(GetTicket));

            try
            {
                var ticketDto = ticketService.GetTicketById(id);

                if (ticketDto == null)
                {
                    log.Warn(nameof(GetTicket) + " ticketDto is null");

                    return NotFound();
                }

                return Json(ticketDto);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpGet("{ticketId}/verification-history")]
        public IActionResult GetTicketVerificationHistory(Guid ticketId)
        {
            log.Info(nameof(GetTicketVerificationHistory));

            try
            {
                var ticketVerification = verificationService
                     .GetVerificationHistoryByTicketId(ticketId);            

                return Ok(ticketVerification);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        // PUT: api/tickets/activate
        [HttpPut("/activate/{ticketId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Activate(Guid ticketId)
        {
            log.Info(nameof(Activate));

            try
            {
                ticketService.Activate(ticketId);

                return NoContent();
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpPost("{ticketId}/verify")]
        public IActionResult VerifyTicket(Guid ticketId, [FromBody]VerifyTicketRequest request)
        {
            log.Info(nameof(VerifyTicket));

            try
            {
                return Ok(verificationService.VerifyTicket(
                    ticketId, request.TransportId, request.Longitude, request.Latitude));
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }
    }
}

