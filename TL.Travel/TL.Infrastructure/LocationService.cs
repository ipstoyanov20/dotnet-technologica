using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL.DataAccess.Models;
using TL.Travel.DataAccess.Base;
using TL.Travel.DataAccess.Oracle;
using TL.Travel.DomainModels.Location;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class LocationService : BaseService, ILocationService
    {
        public LocationService(BaseTLTravelDbContext dbContext) : base(dbContext)
        {

        }
        public IQueryable<LocationVM> GetAll()
        {
            return Db.Locations.Where(x => x.IsActive)
                .Select(l => new LocationVM { Id = l.Id, Name = l.Name, IsAbroad = l.IsAbroad, IsActive = l.IsActive });
        }

        public LocationVM GetById(int id)
        {
            var location = Db.Locations.FirstOrDefault(x => x.Id == id && x.IsActive);
            if (location == null)
            {
                throw new ArgumentException("No such location");
            }
            return new LocationVM { Id = location.Id, Name = location.Name, IsAbroad = location.IsAbroad, IsActive = location.IsActive };
        }

        public LocationVM AddEdit(LocationUM location, int id = 0)
        {
            if (id > 0)
            {
                var existingLocation = Db.Locations.FirstOrDefault(x => x.Id == id && x.IsActive);
                if (existingLocation == null) throw new Exception("Location not found.");
                existingLocation.Name = location.Name;
                existingLocation.IsAbroad = location.IsAbroad;
                existingLocation.IsActive = location.IsActive;
                Db.Locations.Update(existingLocation);
            }
            else
            {
                var newLocation = new Location
                {
                    Name = location.Name,
                    IsAbroad = location.IsAbroad,
                    IsActive = true
                };
                Db.Locations.Add(newLocation);
            }
            Db.SaveChanges();
            return new LocationVM { Id = id, Name = location.Name, IsAbroad = location.IsAbroad, IsActive = location.IsActive };
        }

        public void Delete(int id)
        {
            var location = Db.Locations.FirstOrDefault(x => x.Id == id && x.IsActive);
            if (location == null) throw new Exception("Location not found.");
            location.IsActive = false;
            Db.SaveChanges();
        }


    }
}
