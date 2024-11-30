using VStore.Domain.Enums;

namespace VStore.Application.Usecases.Order.Common;

public record OrderLogModel
{
    public string Status { get; set; }
    public DateTime CreatedDate { get; set; }
}