using MarketRobot.Implementation;
using MarketRobot.Interface;
using MarketRobot.Sheduler;
using MarketRobot.Sheduler.Jobs;
using MarketRobot.Sheduler.Jobs.CN;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMarketCNYStrategy, MarketCNYStrategy>();
builder.Services.AddSingleton<ClosePositionsCNYJob>();
builder.Services.AddTransient<OpenPositionsCNYJob>();
builder.Services.AddTransient<JobFactory>();

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

Sheduler.StartCNY(app.Services);
