using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL.DataAccess.Models;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Extras;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class ExtrasService : BaseService, IExtrasService
    {
        public ExtrasService(BaseTLTravelDbContext dbContext) : base(dbContext)
        {

        }

        public IQueryable<ExtrasVM> GetAllExtras()
        {
            return Db.Extras.Where(e => e.IsActive).Select(e => new ExtrasVM
            {
                Id = e.Id,
                Name = e.Name,
                IsActive = e.IsActive
            });

        }

        public ExtrasVM GetExtraById(int id)
        {
            var extra = Db.Extras.FirstOrDefault(e => e.Id == id && e.IsActive);
            return extra != null ? new ExtrasVM
            {
                Id = extra.Id,
                Name = extra.Name,
                IsActive = extra.IsActive
            } : throw new ArgumentException("Deleted extra");
        }

        public ExtrasVM AddEdit(ExtrasUM body, int id = 0)
        {
            Extra extra;
            if (id > 0)
            {
                extra = Db.Extras.FirstOrDefault(e => e.Id == id && e.IsActive);
                if (extra == null)
                    throw new ArgumentException("No such extra");
                extra.Name = body.Name;
                extra.IsActive = body.IsActive;
            }
            else
            {
                extra = new Extra
                {
                    Name = body.Name,
                    IsActive = body.IsActive
                };
                Db.Extras.Add(extra);
            }

            Db.SaveChanges();

            return new ExtrasVM
            {
                Id = extra.Id,
                Name = extra.Name,
                IsActive = extra.IsActive
            };
        }

        public async Task DeleteExtra(int id)
        {
            var delExtra = Db.Extras.FirstOrDefault(e => e.Id == id && e.IsActive);
            if (delExtra == null)
                throw new ArgumentException("No such extra");
            delExtra.IsActive = false;
            Db.SaveChanges();
        }

        public ExtrasVM Delete(int id)
        {
            var delExtra = Db.Extras.FirstOrDefault(e => e.Id == id && e.IsActive);
            if (delExtra == null)
                throw new ArgumentException("No such extra");
            delExtra.IsActive = false;
            Db.SaveChanges();
            return new ExtrasVM
            {
                Id = delExtra.Id,
                Name = delExtra.Name,
                IsActive = delExtra.IsActive
            };
        }
    }
}
