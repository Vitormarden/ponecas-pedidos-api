using Microsoft.AspNetCore.Mvc;
using PonecasPedidosApi.DTOs;
using PonecasPedidosApi.Services;

namespace PonecasPedidosApi.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(IPedidoService service, ILogger<PedidosController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarPedido(
        [FromBody] CriarPedidoDto dto,
        CancellationToken ct = default)
    {
        try
        {
            var response = await _service.CriarPedidoAsync(dto, ct);
            return CreatedAtAction(nameof(ObterPedido), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ProblemDetails { Title = "Erro interno ao criar pedido" });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPedido(
        [FromRoute] int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
            return BadRequest(new ProblemDetails { Title = "Id deve ser maior que zero" });

        try
        {
            var response = await _service.ObterPedidoPorIdAsync(id, ct);
            if (response is null)
                return NotFound();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ProblemDetails { Title = "Erro interno ao obter pedido" });
        }
    }
}
