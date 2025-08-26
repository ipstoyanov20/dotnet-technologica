using TL.WebHelpers.Models.RequestPaging;

namespace TL.Travel.DomainModels.HotelRoom
{
    public class HotelRoomFilters : BaseRequestModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
