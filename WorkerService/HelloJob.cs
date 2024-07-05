using Quartz;

namespace WorkerService
{
    public class HelloJob : IJob
    {
        private readonly ILogger<HelloJob> _logger;

        public HelloJob(ILogger<HelloJob> logger)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                context.MergedJobDataMap.TryGetBooleanValue("SECONDARY", out bool secondaryTrigger);
                var triggerType = secondaryTrigger ? "((Secondary))" : "Primary";

                _logger.LogInformation("{TriggerType} Worker running at: {time}", triggerType, DateTimeOffset.Now.ToString("HH:mm:ss.fff"));

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }
    }
}
