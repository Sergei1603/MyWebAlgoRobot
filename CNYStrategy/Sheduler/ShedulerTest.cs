using Google.Type;
using MarketRobot.Sheduler.Jobs;
using MarketRobot.Sheduler.Jobs.CN;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using DayOfWeek = System.DayOfWeek;

namespace MarketRobot.Sheduler
{
    internal class ShedulerTest
    {
        //Время здесь по UTC, потому что контейнер по умолчанию работает по UTC
        static protected IScheduler scheduler;
        public static async void StartCNY(IServiceProvider serviceProvider)
        {
            try
            {
                scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                scheduler.JobFactory = serviceProvider.GetRequiredService<JobFactory>();
                await scheduler.Start();

                IJobDetail job1 = JobBuilder.Create<OpenPositionsCNYJob>().Build();

                ITrigger trigger1 = TriggerBuilder.Create()  // создаем триггер
              .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
              .StartNow()                            // запуск сразу после начала выполнения
              .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                  .WithIntervalInMinutes(5)          // через 1 минуту
                  .RepeatForever())                   // бесконечное повторение
              .Build();                               // создаем триггер

                IJobDetail job2 = JobBuilder.Create<ClosePositionsCNYJob>().Build();

                ITrigger trigger2 = TriggerBuilder.Create()  // создаем триггер
               .WithIdentity("trigger2", "group1")     // идентифицируем триггер с именем и группой
               .StartAt(DateTimeOffset.Now.AddMinutes(2))                            // запуск сразу после начала выполнения
                                                                                     //   .StartNow()
               .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                   .WithIntervalInMinutes(5)          // через 1 минуту
                   .RepeatForever())                   // бесконечное повторение
               .Build();                               // создаем триггер

                await scheduler.ScheduleJob(job2, trigger2);
                await scheduler.ScheduleJob(job1, trigger1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в планировщике валюты: {ex}");
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
