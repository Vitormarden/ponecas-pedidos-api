using Microsoft.Extensions.Logging;
using Moq;
using PonecasPedidosApi.Domain;
using PonecasPedidosApi.DTOs;
using PonecasPedidosApi.Metrics;
using PonecasPedidosApi.Repositories;
using PonecasPedidosApi.Services;
using Xunit;

namespace PonecasPedidosApi.UnitTests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _repoMock = new();
    private readonly Mock<ILogger<PedidoService>> _loggerMock = new();
    private readonly Mock<PedidoMetrics> _metricsMock = new();
    private readonly PedidoService _sut;

    public PedidoServiceTests()
    {
        _sut = new PedidoService(_repoMock.Object, _loggerMock.Object, _metricsMock.Object);
    }

    private static CriarPedidoDto CreateValidDto() =>
        new(new List<CriarItemPedidoDto>
        {
            new("Caneta Azul", 3, 2.50m),
            new("Caderno Universitário", 1, 25.90m)
        });

    private void SetupAdicionarComId(int id) =>
        _repoMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pedido p, CancellationToken _) =>
            {
                p.Id = id;
                return p;
            });

    [Fact]
    public async Task CriarPedidoAsync_ValidDto_ReturnsDtoWithPositiveId()
    {
        SetupAdicionarComId(42);

        var result = await _sut.CriarPedidoAsync(CreateValidDto());

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(42, result.Id);
    }

    [Fact]
    public async Task CriarPedidoAsync_ValidDto_IncrementsMetric()
    {
        SetupAdicionarComId(1);

        await _sut.CriarPedidoAsync(CreateValidDto());

        _metricsMock.Verify(m => m.IncrementarPedidosCriados(), Times.Once);
    }

    [Fact]
    public async Task CriarPedidoAsync_RepositoryThrows_PropagatesException()
    {
        _repoMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Falha no banco de dados"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CriarPedidoAsync(CreateValidDto()));
        _metricsMock.Verify(m => m.IncrementarPedidosCriados(), Times.Never);
    }

    [Fact]
    public async Task ObterPedidoPorIdAsync_ExistingId_ReturnsDto()
    {
        var pedido = new Pedido
        {
            Id = 10,
            CriadoEm = DateTime.UtcNow,
            Status = "Pendente",
            Itens = new List<ItemPedido>
            {
                new()
                {
                    Id = 1,
                    PedidoId = 10,
                    NomeProduto = "Caneca Ponecas",
                    Quantidade = 2,
                    PrecoUnitario = 39.90m
                }
            }
        };
        _repoMock
            .Setup(r => r.ObterPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        var result = await _sut.ObterPedidoPorIdAsync(10);

        Assert.NotNull(result);
        Assert.Equal(10, result!.Id);
        Assert.Equal("Pendente", result.Status);
        var item = Assert.Single(result.Itens);
        Assert.Equal("Caneca Ponecas", item.NomeProduto);
        Assert.Equal(2, item.Quantidade);
        Assert.Equal(39.90m, item.PrecoUnitario);
    }

    [Fact]
    public async Task ObterPedidoPorIdAsync_NonExistingId_ReturnsNull()
    {
        _repoMock
            .Setup(r => r.ObterPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pedido?)null);

        var result = await _sut.ObterPedidoPorIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CriarPedidoAsync_ValidDto_StatusIsPendente()
    {
        SetupAdicionarComId(1);

        var result = await _sut.CriarPedidoAsync(CreateValidDto());

        Assert.Equal("Pendente", result.Status);
    }

    [Fact]
    public async Task CriarPedidoAsync_ValidDto_CallsAdicionarAsyncOnce()
    {
        SetupAdicionarComId(1);

        await _sut.CriarPedidoAsync(CreateValidDto());

        _repoMock.Verify(
            r => r.AdicionarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
