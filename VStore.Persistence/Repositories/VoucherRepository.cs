using VStore.Domain.Abstractions.Repositories;
using VStore.Domain.Entities;

namespace VStore.Persistence.Repositories;

public class VoucherRepository : RepositoryBase<Voucher, Guid>, IVoucherRepository
{
    public VoucherRepository(ApplicationDbContext context) : base(context)
    {
    }
}