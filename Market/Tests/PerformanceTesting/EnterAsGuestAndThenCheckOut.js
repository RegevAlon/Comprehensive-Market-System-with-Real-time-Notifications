
import http from 'k6/http';
import { check } from 'k6';
/*
only 30 need to succes
k6 run EnterAsGuestAndThenCheckOut.js
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
    vus: 3, // Key for Smoke test. Keep it at 2, 3, max 5 VUs
    duration: '1s', // This can be shorter or just a few iterations
  };
export default function () {
  const enterurl = 'https://localhost:7209/api/market/enter-as-guest';
  const addtocarturl = 'https://localhost:7209/api/market/add-to-cart';
  const checkouturl = 'https://localhost:7209/api/market/purchase-shopping-cart';
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
  const addtocart = {
    SessionID: sessid,
    ShopID: 1,
    ProductID: 11,
    Quantity: 1,
  };
  const purchasecart = {
    SessionID: sessid,
    CardNumber: "32131",
    Month: "10",
    Year: "2020",
    Holder: "nadav",
    Ccv: "058",
    Id: "32132112",
    Name: "nadav",
    Address: "fsfsfs",
    City: "fsfsfs",
    Country: "fsfsfs",
    Zip: "fsfsfs",
  };
  
  const addresponse = http.post(addtocarturl, JSON.stringify(addtocart), { headers: headers });
  check(addresponse, {
    'Status is 200': (res) => res.status === 200,
  });
  console.log('Response:'+sessid, addresponse.body);
  const purchaseresponse = http.post(checkouturl, JSON.stringify(purchasecart), { headers: headers });
  check(purchaseresponse, {
    'Status is 200': (res) => res.status === 200,
  });
  console.log('Response:'+sessid, purchaseresponse.body);

}
