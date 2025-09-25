
namespace MarketRobot.Telegram
{
    internal interface ITelegramBot
    {
        public void Start();
        public void SendNotify(string mes);
    }
}
