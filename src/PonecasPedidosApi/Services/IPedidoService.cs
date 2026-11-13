using PonecasPedidosApi.DTOs;

namespace PonecasPedidosApi.Services;

public interface IPedidoService
{
    Task<PedidoResponseDto> CriarPedidoAsync(CriarPedidoDto dto, CancellationToken ct = default);
    Task<PedidoResponseDto?> ObterPedidoPorIdAsync(int id, CancellationToken ct = default);
}
