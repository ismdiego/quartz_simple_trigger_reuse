using Quartz;

namespace WorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHostBuilder builder = Host.CreateDefaultBuilder(args);

            IHost host = builder
                .ConfigureServices((ctx, services) =>
                {
                    services.AddQuartz();
                    services.AddQuartzHostedService();
                })
                .Build();

            ISchedulerFactory schedulerFactory = host.Services.GetRequiredService<ISchedulerFactory>();
            IScheduler scheduler = await schedulerFactory.GetScheduler();

            IJobDetail jobDetail = JobBuilder.Create<HelloJob>()
                .WithIdentity("helloJob", "group1")
                .Build();

            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger1);

            await host.RunAsync();
        }
    }
}