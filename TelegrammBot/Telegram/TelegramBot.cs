using MarketRobot.Interface;
using MarketRobot.Interface.Logger;
using Microsoft.Extensions.Options;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static Google.Rpc.Context.AttributeContext.Types;

namespace MarketRobot.Telegram
{
    internal class TelegramBot : ITelegramBot
    {
        private TelegramBotClient _client;
        private TelegramOptions _options;
        private IMarketSberStrategy _strategySber;
        private IMarketCNStrategy _strategyCN;
        private ILogger _logger;

        private long _userID = 1188835685;

        public TelegramBot(IMarketSberStrategy strategySber, IMarketCNStrategy strategyCN, ILogger logger, IOptions<TelegramOptions> options)
        {
            _client = new TelegramBotClient(options.Value.TelegramKey);
            _options = options.Value;
            _strategySber = strategySber;
            _strategyCN = strategyCN;
            _logger = logger;
        }

        public async void Start()
        {
            var me = await _client.GetMe();
            Console.WriteLine($"Бот {me.Username} запущен!");

            _client.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: new() { AllowedUpdates = Array.Empty<UpdateType>() }
            );

            _logger.Notify += async mes => await SendNotify(mes);

            await SendNotify("Бот запущен");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Обрабатываем только текстовые сообщения
            if (update.Message is not { } message)
                return;

            // Игнорируем сообщения, которые не являются текстом
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;
            _userID = message.Chat.Id;
            Console.WriteLine($"Получено сообщение: '{messageText}' от {chatId}");

            // Ответ бота
            string response = messageText.ToLower() switch
            {
                "/start" => "Привет! Я простой бот. Напиши что-нибудь, и я отвечу.",
                "/stoprobot" => await StopRobot(),
                "/check" => await HealthCheck(),
                _ => $"Ты написал: {messageText}"
            };

            // Отправляем ответ
            await botClient.SendMessage(
                chatId: chatId,
                text: response,
                cancellationToken: cancellationToken);
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }

        private async Task<string> StopRobot()
        {
            try
            {
                _strategySber.ClosePosition();
                _strategyCN.ClosePosition();
                await Sheduler.Sheduler.ShutDown();
                return "Робот завершил работу";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return "Произошла ошибка при завершении работы";
            }
        }

        public async Task SendNotify(string mes)
        {
            await _client.SendMessage(
                chatId: _userID,
                text: mes);
        }

        public async Task<string> HealthCheck()
        {
            return "Робот работает\n" +
                "Баланс Сбер: " + await _strategySber.GetBalance() + "\n" +
                "Баланс CNY: " + await _strategyCN.GetBalance() + "\n";
        }
    }
}
