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

    [Fact]
    public async Task PostEGetPedido_FluxoCompleto_PersisteERetornaMesmosDados()
    {
        var payload = new CriarPedidoDto(new List<CriarItemPedidoDto>
        {
            new("Caneca Ponecas", 2, 39.90m)
        });

        var postResponse = await _client.PostAsJsonAsync("/api/pedidos", payload);
        var criado = await postResponse.Content.ReadFromJsonAsync<PedidoResponseDto>();

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(criado);
        Assert.True(criado!.Id > 0);
        Assert.EndsWith($"/api/pedidos/{criado.Id}", postResponse.Headers.Location!.ToString());

        var getResponse = await _client.GetAsync($"/api/pedidos/{criado.Id}");
        var lido = await getResponse.Content.ReadFromJsonAsync<PedidoResponseDto>();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(criado.Id, lido!.Id);
        Assert.Equal("Pendente", lido.Status);
        var item = Assert.Single(lido.Itens);
        Assert.Equal("Caneca Ponecas", item.NomeProduto);
        Assert.Equal(2, item.Quantidade);
        Assert.Equal(39.90m, item.PrecoUnitario);
    }
}
