
namespace MarketRobot.Telegram
{
    internal interface ITelegramBot
    {
        public void Start();
        public Task SendNotify(string mes);
    }
}
