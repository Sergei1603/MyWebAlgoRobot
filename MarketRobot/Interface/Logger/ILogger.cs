using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketRobot.Interface.Logger
{
    internal interface ILogger
    {
        public event NotifyTG Notify;
        public delegate void NotifyTG(string message);

        public void LogWithTG(string message);
        public void LogConsole(string message);
    }
}
