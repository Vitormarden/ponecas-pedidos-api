using PonecasPedidosApi.Domain;

namespace PonecasPedidosApi.Repositories;

public interface IPedidoRepository
{
    Task<Pedido> AdicionarAsync(Pedido pedido, CancellationToken ct = default);
    Task<Pedido?> ObterPorIdAsync(int id, CancellationToken ct = default);
}
