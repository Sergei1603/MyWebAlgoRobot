using Google.Type;
using MarketRobot.Sheduler.Jobs;
using MarketRobot.Sheduler.Jobs.CN;
using MarketRobot.Sheduler.Jobs.Sber;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using DayOfWeek = System.DayOfWeek;

namespace MarketRobot.Sheduler
{
    public class Sheduler
    {
        //Время здесь по UTC, потому что контейнер по умолчанию работает по UTC
        static protected IScheduler scheduler;
        public static async void StartSber(IServiceProvider serviceProvider)
        {
            try
            {
                scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                scheduler.JobFactory = serviceProvider.GetRequiredService<JobFactory>();
                await scheduler.Start();

                IJobDetail job1 = JobBuilder.Create<OpenPositionsSberJob>().Build();

                //ITrigger trigger1 = TriggerBuilder.Create()  // создаем триггер
                //    .WithIdentity("triggerOpenSber", "Open")     // идентифицируем триггер с именем и группой
                //    .StartNow()
                //    .Build();                               // создаем триггер

                ITrigger trigger1 = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("triggerOpenSber", "Open")     // идентифицируем триггер с именем и группой
                    .WithSchedule(CronScheduleBuilder            // настраиваем выполнение действия
                        .AtHourAndMinuteOnGivenDaysOfWeek(14, 10,
                        new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
                         .WithMisfireHandlingInstructionDoNothing())
                    .Build();                               // создаем триггер

                IJobDetail job2 = JobBuilder.Create<ClosePositionsSberJob>().Build();

                ITrigger trigger2 = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("triggerCloseSber", "Close")     // идентифицируем триггер с именем и группой
                    .StartNow()                            // запуск сразу после начала выполнения
                    .WithSchedule(CronScheduleBuilder            // настраиваем выполнение действия
                        .AtHourAndMinuteOnGivenDaysOfWeek(8, 30,
                        new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
                         .WithMisfireHandlingInstructionDoNothing())
                    .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job2, trigger2);
                await scheduler.ScheduleJob(job1, trigger1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в планировщике сбера: {ex}");
            }
        }

        public static async void StartCN(IServiceProvider serviceProvider)
        {
            try
            {
                scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                scheduler.JobFactory = serviceProvider.GetRequiredService<JobFactory>();
                await scheduler.Start();

                IJobDetail job1 = JobBuilder.Create<OpenPositionsCNJob>().Build();

                ITrigger trigger1 = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("triggerOpenCN", "Open")     // идентифицируем триггер с именем и группой
                    .WithSchedule(CronScheduleBuilder            // настраиваем выполнение действия
                        .AtHourAndMinuteOnGivenDaysOfWeek(10, 30, //время по UTC
                        new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday })
                         .WithMisfireHandlingInstructionDoNothing())
                    .Build();                               // создаем триггер

                IJobDetail job2 = JobBuilder.Create<ClosePositionsCNJob>().Build();

                ITrigger trigger2 = TriggerBuilder.Create()  // создаем триггер
                    .WithIdentity("triggerCloseCN", "Close")     // идентифицируем триггер с именем и группой
                    .StartNow()                            // запуск сразу после начала выполнения
                    .WithSchedule(CronScheduleBuilder            // настраиваем выполнение действия
                        .AtHourAndMinuteOnGivenDaysOfWeek(7, 30,//время по UTC
                        new[] { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
                         .WithMisfireHandlingInstructionDoNothing())
                    .Build();                               // создаем триггер


                await scheduler.ScheduleJob(job2, trigger2);
                await scheduler.ScheduleJob(job1, trigger1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в планировщике сбера: {ex}");
            }
        }

        public static async Task ShutDown()
        {
            try
            {
                await scheduler.Shutdown();
                Console.WriteLine("Планировщик завершил работу");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка в завершении работы планировщика");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
