using VStore.Domain.Entities;

namespace VStore.Domain.Abstractions.Repositories;

public interface IVoucherRepository : IRepositoryBase<Voucher, Guid>
{
}