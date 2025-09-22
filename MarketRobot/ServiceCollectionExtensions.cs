using MarketRobot.Implementation;
using MarketRobot.Implementation.Logger;
using MarketRobot.Interface;
using MarketRobot.Interface.Logger;
using MarketRobot.Sheduler;
using MarketRobot.Sheduler.Jobs;
using MarketRobot.Sheduler.Jobs.CN;
using MarketRobot.Sheduler.Jobs.Sber;
using MarketRobot.Telegram;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketRobot
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMarketRobot(this IServiceCollection services)
        {
            string tSandboxAPIKey = Environment.GetEnvironmentVariable("TinkoffSanboxAPIKey");
            string tAPIKeySber = Environment.GetEnvironmentVariable("TinkoffAPIKeySber");
            string tAPIKeyCN = Environment.GetEnvironmentVariable("TinkoffAPIKeyCN");
            string telegramAPIKey = Environment.GetEnvironmentVariable("TelegramAPI");

            services.Configure<TelegramOptions>(options =>
            {
                options.TelegramKey = telegramAPIKey;
            });
            services.Configure<InvestAPIOptions>(options =>
            {
                options.InvestAPIKeySber = tAPIKeySber;
                options.InvestAPIKeyCN = tAPIKeyCN;
            });

            services.AddTransient<ITinkoffProvider, TinkoffProviderActive>();
            services.AddSingleton<IMarketSberStrategy, MarketSberStrategy>();
            services.AddSingleton<IMarketCNStrategy, MarketCNStrategy>();
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<Sheduler.Sheduler>();
            services.AddSingleton<ClosePositionsSberJob>();
            services.AddSingleton<ClosePositionsCNJob>();
            services.AddTransient<OpenPositionsSberJob>();
            services.AddTransient<OpenPositionsCNJob>();
            services.AddTransient<JobFactory>();
            services.AddInvestApiClient((s, settings) =>
            {
                settings.AccessToken = tSandboxAPIKey;
            });
            services.AddSingleton<ITelegramBot, TelegramBot>();
        }
    }
}
