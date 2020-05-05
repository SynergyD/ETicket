using System.Collections.Generic;
using ETicket.ApplicationServices.DTOs;
using ETicket.DataAccess.Domain.Entities;

namespace ETicket.ApplicationServices.Services.Interfaces
{
    public interface IAreaService
    {
        public IEnumerable<Area> GetAll();
        
        public Area Get(int id);

        public void Create(AreaDto areaDto);

        public void Update(AreaDto areaDto);

        public void Delete(int id);

        public bool Exists(int id);
    }
}