﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ETicket.ApplicationServices.DTOs;
using ETicket.ApplicationServices.Services.Interfaces;
using ETicket.DataAccess.Domain.Entities;
using ETicket.DataAccess.Domain.Interfaces;

namespace ETicket.ApplicationServices.Services.DocumentTypes
{
    public class DocumentTypesService : IDocumentTypesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly MapperService mapper;

        public DocumentTypesService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            mapper = new MapperService();
        }

        public IEnumerable<DocumentType> GetDocumentTypes()
        {
            return unitOfWork.DocumentTypes.GetAll().ToList();
        }

        public DocumentType GetDocumentTypeById(int id)
        {
            return unitOfWork.DocumentTypes.Get(id);
        }

        public void Create(DocumentTypeDto documentTypeDto)
        {
            var documentType = mapper.Map<DocumentTypeDto, DocumentType>(documentTypeDto);

            unitOfWork.DocumentTypes.Create(documentType);
            unitOfWork.Save();
        }

        public void Update(DocumentTypeDto documentTypeDto)
        {
            var documentType = mapper.Map<DocumentTypeDto, DocumentType>(documentTypeDto);

            unitOfWork.DocumentTypes.Create(documentType);
            unitOfWork.Save();
        }

        public void Delete(int id)
        {
            unitOfWork.DocumentTypes.Delete(id);
            unitOfWork.Save();
        }

        public bool Exists(int id)
        {
            return unitOfWork.DocumentTypes.Get(id) != null;
        }
    }
}
