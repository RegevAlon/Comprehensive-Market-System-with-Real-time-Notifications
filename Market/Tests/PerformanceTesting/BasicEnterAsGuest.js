
import http from 'k6/http';
import { check } from 'k6';
export const options = {
    vus: 3, // Key for Smoke test. Keep it at 2, 3, max 5 VUs
    duration: '1m', // This can be shorter or just a few iterations
  };
export default function () {
  const url = 'https://localhost:7209/api/market/enter-as-guest';
  const payload = {
    SessionID:"123"
  };

  const headers = {
    'Content-Type': 'application/json',
  };

  const response = http.post(url, JSON.stringify(payload), { headers: headers });

  check(response, {
    'Status is 200': (res) => res.status === 200,
  });

  console.log('Response:', response.body);
}
