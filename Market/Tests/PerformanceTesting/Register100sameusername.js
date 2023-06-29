
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
  
  const data = [];
  
  for (let i = 0; i < 100; i++) {
    const sessionID = generateRandomString(4);
    const username = generateRandomString(8).toLowerCase();
    const password = generateRandomString(10).toLowerCase();
  
    data.push({
      SessionID: sessionID,
      Username: username,
      Password: password,
    });
  }
  
  console.log(data);
  
export const options = {
    vus: 5, // Key for Smoke test. Keep it at 2, 3, max 5 VUs
    duration: '5s', // This can be shorter or just a few iterations
  };
export default function () {
  const url = 'https://localhost:7209/api/market/register';
  const payload = {
    SessionID:"1233",
    Username:"nadav123",
    Password: "password123",
  };

  const headers = {
    'Content-Type': 'application/json',
  };
data.forEach((item)=>{
    const response=http.post(url, JSON.stringify(payload), { headers })
    check(response, {
        'Status is 200': (res) => res.status === 200,
      });
    
      console.log('Response:', response.body);
})



}
