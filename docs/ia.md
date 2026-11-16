# Uso de IA no desenvolvimento

## Ferramentas utilizadas

- **Kiro**: geração da especificação inicial seguindo o fluxo *design-first* (`requirements.md`, `design.md` e `tasks.md`).
- **Agente de IA (Antigravity)**: continuação da implementação guiada pelo `tasks.md`, revisão dos critérios da rubrica, configuração do pipeline de CI, containerização e documentação técnica.

## Fluxo design-first

1. Definição dos requisitos funcionais e não funcionais (`requirements.md`).
2. Especificação da arquitetura em camadas, contratos HTTP, modelos e estratégia de testes (`design.md`).
3. Quebra em etapas incrementais com rastreabilidade aos requisitos (`tasks.md`).
4. Implementação por etapas com commits semânticos estruturados.

## Decisões arquiteturais apoiadas por IA

- **Repository Pattern** (`IPedidoRepository` / `PedidoRepository`) para isolar o EF Core da camada de serviço e viabilizar testes unitários com Moq (ver [ADR 0001](adr/0001-repository-pattern.md)).
- **Observabilidade**: logs estruturados em JSON com Serilog e métricas Prometheus (`pedidos_criados_total`) encapsuladas em `PedidoMetrics`.
- **Estratégia de testes alinhada à rubrica**: testes unitários focados em regras de negócio e validação de DTOs (xUnit + Moq), exatamente 1 teste de integração de fluxo completo (`POST` → `GET`) com `WebApplicationFactory` e SQLite in-memory, além de validação automática de cobertura mínima de 80% no CI.

## Revisão e validação humana

Todo código e infraestrutura gerados passaram por validação executável:

- `dotnet format --verify-no-changes` (lint) e `dotnet test` em configuração `Release`;
- validação da cobertura consolidada (unitários + integração) acima de 80%;
- build da imagem multi-stage e testes de endpoint e persistência em volume com Docker Compose.

Ajustes identificados na revisão — como a normalização de `CriadoEm` para `DateTimeKind.Utc`, a marcação `virtual` em `PedidoMetrics` para mock e a compatibilidade da solução com o SDK .NET 8 no CI — foram corrigidos antes da entrega final.
