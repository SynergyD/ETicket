﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ETicket.Domain.Entities
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }

        //TODO 
        [Required]
        [ForeignKey("DocumentTypeId")]
        public int DocumentTypeId { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        [MaxLength(50)]
        public string Number { get; set; }

        [Required]
        public DateTime? ExpirationDate { get; set; }

        [Required]
        public bool IsValid { get; set; }
    }
}
