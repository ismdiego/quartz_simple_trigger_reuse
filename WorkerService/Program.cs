using Quartz;
using Quartz.Impl.Matchers;

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
                    services.AddSingleton<MyTriggerListener>();
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

            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group1")
                .StartNow()
                .ForJob(jobDetail)
                .UsingJobData("SECONDARY", true)
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(2)
                    .WithRepeatCount(2))
                .Build();

            var listener = host.Services.GetRequiredService<MyTriggerListener>();
            listener.NextTrigger = trigger2;

            await scheduler.ScheduleJob(jobDetail, trigger1);
            scheduler.ListenerManager.AddTriggerListener(listener, KeyMatcher<TriggerKey>.KeyEquals(trigger1.Key));

            await host.RunAsync();
        }
    }
}