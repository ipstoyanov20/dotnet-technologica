using TL.Travel.DomainModels.Operators;

namespace TL.Travel.DomainModels.Reservation
{
    public class ClientHotelsDTO
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public List<NomenclatureDTO> Hotels { get; set; } = new();
    }

    public class RoomFeedingTypesDTO
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public decimal RoomPrice { get; set; }
        public List<NomenclatureDTO> FeedingTypes { get; set; } = new();
    }

    public class PaymentOptionsDTO
    {
        public List<NomenclatureDTO> PaymentTypes { get; set; } = new();
        public List<NomenclatureDTO> FeedingTypes { get; set; } = new();
        public List<NomenclatureDTO> PaymentChannels { get; set; } = new();
    }
}
