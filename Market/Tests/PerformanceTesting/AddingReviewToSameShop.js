
import http from 'k6/http';
import { check } from 'k6';
/*

k6 run AddingReviewToSameShop.js
*/ 
function generateRandomString(length) {
  let result = '';
  const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
  const charactersLength = characters.length;

  for (let i = 0; i < length; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }

  return result;
}
  
  
export const options = {
    vus: 1, // Key for Smoke test. Keep it at 2, 3, max 5 VUs
    duration: '1s', // This can be shorter or just a few iterations
  };
export default function () {
  const enterurl = 'https://localhost:7209/api/market/enter-as-guest';
  const registerurl = 'https://localhost:7209/api/market/register';
  const loginurl = 'https://localhost:7209/api/market/login';
  const addurl = 'https://localhost:7209/api/market/add-review';
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

  console.log('Response enter:'+sessid, enterresponse.body);
  const username=generateRandomString(8).toLowerCase();
  const password=generateRandomString(8).toLowerCase();
  const register = {
    SessionID:sessid,
    Username:username,
    Password: password,
  };
  const login = {
    SessionID:sessid,
    Username:username,
    Password: password,
  };
  const shop = {
    SessionID:sessid,
    ShopName: generateRandomString(6).toLowerCase(),
  };
  
  const regresponse = http.post(registerurl, JSON.stringify(register), { headers: headers });
  check(regresponse, {
    'Status is 200': (res) => res.status === 200,
  });
  console.log('Response register:'+sessid, regresponse.body);
  const logresponse = http.post(loginurl, JSON.stringify(login), { headers: headers });
  check(logresponse, {
    'Status is 200': (res) => res.status === 200,
  });
  console.log('Response login:'+sessid, logresponse.body);
  const add = {
    SessionID:sessid,
    ShopId: 1,
    Username:username,  
    ProductID: 11,
    Rate: 3.5,
    Comment : "fsfsfs",
  };
  const addresponse = http.post(addurl, JSON.stringify(add), { headers: headers });
  check(addresponse, {
    'Status is 200': (res) => res.status === 200,
  });
  console.log('Response addreview:', addresponse.body);
}
