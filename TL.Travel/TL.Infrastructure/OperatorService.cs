using TL.AspNet.Security.Abstractions.Services;
using TL.DataAccess.Models;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Operators;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class OperatorService : BaseService, IOperatorService
    {
        private IPasswordHasher passwordHasher;

        public OperatorService(BaseTLTravelDbContext dbContext, IPasswordHasher passwordHasher)
            : base(dbContext)
        {
            this.passwordHasher = passwordHasher;
        }

        public IQueryable<OperatorDTO> GetAll()
        {
            var res = Db.Agents.Where(x => x.IsActive);

            return res.Select(x => new OperatorDTO()
            {
                Id = x.Id,
                Name = x.Name,
                ComissionPercent = x.ComissionPercent
            });
        }

        public OperatorDTO AddOrUpdate(EditOperatorDTO entry, int id = 0)
        {
            Agent dbEntry;

            if (entry.Id.HasValue && entry.Id != 0)
            {
                dbEntry = Db.Agents.Find(id);
                dbEntry.Name = entry.Name;
                dbEntry.ComissionPercent = entry.ComissionPercent;
            }
            else
            {
                dbEntry = new Agent();

                dbEntry = Db.Agents.Add(dbEntry).Entity;
            }

            dbEntry.Name = entry.Name;
            dbEntry.ComissionPercent = entry.ComissionPercent;

            Db.SaveChanges();



            return new OperatorDTO()
            {
                Id = dbEntry.Id,
                Name = dbEntry.Name,
                ComissionPercent = dbEntry.ComissionPercent
            };
        }

        public OperatorDTO GetById(int id)
        {
            var dbEntry = Db.Agents.Find(id);

            return new OperatorDTO()
            {

                Id = dbEntry.Id,
                Name = dbEntry.Name,
                ComissionPercent = dbEntry.ComissionPercent

            };

        }

        public void Delete(int id)
        {
            var del = Db.Agents.Find(id);
            del.IsActive = false;
            Db.SaveChanges();
        }

    }
}
