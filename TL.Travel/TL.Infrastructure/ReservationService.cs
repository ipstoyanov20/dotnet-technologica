using System.Linq;
using Microsoft.Extensions.Logging;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.Reservation;
using TL.Travel.Infrastructure.Reservations;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class ReservationService : BaseService, IReservationService
    {
        private readonly ILogger<ReservationService> _logger;
        private readonly ReservationQueryService _queryService;
        private readonly ReservationManagementService _managementService;

        public ReservationService(
            BaseTLTravelDbContext dbContext,
            ILogger<ReservationService> logger)
            : base(dbContext)
        {
            _logger = logger;

            var pricingService = new ReservationPricingService(dbContext);
            var validationService = new ReservationValidationService(dbContext);
            var statusManager = new ReservationStatusManager(dbContext);

            _queryService = new ReservationQueryService(dbContext, pricingService);
            _managementService = new ReservationManagementService(
                dbContext,
                validationService,
                pricingService,
                statusManager);
        }

        public IQueryable<ReservationVM> GetAll()
        {
            _logger.LogInformation("Getting all reservations");
            return _queryService.GetAll();
        }

        public ReservationVM GetById(int id)
        {
            _logger.LogInformation("Getting reservation by id: {ReservationId}", id);
            return _queryService.GetById(id);
        }

        public ReservationVM AddEdit(ReservationUM body, int id = 0)
        {
            if (id > 0)
            {
                _logger.LogInformation("Updating reservation with id: {ReservationId}", id);
            }
            else
            {
                _logger.LogInformation("Creating new reservation");
            }

            return _managementService.AddEdit(body, id);
        }

        public void Delete(int id)
        {
            _logger.LogInformation("Deleting reservation with id: {ReservationId}", id);
            _managementService.Delete(id);
        }

        public IQueryable<NomenclatureVMRooms> GetAllBusyRooms(NomenclatureBusyRooms body)
        {
            _logger.LogInformation("Getting all busy rooms");
            return _queryService.GetAllBusyRooms(body);
        }
    }
}
