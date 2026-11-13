using PonecasPedidosApi.Domain;
using PonecasPedidosApi.DTOs;
using PonecasPedidosApi.Metrics;
using PonecasPedidosApi.Repositories;

namespace PonecasPedidosApi.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _repository;
    private readonly ILogger<PedidoService> _logger;
    private readonly PedidoMetrics _metrics;

    public PedidoService(
        IPedidoRepository repository,
        ILogger<PedidoService> logger,
        PedidoMetrics metrics)
    {
        _repository = repository;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<PedidoResponseDto> CriarPedidoAsync(CriarPedidoDto dto, CancellationToken ct = default)
    {
        var pedido = new Pedido
        {
            CriadoEm = DateTime.UtcNow,
            Status = "Pendente",
            Itens = dto.Itens.Select(i => new ItemPedido
            {
                NomeProduto = i.NomeProduto,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario
            }).ToList()
        };

        var criado = await _repository.AdicionarAsync(pedido, ct);

        _metrics.IncrementarPedidosCriados();
        _logger.LogInformation("Pedido {PedidoId} criado com {QuantidadeItens} itens",
            criado.Id, criado.Itens.Count);

        return MapToResponseDto(criado);
    }

    public async Task<PedidoResponseDto?> ObterPedidoPorIdAsync(int id, CancellationToken ct = default)
    {
        var pedido = await _repository.ObterPorIdAsync(id, ct);
        return pedido is null ? null : MapToResponseDto(pedido);
    }

    private static PedidoResponseDto MapToResponseDto(Pedido pedido) =>
        new(
            pedido.Id,
            pedido.CriadoEm,
            pedido.Status,
            pedido.Itens.Select(i => new ItemPedidoResponseDto(
                i.Id,
                i.NomeProduto,
                i.Quantidade,
                i.PrecoUnitario
            ))
        );
}
