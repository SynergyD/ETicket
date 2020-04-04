﻿using System.Collections.Generic;
using ETicket.Domain.Entities;
using ETicket.Domain.Interfaces;

namespace ETicket.Domain.Repositories
{
    public class TransactionHistoryRepository : IRepository<TransactionHistory>
    {
        #region Private Members

        private readonly ETicketDataContext eTicketDataContext;

        #endregion

        public TransactionHistoryRepository(ETicketDataContext eTicketDataContext)
        {
            this.eTicketDataContext = eTicketDataContext;
        }

        public void Create(TransactionHistory transaction)
        {
            // TODO
        }

        public void Delete(int id)
        {
            // TODO
        }

        public TransactionHistory Get(int id)
        {
            // TODO

            var transaction = eTicketDataContext.TransactionHistory.Find(id);

            return transaction;
        }

        public IEnumerable<TransactionHistory> GetAll()
        {
            return eTicketDataContext.TransactionHistory;
        }

        public void Update(TransactionHistory transaction)
        {
            // TODO
        }
    }
}