using TL.DataAccess.Models;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.PaymentChannels;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class PaymentChannelService : BaseService, IPaymentChannelService
    {
        public PaymentChannelService(BaseTLTravelDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<PaymentChannelVM> GetAll()
        {
            return Db.PaymentChannels.Where(pc => pc.IsActive).Select(pc => new PaymentChannelVM
            {
                Id = pc.Id,
                Name = pc.Name,
                IsActive = pc.IsActive
            });
        }

        public PaymentChannelVM GetById(int id)
        {
            var paymentChannel = Db.PaymentChannels.FirstOrDefault(pc => pc.Id == id && pc.IsActive);
            return paymentChannel != null ? new PaymentChannelVM
            {
                Id = paymentChannel.Id,
                Name = paymentChannel.Name,
                IsActive = paymentChannel.IsActive
            } : throw new ArgumentException("Deleted payment channel");
        }

        public PaymentChannelVM AddEdit(PaymentChannelUM body, int id = 0)
        {
            PaymentChannel paymentChannel;
            if (id > 0)
            {
                paymentChannel = Db.PaymentChannels.FirstOrDefault(pc => pc.Id == id && pc.IsActive);
                if (paymentChannel == null)
                    throw new ArgumentException("No such payment channel");
                
                paymentChannel.Name = body.Name;
                paymentChannel.IsActive = body.IsActive;
            }
            else
            {
                paymentChannel = new PaymentChannel
                {
                    Name = body.Name,
                    IsActive = body.IsActive
                };
                Db.PaymentChannels.Add(paymentChannel);
            }

            Db.SaveChanges();

            return new PaymentChannelVM
            {
                Id = paymentChannel.Id,
                Name = paymentChannel.Name,
                IsActive = paymentChannel.IsActive
            };
        }

        public PaymentChannelVM Delete(int id)
        {
            var paymentChannel = Db.PaymentChannels.FirstOrDefault(pc => pc.Id == id && pc.IsActive);
            if (paymentChannel == null)
                throw new ArgumentException("No such payment channel");
            
            paymentChannel.IsActive = false;
            Db.SaveChanges();
            
            return new PaymentChannelVM
            {
                Id = paymentChannel.Id,
                Name = paymentChannel.Name,
                IsActive = paymentChannel.IsActive
            };
        }
    }
}
