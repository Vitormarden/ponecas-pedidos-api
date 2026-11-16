# ponecas-pedidos-api

Web API RESTful para gerenciamento de pedidos construída em **ASP.NET Core (.NET 8)**, com persistência em SQLite via Entity Framework Core, logging estruturado com Serilog e métricas no formato Prometheus via prometheus-net.

---

## Pré-requisitos

| Ferramenta | Versão mínima |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | 8.0 |
| [Docker](https://www.docker.com/get-started) | 24.x |
| [k6](https://k6.io/docs/get-started/installation/) | 0.50.x |

---

## Build & Run

### Local (sem Docker)

```bash
# Restaurar dependências e compilar
dotnet restore
dotnet build -c Release

# Executar a API (aplica migrations automaticamente na inicialização)
dotnet run --project src/PonecasPedidosApi/PonecasPedidosApi.csproj
```

A API ficará disponível em `http://localhost:5000` (HTTP) e `https://localhost:5001` (HTTPS).

### Com Docker Compose

```bash
# Copiar e ajustar variáveis de ambiente
cp .env.example .env

# Subir a API
docker compose up --build
```

A API ficará disponível em `http://localhost:8080`.

---

## Testes

- **Testes unitários** (`tests/PonecasPedidosApi.UnitTests/`): regras de negócio de `PedidoService` (xUnit + Moq) e validação dos DTOs de entrada.
- **Teste de integração** (`tests/PonecasPedidosApi.IntegrationTests/`): 1 teste de fluxo completo (`POST /api/pedidos` → `GET /api/pedidos/{id}`) com `WebApplicationFactory` e SQLite in-memory.
- **Cobertura mínima declarada:** **80%** de cobertura de linhas (excluindo migrations geradas em `coverlet.runsettings`), validada automaticamente no pipeline de CI.

```bash
# Executar todos os testes (unitários + integração)
dotnet test -c Release

# Com relatório de cobertura (formato Cobertura XML) e validação da meta de 80%
dotnet test -c Release --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory ./coverage
python scripts/check-coverage.py ./coverage 80
```

### Smoke test de desempenho (k6)

```bash
k6 run k6/smoke.js
```

SLOs esperados: `p95 < 300ms`, taxa de erros `< 1%`.

---

## API

### POST /api/pedidos

Cria um novo pedido.

**Request:**
```http
POST /api/pedidos
Content-Type: application/json

{
  "itens": [
    {
      "nomeProduto": "Caneta Azul",
      "quantidade": 3,
      "precoUnitario": 2.50
    }
  ]
}
```

**Respostas:**

| Status | Descrição |
|---|---|
| `201 Created` | Pedido criado. Header `Location: /api/pedidos/{id}` |
| `400 Bad Request` | Payload inválido (`ValidationProblemDetails`) |
| `500 Internal Server Error` | Falha interna |

---

### GET /api/pedidos/{id}

Consulta um pedido existente pelo identificador.

**Respostas:**

| Status | Descrição |
|---|---|
| `200 OK` | Pedido encontrado |
| `400 Bad Request` | `id` inválido (≤ 0 ou não inteiro) |
| `404 Not Found` | Pedido não encontrado |
| `500 Internal Server Error` | Falha interna |

**Response body (200):**
```json
{
  "id": 1,
  "criadoEm": "2024-01-15T10:30:00Z",
  "status": "Pendente",
  "itens": [
    {
      "id": 1,
      "nomeProduto": "Caneta Azul",
      "quantidade": 3,
      "precoUnitario": 2.50
    }
  ]
}
```

---

## Docker

### Variáveis de ambiente

| Variável | Descrição | Exemplo |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execução | `Production` |
| `ConnectionStrings__DefaultConnection` | Connection string do SQLite | `Data Source=/data/pedidos.db` |

Copie `.env.example` para `.env` e ajuste os valores antes de subir o compose.

### Estrutura do compose

O `docker-compose.yml` expõe a porta `8080` e monta o volume `pedidos_data` em `/data` para persistência do banco SQLite entre reinicializações do container.

---

## CI (GitHub Actions)

O pipeline é definido em `.github/workflows/ci.yml` e é disparado em:
- Push para `main` ou `develop`
- Pull Requests com destino em `main`

Passos do pipeline:
1. Restaurar dependências NuGet
2. Verificar formatação (`dotnet format --verify-no-changes --no-restore`)
3. Build em Release
4. Executar todos os testes com coleta de cobertura (`coverlet.runsettings` → Cobertura XML)
5. Validar cobertura mínima de **80%** via `scripts/check-coverage.py`
6. Upload do relatório de cobertura como artifact `coverage-report`

---

## Estrutura do projeto

```
ponecas-pedidos-api/
├── src/PonecasPedidosApi/     # Projeto principal da API
├── tests/
│   ├── PonecasPedidosApi.UnitTests/        # Testes unitários (xUnit + Moq)
│   └── PonecasPedidosApi.IntegrationTests/ # Teste de integração (WebApplicationFactory)
├── docs/                      # ADR, OpenAPI spec, uso de IA e notas de desempenho
├── k6/                        # Script de smoke test
├── scripts/                   # Verificação de cobertura mínima
├── .github/workflows/         # Pipeline de CI
├── coverlet.runsettings
├── Dockerfile
├── docker-compose.yml
└── .env.example
```

---

## Observabilidade

- **Logs**: Serilog com saída JSON estruturada no console. Acesse via `docker logs`.
- **Métricas**: Endpoint `/metrics` em formato Prometheus (prometheus-net). Counter principal: `pedidos_criados_total`.
