using Wallet.DataAccess.Repositories;
using Wallet.DataAccess.Repositories.Mock;
using Wallet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddTransient<IPlayerRepository, PlayerMockRepository>();

builder.Services.AddTransient<IWalletService, WalletService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
