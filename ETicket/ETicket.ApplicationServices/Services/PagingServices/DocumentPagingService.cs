﻿using ETicket.ApplicationServices.Services.DataTable.Interfaces;
using ETicket.DataAccess.Domain.Entities;
using ETicket.DataAccess.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ETicket.ApplicationServices.Services.PagingServices
{
    public class DocumentPagingService : IDataTablePagingService<Document>
    {
        private readonly IUnitOfWork unitOfWork;

        public DocumentPagingService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IQueryable<Document> GetAll()
        {
            return unitOfWork.Documents
                .GetAll()
                .Include(t => t.DocumentType);
        }

        public IDictionary<string, Expression<Func<Document, object>>> GetSortExpressions()
        {
            return new Dictionary<string, Expression<Func<Document, object>>>
            {
                { "documentType", (t => t.DocumentType.Name) },
                { "number", (t => t.Number) },
                { "expirationDate", (t => t.ExpirationDate) },
                { "isValid", (t => t.IsValid) }
            };
        }

        public DateTime ParseDateTime(string parseValue)
        {
            if (DateTime.TryParse(parseValue, out DateTime result))
            {
                return result;
            }
            return new DateTime();
        }
        public bool? ParseBoolean(string parseValue)
        {
            if (Boolean.TryParse(parseValue, out bool result))
            {
                return result;
            }
            return null;
        }
        public Expression<Func<Document, bool>> GetSingleFilterExpression(string columnName, string filterValue)
        {
            return columnName switch
            {
                "documentType" => (t => t.DocumentType.Name.StartsWith(filterValue)),
                "number" => (t => t.Number.StartsWith(filterValue)),
                "expirationDate" => (t => t.ExpirationDate.Value.Date == ParseDateTime(filterValue).Date),
                "isValid" => (t => t.IsValid == ParseBoolean(filterValue)),
                _ => (t => true)
            };
        }

        public IList<Expression<Func<Document, bool>>> GetFilterExpressions(string[] columnNames, string[] filterValues)
        {
            var result = new List<Expression<Func<Document, bool>>>();

            for (int i = 0; i < columnNames.Length; i++)
            {
                result.Add(GetSingleFilterExpression(columnNames[i], filterValues[i]));
            }

            return result;
        }

        public IList<Expression<Func<Document, bool>>> GetGlobalSearchExpressions(string searchValue)
        {
            return new List<Expression<Func<Document, bool>>>
            {
                (t => t.Number.StartsWith(searchValue)),
                (t => t.DocumentType.Name.StartsWith(searchValue)),
                (t => t.ExpirationDate.ToString().Contains(searchValue)),
            };
        }
    }
}
