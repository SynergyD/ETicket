﻿using Newtonsoft.Json;

namespace ETicketMobile.WebAccess.DTO
{
    public class UserSignUpResponseDto
    {
        [JsonProperty("succeeded")]
        public bool Succeeded { get; set; }
    }
}