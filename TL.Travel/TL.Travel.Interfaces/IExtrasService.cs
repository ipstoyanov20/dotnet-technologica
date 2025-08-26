using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL.DataAccess.Models;
using TL.Travel.DomainModels.Extras;

namespace TL.Travel.Interfaces
{
    public interface IExtrasService : IGenericCrudService<Extra>
    {
        IQueryable<ExtrasVM> GetAllExtras();
        ExtrasVM GetExtraById(int id);
        ExtrasVM AddEdit(ExtrasUM extra, int id = 0);
        Task DeleteExtra(int id);
    }
}
