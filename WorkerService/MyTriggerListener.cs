using Quartz;
using Quartz.Listener;

namespace WorkerService
{
    internal class MyTriggerListener : TriggerListenerSupport
    {
        private readonly ILogger<MyTriggerListener> _logger;

        public MyTriggerListener(ILogger<MyTriggerListener> logger)
        {
            _logger = logger;
        }

        public override string Name => nameof(MyTriggerListener);

        public ITrigger NextTrigger { get; internal set; }

        public override Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("---> Trigger completed");
            context.Scheduler.ScheduleJob(this.NextTrigger);

            return base.TriggerComplete(trigger, context, triggerInstructionCode, cancellationToken);
        }
    }
}
