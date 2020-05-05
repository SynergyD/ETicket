﻿using ETicket.DataAccess.Domain.Entities;
using ETicket.DataAccess.Domain.Interfaces;
using ETicket.DataAccess.Domain.Repositories;
using System;

namespace ETicket.DataAccess.Domain
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region private members

        private ETicketDataContext eTicketDataContext;

        private DocumentRepository documentRepository;
        private DocumentTypeRepository documentTypeRepository;
        private PrivilegeRepository privilegeRepository;
        private TicketRepository ticketRepository;
        private TicketTypeRepository ticketTypeRepository;
        private TransactionHistoryRepository transactionHistoryRepository;
        private UserRepository userRepository;
        private RouteRepository routeRepository;
        private CarrierRepository carrierRepository;
        private RouteStationRepository routeStationRepository;
        private AreaRepository areaRepository;
        private StationRepository stationRepository;
        private PriceListRepository priceListRepository;
        private TransportRepository transportRepository;
        private TicketAreaRepository ticketAreaRepository;
        private TicketVerificationRepository ticketVerificationRepository;

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

        public RouteRepository Routes
        {
            get
            {
                if (routeRepository == null)
                    routeRepository = new RouteRepository(eTicketDataContext);
                return routeRepository;
            }
        }

        public CarrierRepository Carriers
        {
            get
            {
                if(carrierRepository == null)
                {
                    carrierRepository = new CarrierRepository(eTicketDataContext);
                }

                return carrierRepository;
            }
        }

        public RouteStationRepository RouteStation
        {
            get
            {
                if (routeStationRepository == null)
                {
                    routeStationRepository = new RouteStationRepository(eTicketDataContext);
                }

                return routeStationRepository;
            }
        }

        public AreaRepository Areas => areaRepository ??= new AreaRepository(eTicketDataContext);
        
        public PriceListRepository PriceList
        {
            get
            {
                if(priceListRepository == null)
                {
                    priceListRepository = new PriceListRepository(eTicketDataContext);
                }

                return priceListRepository;
            }
        }
        

        public StationRepository Stations
        {
            get
            {
                if (stationRepository == null)
                    stationRepository = new StationRepository(eTicketDataContext);

                return stationRepository;
            }
        }

        public TransportRepository Transports
        {
            get
            {
                if (transportRepository == null)
                {
                    transportRepository = new TransportRepository(eTicketDataContext);
                }

                return transportRepository;
            }
        }
        
        public TicketAreaRepository TicketAreas
        {
            get
            {
                if (ticketAreaRepository == null)
                {
                    ticketAreaRepository = new TicketAreaRepository(eTicketDataContext);
                }

                return ticketAreaRepository;
            }
        }


        public TicketVerificationRepository TicketVerifications
        {
            get
            {
                if (ticketVerificationRepository == null)
                {
                    ticketVerificationRepository = new TicketVerificationRepository(eTicketDataContext);
                }
                return ticketVerificationRepository;
            }
        }

        public void Save()
        {
            eTicketDataContext.SaveChanges();
        }

        public UnitOfWork(ETicketDataContext eTicketDataContext)
        {
            this.eTicketDataContext = eTicketDataContext;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
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
