namespace TL.Travel.DataAccess.Abstractions.Interfaces
{
    public interface IAuditable
    {
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
