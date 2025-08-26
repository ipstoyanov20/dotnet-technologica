using System;
using System.Linq;
using TL.DataAccess.Models;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.ReservationStatuses;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class ReservationStatusService : BaseService, IReservationStatusService
    {
        public ReservationStatusService(BaseTLTravelDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<ReservationStatusVM> GetAll()
        {
            return Db.ReservationStatuses.Where(rs => rs.IsActive).Select(rs => new ReservationStatusVM
            {
                Id = rs.Id,
                Code = rs.Code,
                Name = rs.Name,
                IsActive = rs.IsActive
            });
        }

        public ReservationStatusVM GetById(int id)
        {
            var reservationStatus = Db.ReservationStatuses.FirstOrDefault(rs => rs.Id == id && rs.IsActive);
            return reservationStatus != null ? new ReservationStatusVM
            {
                Id = reservationStatus.Id,
                Code = reservationStatus.Code,
                Name = reservationStatus.Name,
                IsActive = reservationStatus.IsActive
            } : throw new ArgumentException("Deleted reservation status");
        }

        public ReservationStatusVM AddEdit(ReservationStatusUM body, int id = 0)
        {
            ReservationStatus reservationStatus;
            if (id > 0)
            {
                reservationStatus = Db.ReservationStatuses.FirstOrDefault(rs => rs.Id == id && rs.IsActive);
                if (reservationStatus == null)
                    throw new ArgumentException("No such reservation status");

                reservationStatus.Code = body.Code;
                reservationStatus.Name = body.Name;
                reservationStatus.IsActive = body.IsActive;
            }
            else
            {
                reservationStatus = new ReservationStatus
                {
                    Code = body.Code,
                    Name = body.Name,
                    IsActive = body.IsActive
                };
                Db.ReservationStatuses.Add(reservationStatus);
            }

            Db.SaveChanges();

            return new ReservationStatusVM
            {
                Id = reservationStatus.Id,
                Code = reservationStatus.Code,
                Name = reservationStatus.Name,
                IsActive = reservationStatus.IsActive
            };
        }

        public ReservationStatusVM Delete(int id)
        {
            var reservationStatus = Db.ReservationStatuses.FirstOrDefault(rs => rs.Id == id && rs.IsActive);
            if (reservationStatus == null)
                throw new ArgumentException("No such reservation status");

            reservationStatus.IsActive = false;

            Db.SaveChanges();

            return new ReservationStatusVM
            {
                Id = reservationStatus.Id,
                Code = reservationStatus.Code,
                Name = reservationStatus.Name,
                IsActive = reservationStatus.IsActive
            };
        }
    }
}
