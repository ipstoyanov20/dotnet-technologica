using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL.Travel.DomainModels.Location;

namespace TL.Travel.Interfaces
{
    public interface ILocationService : IService
    {
        IQueryable<LocationVM> GetAll();
        LocationVM GetById(int id);
        LocationVM AddEdit(LocationUM location, int id = 0);
        void Delete(int id);


    }
}
