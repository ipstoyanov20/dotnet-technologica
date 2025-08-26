using System.Linq;
using TL.Travel.DomainModels.Reservation;

namespace TL.Travel.Interfaces
{
    public interface IReservationService : IService
    {
        IQueryable<ReservationVM> GetAll();
        ReservationVM GetById(int id);
        ReservationVM AddEdit(ReservationUM body, int id = 0);
        void Delete(int id);

        IQueryable<NomenclatureVMRooms> GetAllBusyRooms(NomenclatureBusyRooms body);
    }
}
