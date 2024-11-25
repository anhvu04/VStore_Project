using Microsoft.Extensions.Logging;
using Quartz;
using VStore.Application.Abstractions.QuartzService;
using VStore.Infrastructure.Quartz.PayOsService;

namespace VStore.Infrastructure.Quartz;

public class QuartzService : IQuartzService
{
    private readonly IScheduler _scheduler;
    private readonly ILogger<QuartzService> _logger;

    public QuartzService(IScheduler scheduler, ILogger<QuartzService> logger)
    {
        _scheduler = scheduler;
        _logger = logger;
    }

    public async Task CheckPayOsPaymentStatusJobAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scheduling job: {JobName}", nameof(PayOsPaymentStatus));
        var jobKey = new JobKey(nameof(PayOsPaymentStatus));
        var job = JobBuilder.Create<PayOsPaymentStatus>()
            .WithIdentity(jobKey)
            .Build();
        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{jobKey.Name}@Trigger")
            .ForJob(job)
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(60) // check every 60 seconds
                .RepeatForever())
            .Build();
        await _scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public async Task StartSchedulerAsync(CancellationToken cancellationToken)
    {
        await _scheduler.Start(cancellationToken);
    }
}