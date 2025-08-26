using TL.DataAccess.Models;
using TL.Travel.DomainModels.Payment;

namespace TL.Travel.Interfaces
{
    public interface IPaymentTypeService : IGenericCrudService<PaymentType>
    {
        new IQueryable<PaymentVM> GetAll();
        new PaymentVM? GetById(int id);
        PaymentVM? AddEdit(PaymentUM payment, int id = 0);
        new bool Delete(int id);
    }
}
