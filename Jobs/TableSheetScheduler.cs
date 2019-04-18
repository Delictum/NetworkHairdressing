using Quartz;
using Quartz.Impl;

namespace NetworkHairdressing.Jobs
{
    public class TableSheetScheduler
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<AutoTimeSheetCreater>().Build();

            ITrigger trigger = TriggerBuilder.Create() 
                .WithIdentity("autoTimeSheetCreaterTrigger", "generalizedGroup")    
                .StartNow()                           
                .WithSimpleSchedule(x => x  
                    .WithIntervalInHours(24)          
                    .RepeatForever())               
                .Build();                            

            await scheduler.ScheduleJob(job, trigger);  
        }
    }
}