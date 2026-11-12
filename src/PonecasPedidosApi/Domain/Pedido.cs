namespace PonecasPedidosApi.Domain;

public class Pedido
{
    public int Id { get; set; }                                           // PK gerada pelo banco
    public DateTime CriadoEm { get; set; }                               // UTC, set no service
    public string Status { get; set; } = "Pendente";                     // Pendente | Confirmado | Cancelado
    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
}
