namespace PonecasPedidosApi.DTOs;

public record PedidoResponseDto(
    int Id,
    DateTime CriadoEm,
    string Status,
    IEnumerable<ItemPedidoResponseDto> Itens
);

public record ItemPedidoResponseDto(
    int Id,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario
);
