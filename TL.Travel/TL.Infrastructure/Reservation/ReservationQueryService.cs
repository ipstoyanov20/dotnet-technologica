using System;
using System.Linq;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Reservation;

namespace TL.Travel.Infrastructure.Reservations
{
    public class ReservationQueryService
    {
        private readonly BaseTLTravelDbContext _dbContext;
        private readonly ReservationPricingService _pricingService;

        public ReservationQueryService(BaseTLTravelDbContext dbContext, ReservationPricingService pricingService)
        {
            _dbContext = dbContext;
            _pricingService = pricingService;
        }

        public IQueryable<ReservationVM> GetAll()
        {
            var reservations = from r in _dbContext.Reservations
                               join c in _dbContext.Clients on r.ClientId equals c.Id
                               join o in _dbContext.Agents on r.OperatorId equals o.Id
                               join rs in _dbContext.ReservationStatuses on r.ReservationStatusId equals rs.Id
                               where r.IsActive
                               select new
                               {
                                   r.Id,
                                   r.DateFrom,
                                   r.DateTo,
                                   r.CustomerNotes,
                                   r.IsActive,
                                   r.ClientId,
                                   ClientName = c.Name,
                                   r.OperatorId,
                                   OperatorName = o.Name,
                                   r.ReservationStatusId,
                                   ReservationStatusName = rs.Name
                               };

            return reservations.ToList().Select(r => new ReservationVM
            {
                Id = r.Id,
                DateFrom = r.DateFrom,
                DateTo = r.DateTo,
                CustomerNotes = r.CustomerNotes,
                TotalPrice = _pricingService.CalculateTotalPriceFromRooms(r.Id),
                IsActive = r.IsActive,
                ClientId = r.ClientId,
                ClientName = r.ClientName,
                OperatorId = r.OperatorId,
                OperatorName = r.OperatorName,
                ReservationStatusId = r.ReservationStatusId,
                ReservationStatusName = r.ReservationStatusName
            }).AsQueryable();
        }

        public IQueryable<NomenclatureVMRooms> GetAllBusyRooms(NomenclatureBusyRooms body)
        {
            var busyReservations = from r in _dbContext.Reservations
                                   join rr in _dbContext.ReservationRooms on r.Id equals rr.ReservationId
                                   join hr in _dbContext.HotelRooms on rr.HotelRoomId equals hr.Id
                                   join h in _dbContext.Hotels on hr.HotelId equals h.Id
                                   where r.IsActive && r.DateTo >= body.DateFrom && r.DateFrom <= body.DateTo && h.Id == body.HotelId
                                   select new
                                   {
                                       RoomTypeName = hr.Name,
                                   };

            return busyReservations.ToList()
                .GroupBy(r => r.RoomTypeName)
                .Select(g => new NomenclatureVMRooms
                {
                    RoomType = g.Key,
                    BusyCount = g.Count()
                }).AsQueryable();
        }

        public ReservationVM GetById(int id)
        {
            var reservationData = (from r in _dbContext.Reservations
                                   join c in _dbContext.Clients on r.ClientId equals c.Id
                                   join o in _dbContext.Agents on r.OperatorId equals o.Id
                                   join rs in _dbContext.ReservationStatuses on r.ReservationStatusId equals rs.Id
                                   where r.Id == id && r.IsActive
                                   select new
                                   {
                                       r.Id,
                                       r.DateFrom,
                                       r.DateTo,
                                       r.CustomerNotes,
                                       r.IsActive,
                                       r.ClientId,
                                       ClientName = c.Name,
                                       r.OperatorId,
                                       OperatorName = o.Name,
                                       r.ReservationStatusId,
                                       ReservationStatusName = rs.Name
                                   }).FirstOrDefault();

            if (reservationData == null)
                throw new ArgumentException("Reservation not found");

            return new ReservationVM
            {
                Id = reservationData.Id,
                DateFrom = reservationData.DateFrom,
                DateTo = reservationData.DateTo,
                CustomerNotes = reservationData.CustomerNotes,
                TotalPrice = _pricingService.CalculateTotalPriceFromRooms(reservationData.Id),
                IsActive = reservationData.IsActive,
                ClientId = reservationData.ClientId,
                ClientName = reservationData.ClientName,
                OperatorId = reservationData.OperatorId,
                OperatorName = reservationData.OperatorName,
                ReservationStatusId = reservationData.ReservationStatusId,
                ReservationStatusName = reservationData.ReservationStatusName
            };
        }
    }
}
