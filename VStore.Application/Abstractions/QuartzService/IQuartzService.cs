namespace VStore.Application.Abstractions.QuartzService;

public interface IQuartzService
{
    Task CheckPayOsPaymentStatusJobAsync(CancellationToken cancellationToken);
    Task StartSchedulerAsync(CancellationToken cancellationToken);
}