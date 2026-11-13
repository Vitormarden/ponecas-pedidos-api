using Microsoft.EntityFrameworkCore;
using PonecasPedidosApi.Domain;

namespace PonecasPedidosApi.Infrastructure;

public class PedidosDbContext : DbContext
{
    public PedidosDbContext(DbContextOptions<PedidosDbContext> options) : base(options) { }

    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Status)
                  .IsRequired()
                  .HasMaxLength(50);
            entity.Property(p => p.CriadoEm)
                  .IsRequired();
            entity.HasMany(p => p.Itens)
                  .WithOne(i => i.Pedido)
                  .HasForeignKey(i => i.PedidoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemPedido>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.NomeProduto)
                  .IsRequired()
                  .HasMaxLength(200);
            entity.Property(i => i.Quantidade)
                  .IsRequired();
            entity.Property(i => i.PrecoUnitario)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");
        });
    }
}
