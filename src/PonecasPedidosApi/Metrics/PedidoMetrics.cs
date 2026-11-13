namespace PonecasPedidosApi.Metrics;

public class PedidoMetrics
{
    private readonly Prometheus.Counter _pedidosCriadosTotal;

    public PedidoMetrics()
    {
        _pedidosCriadosTotal = Prometheus.Metrics.CreateCounter(
            "pedidos_criados_total",
            "Número total de pedidos criados com sucesso");
    }

    public void IncrementarPedidosCriados() => _pedidosCriadosTotal.Inc();
}
