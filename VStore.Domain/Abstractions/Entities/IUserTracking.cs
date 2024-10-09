namespace VStore.Domain.Abstractions.Entities;

public interface IUserTracking
{
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
}