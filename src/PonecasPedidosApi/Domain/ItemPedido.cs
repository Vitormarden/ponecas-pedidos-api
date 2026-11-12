namespace PonecasPedidosApi.Domain;

public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }                                     // FK → Pedido.Id
    public string NomeProduto { get; set; } = string.Empty;              // obrigatório, max 200 chars
    public int Quantidade { get; set; }                                   // ≥ 1
    public decimal PrecoUnitario { get; set; }                           // ≥ 0.01
    public Pedido Pedido { get; set; } = null!;                          // nav property
}
