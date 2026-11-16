# Desempenho

## Smoke test com k6

O script [`k6/smoke.js`](../k6/smoke.js) envia `POST /api/pedidos` com 1 usuário virtual (VU) por 30 segundos e valida, em cada resposta:

- status HTTP `201`;
- presença do header `Location`;
- tempo de resposta menor que 300 ms.

### Configuração

```javascript
export const options = {
  vus: 1,
  duration: '30s',
  thresholds: {
    http_req_duration: ['p(95)<300'],
    http_req_failed: ['rate<0.01'],
  },
};
```

## SLO

| Indicador | Meta |
|---|---|
| Latência p95 | < 300 ms |
| Taxa de erros | < 1% |

## Execução local

1. Instale o [k6](https://k6.io/docs/get-started/installation/).
2. Suba a API:

   ```bash
   docker-compose up --build
   ```

3. Em outro terminal, execute o teste:

   ```bash
   k6 run k6/smoke.js
   ```

4. O k6 termina com código diferente de zero se algum threshold (SLO) for violado.

