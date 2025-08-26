using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL.Travel.DataAccess.Base;
using TL.DataAccess.Models;
using TL.Travel.DomainModels.Feeding;
using TL.Travel.Interfaces;
using TL.Travel.DomainModels.Payment;
namespace TL.Travel.Infrastructure
{
    public class PaymentTypeService
        : BaseService, IPaymentTypeService
    {
        public PaymentTypeService(BaseTLTravelDbContext dbContext)
            : base(dbContext)
        {
        }
        public IQueryable<PaymentVM> GetAll()
        {
            var query = Db.PaymentTypes.Where(x => x.IsActive);

            return query.Select(pt => new PaymentVM
            {
                Id = pt.Id,
                Name = pt.Name,
                IsActive = pt.IsActive,
                CreatedBy = pt.CreatedBy,
                CreatedOn = pt.CreatedOn,
                UpdatedBy = pt.UpdatedBy,
                UpdatedOn = pt.UpdatedOn

            });
        }
        public PaymentVM? GetById(int id)
        {
            var paymentType = Db.PaymentTypes.FirstOrDefault(pt => pt.Id == id && pt.IsActive);
            if (paymentType == null) return null;
            return new PaymentVM
            {
                Id = paymentType.Id,
                Name = paymentType.Name,
                IsActive = paymentType.IsActive,
                CreatedOn = paymentType.CreatedOn,
                CreatedBy = paymentType.CreatedBy,
                UpdatedBy = paymentType.UpdatedBy,
                UpdatedOn = paymentType.UpdatedOn
            };
        }
        public PaymentVM AddEdit(PaymentUM paymentType, int id = 0)
        {
            PaymentType entity;
            if (id > 0)
            {
                entity = Db.PaymentTypes.FirstOrDefault(pt => pt.Id == id);
                if (entity == null) throw new Exception("Payment type not found.");

                entity.Name = paymentType.Name;
                entity.IsActive = paymentType.IsActive;
            }
            else
            {
                entity = new PaymentType
                {
                    Name = paymentType.Name,
                    IsActive = paymentType.IsActive
                };

                Db.PaymentTypes.Add(entity);
            }
            Db.SaveChanges();
            return new PaymentVM
            {
                Id = entity.Id,
                Name = entity.Name,
                IsActive = entity.IsActive

            };
        }



        public bool Delete(int id)
        {
            var paymentType = Db.PaymentTypes.FirstOrDefault(pt => pt.Id == id);
            if (paymentType == null) return false;
            paymentType.IsActive = false;
            Db.SaveChanges();
            return true;
        }

    }

}
