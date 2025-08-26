namespace TL.Travel.DataAccess.Abstractions.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsActive { get; set; }
    }
}
