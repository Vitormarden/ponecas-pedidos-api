using System.Net;
using System.Net.Http.Json;
using PonecasPedidosApi.DTOs;
using Xunit;

namespace PonecasPedidosApi.IntegrationTests;

public class PedidosEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PedidosEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static CriarPedidoDto PayloadValido() => new(
        new List<CriarItemPedidoDto>
        {
            new("Caneta Azul", 2, 1.50m)
        }
    );

    [Fact]
    public async Task PostPedido_PayloadValido_Retorna201ComLocationHeader()
    {
        var response = await _client.PostAsJsonAsync("/api/pedidos", PayloadValido());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/pedidos/", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task PostPedido_ItensVazios_Retorna400()
    {
        var payload = new CriarPedidoDto(new List<CriarItemPedidoDto>());
        var response = await _client.PostAsJsonAsync("/api/pedidos", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPedido_AposCriar_Retorna200ComDadosCorretos()
    {
        var postResponse = await _client.PostAsJsonAsync("/api/pedidos", PayloadValido());
        var created = await postResponse.Content.ReadFromJsonAsync<PedidoResponseDto>();

        var getResponse = await _client.GetAsync($"/api/pedidos/{created!.Id}");
        var pedido = await getResponse.Content.ReadFromJsonAsync<PedidoResponseDto>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(created.Id, pedido!.Id);
        Assert.Equal("Pendente", pedido.Status);
        Assert.Single(pedido.Itens);
        Assert.Equal("Caneta Azul", pedido.Itens.First().NomeProduto);
    }

    [Fact]
    public async Task GetPedido_IdInexistente_Retorna404()
    {
        var response = await _client.GetAsync("/api/pedidos/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostPedido_NomeProdutoVazio_Retorna400()
    {
        var payload = new CriarPedidoDto(new List<CriarItemPedidoDto> { new("", 1, 1.00m) });
        var response = await _client.PostAsJsonAsync("/api/pedidos", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostPedido_QuantidadeZero_Retorna400()
    {
        var payload = new CriarPedidoDto(new List<CriarItemPedidoDto> { new("Produto", 0, 1.00m) });
        var response = await _client.PostAsJsonAsync("/api/pedidos", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostPedido_PrecoNegativo_Retorna400()
    {
        var payload = new CriarPedidoDto(new List<CriarItemPedidoDto> { new("Produto", 1, -1.00m) });
        var response = await _client.PostAsJsonAsync("/api/pedidos", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
