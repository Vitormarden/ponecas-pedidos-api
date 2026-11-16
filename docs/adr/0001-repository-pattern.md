# ADR 0001: Adoção do Repository Pattern

## Status

Aceito

## Context

A API de pedidos precisa persistir e consultar `Pedido` e `ItemPedido` em SQLite via Entity Framework Core. Se o `PedidoService` usasse o `DbContext` diretamente, a lógica de negócio ficaria acoplada ao EF Core, dificultando testes unitários isolados e a troca futura da tecnologia de persistência.

## Decision

Adotar o Repository Pattern com a interface `IPedidoRepository` (`AdicionarAsync`, `ObterPorIdAsync`) e a implementação `PedidoRepository`, que recebe apenas `PedidosDbContext`. O `PedidoService` depende somente da interface, registrada via DI com escopo `Scoped`. O repositório não contém lógica de negócio, apenas acesso a dados (`Include` para eager loading dos itens).

## Consequences

- **Positivas:** o service é testável com `Mock<IPedidoRepository>` (Moq), sem banco; separação clara de responsabilidades; a troca do mecanismo de persistência afeta apenas o repositório.
- **Negativas:** camada adicional de abstração e mais código para um domínio pequeno; novas operações exigem alterar a interface e a implementação.

