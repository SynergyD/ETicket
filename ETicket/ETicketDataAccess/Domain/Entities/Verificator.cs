using System;
using System.ComponentModel.DataAnnotations;

namespace ETicket.DataAccess.Domain.Entities
{
    public class Verificator
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Number { get; set; }
    }
}