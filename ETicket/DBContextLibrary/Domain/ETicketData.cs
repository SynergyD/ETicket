﻿using System;

using ETicket.Domain.Entities;
using ETicket.Domain.Repositories;

namespace ETicket.Domain
{
    public class ETicketData : IDisposable
    {
        #region private members

        private ETicketDataContext eTicketDataContext;

        private DocumentRepository documentRepository;
        private DocumentTypeRepository documentTypeRepository;
        private PrivilegeRepository privilegeRepository;
        private RoleRepository roleRepository;
        private TicketRepository ticketRepository;
        private TicketTypeRepository ticketTypeRepository;
        private TransactionHistoryRepository transactionHistoryRepository;
        private UserRepository userRepository;

        #endregion

        public DocumentRepository Documents
        {
            get
            {
                if (documentRepository == null)
                    documentRepository = new DocumentRepository(eTicketDataContext);
                return documentRepository;
            }
        }

        public DocumentTypeRepository DocumentTypes
        {
            get
            {
                if (documentTypeRepository == null)
                    documentTypeRepository = new DocumentTypeRepository(eTicketDataContext);
                return documentTypeRepository;
            }
        }

        public PrivilegeRepository Privileges
        {
            get
            {
                if (privilegeRepository == null)
                    privilegeRepository = new PrivilegeRepository(eTicketDataContext);
                return privilegeRepository;
            }
        }

        public RoleRepository Roles
        {
            get
            {
                if (roleRepository == null)
                    roleRepository = new RoleRepository(eTicketDataContext);
                return roleRepository;
            }
        }

        public TicketRepository Tickets
        {
            get
            {
                if (ticketRepository == null)
                    ticketRepository = new TicketRepository(eTicketDataContext);
                return ticketRepository;
            }
        }

        public TicketTypeRepository TicketTypes
        {
            get
            {
                if (ticketTypeRepository == null)
                    ticketTypeRepository = new TicketTypeRepository(eTicketDataContext);
                return ticketTypeRepository;
            }
        }

        public TransactionHistoryRepository TransactionHistory
        {
            get
            {
                if (transactionHistoryRepository == null)
                    transactionHistoryRepository = new TransactionHistoryRepository(eTicketDataContext);
                return transactionHistoryRepository;
            }
        }

        public UserRepository Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(eTicketDataContext);
                return userRepository;
            }
        }

        public void Save()
        {
            eTicketDataContext.SaveChanges();
        }

        public ETicketData(ETicketDataContext eTicketDataContext)
        {
            this.eTicketDataContext = eTicketDataContext;
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    eTicketDataContext.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
