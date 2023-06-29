
import http from 'k6/http';
import { check } from 'k6';
/*
k6 run SimpleEnterAsGuest.js
*/  
export const options = {
    vus: 520, // Key for Smoke test. Keep it at 2, 3, max 5 VUs
    duration: '2s', // This can be shorter or just a few iterations
  };
export default function () {
  const enterurl = 'https://localhost:7209/api/market/enter-as-guest';

  const enter = {
    SessionID:"fsfs",
  };

  const headers = {
    'Content-Type': 'application/json',
  };

  const enterresponse = http.post(enterurl, JSON.stringify(enter), { headers: headers });
  check(enterresponse, {
    'Status is 200': (res) => res.status === 200,
  });

  const responseBody = JSON.parse(enterresponse.body);
  const sessid = responseBody.value;

  console.log('Response:'+sessid, enterresponse.body);

  

}
