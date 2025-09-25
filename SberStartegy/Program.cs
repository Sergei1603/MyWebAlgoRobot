using Google.Api;
using MarketRobot;
using MarketRobot.Implementation;
using MarketRobot.Implementation.Logger;
using MarketRobot.Interface;
using MarketRobot.Interface.Logger;
using MarketRobot.Sheduler;
using MarketRobot.Sheduler.Jobs;
using MarketRobot.Sheduler.Jobs.Sber;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMarketRobot("", true);
builder.Services.AddSingleton<IMarketSberStrategy, MarketSberStrategy>();
builder.Services.AddSingleton<ClosePositionsSberJob>();
builder.Services.AddTransient<OpenPositionsSberJob>();
builder.Services.AddTransient<JobFactory>();
builder.Services.AddTransient<IMyLogger, MyLogger>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//Sheduler.StartSber(app.Services);
