using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL.DataAccess.Models;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Feeding;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class FeedingTypeService : BaseService, IFeedingTypeService
    {
        public FeedingTypeService(BaseTLTravelDbContext dbContext)
            : base(dbContext)
        {
        }
        public IQueryable<FeedingVM> GetAll()
        {
            var query = Db.FeedingTypes.Where(x => x.IsActive);
            return query.Select(ft => new FeedingVM
            {
                Id = ft.Id,
                Name = ft.Name,
                IsActive = ft.IsActive,
                CreatedBy = ft.CreatedBy,
                CreatedOn = ft.CreatedOn,
                UpdatedBy = ft.UpdatedBy,
                UpdatedOn = ft.UpdatedOn
            });
        }
        public FeedingVM? GetById(int id)
        {
            var feedingType = Db.FeedingTypes.FirstOrDefault(ft => ft.Id == id && ft.IsActive);
            if (feedingType == null) return null;
            return new FeedingVM
            {
                Id = feedingType.Id,
                Name = feedingType.Name,
                IsActive = feedingType.IsActive,
                Code = feedingType.Code,
                CreatedOn = feedingType.CreatedOn,
                CreatedBy = feedingType.CreatedBy,
                UpdatedBy = feedingType.UpdatedBy,
                UpdatedOn = feedingType.UpdatedOn
            };
        }
        public FeedingVM AddEdit(FeedingUM feedingType, int id = 0)
        {
            FeedingType entity;
            if (id > 0)
            {
                entity = Db.FeedingTypes.FirstOrDefault(ft => ft.Id == id);
                if (entity == null) throw new Exception("Feeding type not found.");
                entity.Name = feedingType.Name;
                entity.IsActive = feedingType.IsActive;
                entity.Code = feedingType.Code;
            }
            else
            {
                entity = new FeedingType
                {
                    Name = feedingType.Name,
                    IsActive = feedingType.IsActive,
                    Code = feedingType.Code,
                };
                Db.FeedingTypes.Add(entity);
            }
            Db.SaveChanges();
            return new FeedingVM
            {
                Id = entity.Id,
                Name = entity.Name,
                IsActive = entity.IsActive,
                Code = entity.Code
                
            };
        }

        public bool Delete(int id)
        {
            return SoftDeleteById<FeedingType>(id);
        }
    }
}
