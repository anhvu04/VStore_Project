namespace VStore.Domain.Abstractions.Entities;

public interface IDateTracking
{
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}