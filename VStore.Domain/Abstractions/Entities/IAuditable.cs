namespace VStore.Domain.Abstractions.Entities;

public interface IAuditable : IUserTracking, IDateTracking, ISoftDelete
{
}