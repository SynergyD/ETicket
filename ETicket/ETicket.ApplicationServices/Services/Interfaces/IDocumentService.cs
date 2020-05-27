﻿using System;
using System.Collections.Generic;
using ETicket.Admin.Models.DataTables;
using ETicket.ApplicationServices.DTOs;
using ETicket.ApplicationServices.Services.PagingServices.Models;
using ETicket.DataAccess.Domain.Entities;

namespace ETicket.ApplicationServices.Services.Interfaces
{
    public interface IDocumentService
    {

        public IEnumerable<DocumentDto> GetDocuments();
        public Document GetDocumentById(Guid Id);
        public void Create(DocumentDto documentTypeDto);
        public void Update(DocumentDto documentTypeDto);
        public void Delete(Guid id);
        public bool Exists(Guid id);
        public DataTablePage<DocumentDto> GetDocumentPage(DataTablePagingInfo pagingInfo);
    }
}