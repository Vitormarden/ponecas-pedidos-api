using System.ComponentModel.DataAnnotations;

namespace PonecasPedidosApi.DTOs;

public record CriarPedidoDto(
    [Required][MinLength(1)] List<CriarItemPedidoDto> Itens
);
