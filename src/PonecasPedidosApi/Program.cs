using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using Serilog.Formatting.Compact;
using PonecasPedidosApi.Infrastructure;
using PonecasPedidosApi.Repositories;
using PonecasPedidosApi.Services;
using PonecasPedidosApi.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Serilog JSON structured logging
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console(new CompactJsonFormatter()));

// EF Core + SQLite
builder.Services.AddDbContext<PedidosDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository + Service (Scoped)
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

// Prometheus metrics singleton
builder.Services.AddSingleton<PedidoMetrics>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PedidosDbContext>();
    try
    {
        await db.Database.MigrateAsync();
        Log.Information("Migrations aplicadas com sucesso");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Falha ao aplicar migrations — aplicação encerrando");
        throw;
    }
}

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseHttpMetrics();
app.MapControllers();
app.MapMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public partial class Program { }
