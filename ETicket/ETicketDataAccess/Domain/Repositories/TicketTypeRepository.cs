using System.Collections.Generic;
using System.Linq;
using ETicket.DataAccess.Domain.Entities;
using ETicket.DataAccess.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ETicket.DataAccess.Domain.Repositories
{
    public class TicketTypeRepository : IRepository<TicketType, int>
    {
        private readonly ETicketDataContext context;

        public TicketTypeRepository(ETicketDataContext context)
        {
            this.context = context;
        }
        
        public virtual IQueryable<TicketType> GetAll()
        {
            return context.TicketTypes;
        }

        public virtual TicketType Get(int id)
        {
            return context.TicketTypes.Find(id);
        }

        public virtual void Create(TicketType ticketType)
        {
            context.TicketTypes.Add(ticketType);
        }

        public virtual void Update(TicketType ticketType)
        {
            context.Entry(ticketType).State = EntityState.Modified;
        }

        public virtual void Delete(int id)
        {
            var ticketType = context.TicketTypes.Find(id);
            
            if (ticketType != null)
            {
                context.TicketTypes.Remove(ticketType);
            }
        }
    }
}