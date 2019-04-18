using Quartz;
using Quartz.Impl;

namespace NetworkHairdressing.Jobs
{
    public class EmailScheduler
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailSender>().Build();

            ITrigger trigger = TriggerBuilder.Create() 
                .WithIdentity("trigger1", "group1") 
                .StartNow()           
                .WithSimpleSchedule(x => x           
                    .WithIntervalInMinutes(1)     
                    .RepeatForever())              
                .Build();                             

            await scheduler.ScheduleJob(job, trigger);      
        }
    }
}