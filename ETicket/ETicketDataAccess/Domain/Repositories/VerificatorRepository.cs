using System;
using System.Linq;
using ETicket.DataAccess.Domain.Entities;
using ETicket.DataAccess.Domain.Interfaces;

namespace ETicket.DataAccess.Domain.Repositories
{
    public class VerificatorRepository : IRepository<Verificator,Guid>
    {
        private readonly ETicketDataContext context;

        public VerificatorRepository(ETicketDataContext context)
        {
            this.context = context;
        }
        public IQueryable<Verificator> GetAll()
        {
            throw new NotImplementedException();
        }

        public Verificator Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Create(Verificator item)
        {
            throw new NotImplementedException();
        }

        public void Update(Verificator item)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}