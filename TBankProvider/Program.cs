using MarketRobot.Implementation;
using MarketRobot.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ITinkoffProvider, TinkoffProviderActive>();

string tAPIKeySber = Environment.GetEnvironmentVariable("TinkoffAPIKeySber");
string tAPIKeyCN = Environment.GetEnvironmentVariable("TinkoffAPIKeyCN");
builder.Services.Configure<InvestAPIOptions>(options =>
{
    options.InvestAPIKeySber = tAPIKeySber;
    options.InvestAPIKeyCN = tAPIKeyCN;
});

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
