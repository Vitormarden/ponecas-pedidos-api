import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 1,
  duration: '30s',
  thresholds: {
    http_req_duration: ['p(95)<300'], // SLO: p95 < 300ms
    http_req_failed: ['rate<0.01'],   // < 1% de falhas
  },
};

export default function () {
  const payload = JSON.stringify({
    itens: [
      { nomeProduto: 'Produto Teste', quantidade: 1, precoUnitario: 9.99 },
    ],
  });

  const params = { headers: { 'Content-Type': 'application/json' } };
  const res = http.post('http://localhost:8080/api/pedidos', payload, params);

  check(res, {
    'status is 201': (r) => r.status === 201,
    'has location header': (r) => r.headers['Location'] !== undefined,
    'response time < 300ms': (r) => r.timings.duration < 300,
  });

  sleep(1);
}

