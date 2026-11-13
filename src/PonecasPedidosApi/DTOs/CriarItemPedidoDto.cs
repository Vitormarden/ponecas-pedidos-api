using System.ComponentModel.DataAnnotations;

namespace PonecasPedidosApi.DTOs;

public record CriarItemPedidoDto(
    [Required][StringLength(200, MinimumLength = 1)] string NomeProduto,
    [Range(1, 10000)] int Quantidade,
    [Range(0.01, 999999.99)] decimal PrecoUnitario
);
