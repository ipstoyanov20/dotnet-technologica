using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL.DataAccess.Models;
using TL.Travel.DomainModels.Feeding;

namespace TL.Travel.Interfaces
{
    public interface IFeedingTypeService : IGenericCrudService<FeedingType>
    {
        new IQueryable<FeedingVM> GetAll();
        new FeedingVM? GetById(int id);
        FeedingVM? AddEdit(FeedingUM feeding, int id = 0);
        new bool Delete(int id);
    }
}
