﻿using ETicket.DataAccess.Domain.Repositories;

namespace ETicket.DataAccess.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        DocumentRepository Documents { get; }

        DocumentTypeRepository DocumentTypes { get; }

        PrivilegeRepository Privileges { get; }

        TicketRepository Tickets { get; }

        TicketTypeRepository TicketTypes { get; }

        TransactionHistoryRepository TransactionHistory { get; }

        UserRepository Users { get; }

        CarrierRepository Carriers { get; }

        SecretCodeRepository SecretCodes { get; }

        StationRepository Stations { get; }

        AreaRepository Areas { get; }

        RouteRepository Routes { get; }

        TicketVerificationRepository TicketVerifications { get; }

        void Save();
    }
}
