using Microsoft.EntityFrameworkCore;
using PonecasPedidosApi.Domain;
using PonecasPedidosApi.Infrastructure;

namespace PonecasPedidosApi.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly PedidosDbContext _context;

    public PedidoRepository(PedidosDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido> AdicionarAsync(Pedido pedido, CancellationToken ct = default)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync(ct);
        return pedido;
    }

    public async Task<Pedido?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}
