using MarketRobot.Interface.Logger;
using Providers.Interface.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarketRobot.Interface.IMarketStrategy;
using static MarketRobot.Interface.Logger.IMyLogger;

namespace MarketRobot.Implementation.Logger
{
    internal class MyLogger : Interface.Logger.IMyLogger
    {
        private IKafkaProducerService _kafkaProducerService;

        public MyLogger(IKafkaProducerService kafkaProducerService)
        {
            _kafkaProducerService = kafkaProducerService;
        }

        public event NotifyTG Notify;

        public void LogWithTG(string message, string key)
        {
            Console.WriteLine(message);
            _kafkaProducerService.ProduceAsync("Strategy", key, message);
            //Notify?.Invoke(message);
        }

        public void LogConsole(string message)
        {
            Console.WriteLine(message);
        }
    }
}
