using MarketRobot.Interface.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarketRobot.Interface.IMarketStrategy;
using static MarketRobot.Interface.Logger.ILogger;

namespace MarketRobot.Implementation.Logger
{
    internal class Logger : ILogger
    {
        public event NotifyTG Notify;

        public void LogWithTG(string message)
        {
            Console.WriteLine(message);
            Notify?.Invoke(message);
        }

        public void LogConsole(string message)
        {
            Console.WriteLine(message);
        }
    }
}
