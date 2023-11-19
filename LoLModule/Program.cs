using LoLModule.Models;
using LoLModule.Repositories;
using LoLModule.Repositories.Impl;
using LoLModule.Services;
using LoLModule.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Injection dependencies
builder.Services.AddTransient<IMatchService, MatchService>();
builder.Services.AddTransient<IPlayerService, PlayerService>();
builder.Services.AddTransient<IMatchRepository,MatchRepository>();
builder.Services.AddTransient<ISummonerRepository,SummonerRepository>();

//Add MongoDB
builder.Services.Configure<LoLDatabaseSettings>( 
   builder.Configuration.GetSection("LolDatabase"));

builder.Services.Configure<LoLDatabaseSettings>(settings =>
    settings.connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ??
                                throw new MissingFieldException(
                                    "Missing Environment Variable for mongoDB connection string"));

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