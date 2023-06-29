
import http from 'k6/http';
import { check } from 'k6';
function generateRandomString(length) {
  let result = '';
  const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
  const charactersLength = characters.length;

  for (let i = 0; i < length; i++) {
    result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }

  return result;
}
  
 //need to be 50% means only enter as guest working
 //k6 run EnterAndTheRegisterWithSameUserName.js 
export const options = {
    vus: 5, // Key for Smoke test. Keep it at 2, 3, max 5 VUs
    duration: '5s', // This can be shorter or just a few iterations
  };
export default function () {
  const registerurl = 'https://localhost:7209/api/market/register';  
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

  console.log('Response enter:'+sessid, enterresponse.body);
  const password=generateRandomString(8).toLowerCase();
  const register = {
    SessionID:sessid,
    Username:"username",
    Password: password,
  };
  const regresponse = http.post(registerurl, JSON.stringify(register), { headers: headers });
  check(regresponse, {
    'Status is 200': (res) => res.status === 200,
  });
  console.log('Response register:'+sessid, regresponse.body);


}
