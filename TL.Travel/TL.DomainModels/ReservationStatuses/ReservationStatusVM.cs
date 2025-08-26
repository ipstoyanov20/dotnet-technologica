namespace TL.Travel.DomainModels.ReservationStatuses
{
    public class ReservationStatusVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
