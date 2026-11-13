using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PonecasPedidosApi.Infrastructure;

namespace PonecasPedidosApi.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Conexao mantida aberta para o banco in-memory sobreviver durante os testes
    private readonly SqliteConnection _connection;

    public CustomWebApplicationFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove o DbContext registrado no Program.cs
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<PedidosDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Registra DbContext usando a conexao persistente
            services.AddDbContext<PedidosDbContext>(options =>
                options.UseSqlite(_connection));

            // Cria o schema antes dos testes rodarem
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PedidosDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }
}
