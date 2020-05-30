﻿using System.ComponentModel;

namespace ETicket.ApplicationServices.DTOs
{
    public class RouteDto
    {
        public int Id { get; set; }

        [DisplayName("Route number")]
        public string Number { get; set; }

        [DisplayName("First station")]
        public int FirstStationId { get; set; }

        [DisplayName("Last station")]
        public int LastStationId { get; set; }
    }
}
