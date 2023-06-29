import Appointment from "../Objects/Appointment";
import Cart from "../Objects/Cart";
import Checkout from "../Objects/Checkout";
import Member from "../Objects/Member";
import Product from "../Objects/Product";
import Purchase from "../Objects/Purchase";
import Shop from "../Objects/Shop";
import checkInput, { serverPort } from "../Utils";
import { fetchResponse } from "./GeneralService";
import ClientResponse from "./Response";

import {
  getSessionId,
  getUserName,
  setIsAdmin,
  setIsGuest,
  setSessionId,
  setUsername,
} from "./SessionService";

export async function serverLogin(
  username: string,
  password: string
): Promise<ClientResponse<string>> {
  const fields: any[] = [username, password];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/login";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      // body: '{\n  "userName": "string",\n  "password": "string"\n}',
      body: JSON.stringify({
        SessionId: getSessionId(),
        Username: username,
        Password: password,
      }),
    });

    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    setIsGuest(false);
    setUsername(username);
    fetchResponse(serverIsAdmin()).then((isAdmin: boolean) =>
      setIsAdmin(isAdmin)
    );

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function serverLogout(): Promise<ClientResponse<string>> {
  const uri = serverPort + "/api/market/logout";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      // body: '{\n  "userName": "string",\n  "password": "string"\n}',
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });

    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function serverIsAdmin(): Promise<ClientResponse<boolean>> {
  const uri = serverPort + "/api/market/is-admin";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      // body: '{\n  "userId": 0,\n  "targetId": 0\n}',
      body: JSON.stringify({
        sessionId: getSessionId(),
      }),
    });
    //const response = await jsonResponse.json();
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<boolean> = await jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function serverEnterAsGuest(): Promise<ClientResponse<string>> {
  const uri = serverPort + "/api/market/enter-as-guest";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionID: "1", // Pass the appropriate session ID value here
      }),
    });

    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      alert(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();

    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    //const response = JSON.parse(responseText);
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function serverAddProduct(
  shopId: number | undefined | null,
  productName: string | undefined | null,
  setBid: number | undefined | null,
  description: string | undefined | null,
  price: number | undefined | null,
  quantity: number | undefined | null,
  category: string | undefined | null,
  keyWords: string[] | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [
    shopId,
    productName,
    setBid,
    description,
    price,
    quantity,
    category,
    keyWords,
  ];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-product";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        SellType: setBid,
        ProductName: productName,
        Description: description,
        Price: price,
        Quantity: quantity,
        Category: category,
        KeyWords: keyWords,
      }),
    });

    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    //const response = jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function addReview(
  shopId: number | undefined | null,
  productID: number | undefined | null,
  rate: number | undefined | null,
  comment: string | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, productID, rate, comment];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-review";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        UserName: getUserName(),
        ProductID: productID,
        Rate: rate,
        Comment: comment,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function serverAddToCart(
  shopID: number | undefined | null,
  productID: number | undefined | null,
  quantity: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopID, productID, quantity];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-to-cart";

  if (!checkInput(fields)) return Promise.reject();
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopID: shopID,
        ProductID: productID,
        Quantity: quantity,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function appoint(
  appointeeUserName: string | undefined | null,
  shopID: number | undefined | null,
  role: number | undefined | null,
  permission: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [appointeeUserName, shopID, role, permission];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/appoint";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        AppointeeUserName: appointeeUserName,
        ShopID: shopID,
        Role: role,
        Permission: permission,
      }),
    });

    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function RemoveAppointee(
  appointeeUserName: string | undefined | null,
  shopID: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [appointeeUserName, shopID];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/remove-appoint";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        AppointeeUserName: appointeeUserName,
        ShopID: shopID,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function ChangePermission(
  appointeeUserName: string | undefined | null,
  shopID: number | undefined | null,
  permission: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [appointeeUserName, shopID, permission];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/change-permission";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        AppointeeUserName: appointeeUserName,
        ShopID: shopID,
        Permission: permission,
      }),
    });
    //const response = await jsonResponse.json();
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function GetManagedShops(): Promise<Shop[]> {
  const uri = serverPort + "/api/market/get-user-shops";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("You are not a manager of any shop!");
    }
    const myShops: Shop[] = response.value.map((shop: any) => {
      return new Shop(
        shop.id,
        shop.name,
        shop.products,
        shop.isOpen,
        shop.purchasePolicies,
        shop.discountPolicies,
        shop.appointments,
        shop.rules,
        shop.purchases,
        shop.pendingAgreements,
        shop.rating
      );
    });
    return Promise.resolve(myShops);
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function CloseShop(
  appointeeUserName: string | undefined | null,
  shopID: number | undefined | null
): Promise<ClientResponse<boolean>> {
  const fields: any[] = [appointeeUserName, shopID];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/close-shop";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        AppointeeUserName: appointeeUserName,
        ShopID: shopID,
      }),
    });
    //const response = await jsonResponse.json();
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<boolean> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function CreateShop(
  shopName: string | undefined | null
): Promise<Shop> {
  const fields: any[] = [shopName];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/create-shop";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopName: shopName,
      }),
    });

    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    const shop = new Shop(
      response.value.id,
      response.value.name,
      response.value.products,
      response.value.isOpen,
      response.value.purchasePolicies,
      response.value.discountPolicies,
      response.value.appointments,
      response.value.rules,
      response.value.purchases,
      response.value.pendingAgreements,
      response.value.rating
    );

    return Promise.resolve(shop);
  } catch (e) {
    return Promise.reject(e);
  }
}
export async function OpenShop(
  appointeeUserName: string | undefined | null,
  shopId: number | undefined | null
): Promise<ClientResponse<boolean>> {
  const fields: any[] = [appointeeUserName, shopId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/open-shop";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        AppointeeUserName: appointeeUserName,
        ShopId: shopId,
      }),
    });
    //const response = await jsonResponse.json();
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<boolean> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}
export async function PurchaseBasket(
  appointeeUserName: string | undefined | null,
  shopId: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [appointeeUserName, shopId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/purchase-basket";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        AppointeeUserName: appointeeUserName,
        ShopId: shopId,
      }),
    });
    const response = await jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}
export async function serverPurchaseShoppingCart(
  checkout: Checkout | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [checkout];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/purchase-shopping-cart";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        CardNumber: checkout?.cardNumber,
        Month: checkout?.month,
        Year: checkout?.year,
        Holder: checkout?.cardHolder,
        Ccv: checkout?.ccv,
        Id: checkout?.id,
        Name: checkout?.fullName,
        Address: checkout?.address,
        City: checkout?.city,
        Country: checkout?.country,
        Zip: checkout?.zip,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function notificationOn(): Promise<ClientResponse<void>> {
  const uri = serverPort + "/api/market/notification-on";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    const response = await jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}
export async function notificationOff(): Promise<ClientResponse<void>> {
  const uri = serverPort + "/api/market/notification-off";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    const response = await jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function Register(
  username: string | undefined | null,
  password: string | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [username, password];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/register";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        Username: username,
        Password: password,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    //const response = JSON.parse(responseText);
    setSessionId(response.value);
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function removeProduct(
  shopId: number | undefined | null,
  productId: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, productId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/remove-product";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ProductId: productId,
      }),
    });

    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function removePolicy(
  shopId: number | undefined | null,
  policyId: number | undefined | null,
  type: string | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, policyId, type];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/remove-policy";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        PolicyId: policyId,
        Type: type,
      }),
    });

    //const response = jsonResponse.json();
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function removeFromCart(
  shopId: number | undefined | null,
  productId: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, productId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/remove-from-cart";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ProductId: productId,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function sendMessage(
  shopId: number | undefined | null,
  comment: string | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, comment];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/send-message";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        Comment: comment,
      }),
    });
    const response = jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function sendReport(
  shopId: number | undefined | null,
  comment: string | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, comment];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/send-report";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        Comment: comment,
      }),
    });
    const response = jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function updateProductName(
  shopId: number | undefined | null,
  productId: number | undefined | null,
  name: string | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, productId, name];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/update-product-name";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ProductId: productId,
        Name: name,
      }),
    });
    const response = jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function ServerUpdateProductPrice(
  shopId: number | undefined | null,
  productId: number | undefined | null,
  price: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, productId, price];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/update-product-price";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ProductId: productId,
        Price: price,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function updateProductQuantity(
  shopId: number | undefined | null,
  productId: number | undefined | null,
  quantity: number | undefined | null
): Promise<ClientResponse<boolean>> {
  const fields: any[] = [shopId, productId, quantity];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/update-product-quantity";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ProductId: productId,
        Quantity: quantity,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function cancelMembership(
  memberUserName: string | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [memberUserName];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/cancel-membership";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        MemberUserName: memberUserName,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<void> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response: ClientResponse<void> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function addCompositePolicy(
  shopId: number | undefined | null = null,
  expirationDate: string | undefined | null = null,
  subject: string | undefined | null = null,
  op: number | undefined | null = null,
  policies: number[] | undefined | null = null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, expirationDate, subject, op, policies];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-composit-policy";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        shopID: shopId,
        ExpirationDate: expirationDate,
        Subject: subject,
        Op: op,
        Policies: policies,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function addDiscountPolicy(
  shopId: number | undefined | null,
  expirationDate: string | undefined | null,
  subject: string | undefined | null,
  ruleId: number | undefined | null,
  percentage: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, expirationDate, subject, ruleId, percentage];
  if (!checkInput(fields)) return Promise.reject();
  console.log(ruleId);
  const uri = serverPort + "/api/market/add-discount-policy";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ExpirationDate: expirationDate,
        Subject: subject,
        RuleId: ruleId,
        Percentage: percentage,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function addPurchasePolicy(
  shopId: number | undefined | null,
  expirationDate: string | undefined | null,
  subject: string | undefined | null,
  ruleId: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, expirationDate, ruleId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-purchase-policy";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ExpirationDate: expirationDate,
        Subject: subject,
        RuleId: ruleId,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function addQuntityRule(
  shopId: number | undefined | null,
  subject: string | undefined | null,
  minQuantity: number | undefined | null,
  maxQuantity: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, subject, minQuantity, maxQuantity];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-quantity-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        Subject: subject,
        MinQuantity: minQuantity,
        MaxQuantity: maxQuantity,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function addSimpleRule(
  shopId: number | undefined | null,
  subject: string | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, subject];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-simple-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        Subject: subject,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}
export async function addTotalPriceRule(
  shopId: number | undefined | null,
  subject: string | undefined | null,
  targetPrice: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, subject, targetPrice];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-total-price-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        Subject: subject,
        TargetPrice: targetPrice,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function updateCompositeRule(
  shopId: number | undefined | null,
  ruleId: number | undefined | null,
  rules: number[] | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, ruleId, rules];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/update-composit-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        RuleId: ruleId,
        Rules: rules,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function updateQuntityRule(
  shopId: number | undefined | null,
  ruleId: number | undefined | null,
  minQuantity: number | undefined | null,
  maxQuantity: number | undefined | null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, ruleId, minQuantity, maxQuantity];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/update-quantity-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        RuleId: ruleId,
        MinQuantity: minQuantity,
        MaxQuantity: maxQuantity,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function updateRuleSubject(
  shopId: number | undefined | null,
  ruleId: number | undefined | null,
  subject: string | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, ruleId, subject];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/update-rule-subject";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        RuleId: ruleId,
        Subject: subject,
      }),
    });
    const response = jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}
export async function updateCompositeOperator(
  shopId: number | undefined | null,
  ruleId: number | undefined | null,
  operator: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, ruleId, operator];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/update-composite-operator";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "http",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        RuleId: ruleId,
        Operator: operator,
      }),
    });
    const response = jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function getMarketInfo(): Promise<Shop[]> {
  const uri = serverPort + "/api/market/get-market-info";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("You are not a manager of any shop!");
    }
    const myShops: Shop[] = response.value.map((shop: any) => {
      return new Shop(
        shop.id,
        shop.name,
        shop.products,
        shop.isOpen,
        shop.purchasePolicies,
        shop.discountPolicies,
        shop.appointments,
        shop.rules,
        shop.purchases,
        shop.pendingAgreements,
        shop.rating
      );
    });
    return Promise.resolve(myShops);
  } catch (e) {
    return Promise.reject("error fetching market shops" + e);
  }
}

export async function serverGetShopInfo(
  shopId: number | undefined | null
): Promise<Shop> {
  const fields: any[] = [shopId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/get-shop-info";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    const products: Product[] = [];
    response.value.products.forEach((product: any) => {
      products.push(
        new Product(
          product.id,
          product.name,
          product.description,
          product.price,
          product.quantity,
          product.category,
          product.shopId,
          product.keywords,
          product.reviews,
          product.rate,
          product.bids,
          product.sellType
        )
      );
    });
    const shop: Shop = new Shop(
      response.value.id,
      response.value.name,
      products,
      response.value.isOpen,
      response.value.purchasePolicies,
      response.value.discountPolicies,
      response.value.appointments,
      response.value.rules,
      response.value.purchases,
      response.value.pendingAgreements,
      response.value.rating
    );
    return Promise.resolve(shop);
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function GetShoppingCartInfo(): Promise<Cart> {
  const uri = serverPort + "/api/market/get-shopping-cart-info";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    const cart: Cart = new Cart(response.value.totalPrice, response.value.cart);
    return Promise.resolve(cart);
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function getActiveMembers(): Promise<ClientResponse<string[]>> {
  const uri = serverPort + "/api/market/get-active-members";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function getAllMembers(): Promise<ClientResponse<string[]>> {
  const uri = serverPort + "/api/market/get-all-members";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function getShopPositions(
  shopId: number | undefined | null
): Promise<ClientResponse<Appointment[]>> {
  const fields: any[] = [shopId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/get-shop-positions";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
      }),
    });
    const response = await jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function serverSearchProducts(
  searchMsg: string | null = null,
  searchType: number[] | null = null,
  filterSearch: number[] | null = null,
  lowPrice: number | null = null,
  highPrice: number | null = null,
  lowRate: number | null = null,
  highRate: number | null = null,
  category: string | null = null
): Promise<Product[]> {
  const uri = serverPort + "/api/market/search";
  const fields: any[] = [
    searchMsg,
    searchType,
    filterSearch,
    lowPrice,
    highPrice,
    lowRate,
    highRate,
    category,
  ];
  if (!checkInput(fields)) return Promise.reject();
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        Word: searchMsg,
        SearchType: searchType,
        FilterType: filterSearch,
        LowPrice: lowPrice,
        HighPrice: highPrice,
        LowRate: lowRate,
        HighRate: highRate,
        Category: category,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    const searchedProducts: Product[] = response.value.map((product: any) => {
      return new Product(
        product.id,
        product.name,
        product.description,
        product.price,
        product.quantity,
        product.category,
        product.shopId,
        product.keywords,
        product.reviews,
        product.rate,
        product.bids,
        product.sellType
      );
    });
    return Promise.resolve(searchedProducts);
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function showMemberPurchaseHistory(): Promise<
  ClientResponse<Purchase[]>
> {
  const uri = serverPort + "/api/market/show-member-purchase-history";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    const response = await jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function showShopPurchaseHistory(
  shopId: number | undefined | null
): Promise<Purchase[]> {
  const fields: any[] = [shopId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/show-shop-purchase-history";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
      }),
    });
    const response = await jsonResponse.json();
    return response.value;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function getShopByName(
  name: number | undefined | null
): Promise<ClientResponse<Appointment[]>> {
  const fields: any[] = [name];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/get-shop-by-name";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        Name: name,
      }),
    });
    const response = await jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function getUsersShop(): Promise<ClientResponse<Shop[]>> {
  const uri = serverPort + "/api/market/get-user-shops";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    const response = await jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}
export async function getMessages(): Promise<ClientResponse<string[]>> {
  const uri = serverPort + "/api/market/get-messages";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function getMessagesNumber(): Promise<number> {
  const uri = serverPort + "/api/market/get-messages-number";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response.value;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function getProductNumber(): Promise<number> {
  const uri = serverPort + "/api/market/get-shopping-cart-amount";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response.value;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function AddSimpleRule(
  shopId: number | null = null,
  subject: string | null = null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, subject];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-simple-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        Subject: subject,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function AddCompositeRule(
  shopId: number | null = null,
  op: number | null = null,
  rules: number[] | null = null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, op, rules];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-composit-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        shopId: shopId,
        op: op,
        rules: rules,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function AddTotalPriceRule(
  shopId: number | null = null,
  subject: string | null = null,
  targetPrice: number | null = null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, subject, targetPrice];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-total-price-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        Subject: subject,
        TargetPrice: targetPrice,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function AddQuantityRule(
  shopId: number | null = null,
  subject: string | null = null,
  minQuantity: number | null = null,
  maxQuantity: number | null = null
): Promise<ClientResponse<string>> {
  const fields: any[] = [shopId, subject, minQuantity, maxQuantity];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/add-quantity-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        Subject: subject,
        MinQuantity: minQuantity,
        MaxQuantity: maxQuantity,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }

    const response: ClientResponse<string> = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }

    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function RemoveRule(
  shopId: number | null = null,
  ruleId: number | null = null
): Promise<ClientResponse<boolean>> {
  const fields: any[] = [shopId, ruleId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/remove-rule";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        shopId: shopId,
        ruleId: ruleId,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function isAdmin(): Promise<ClientResponse<boolean>> {
  const uri = serverPort + "/api/market/is-admin";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "text/plain",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
      }),
    });
    const response = await jsonResponse.json();
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function updateBasketItemQuantity(
  shopId: number | undefined | null,
  productId: number | undefined | null,
  newQuantity: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, productId, newQuantity];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/update-basket-quantity";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ProductId: productId,
        Quantity: newQuantity,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function setBidOnProduct(
  shopId: number | undefined | null,
  productId: number | undefined | null,
  quantity: number | undefined | null,
  suggestedPriceForOne: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, productId, quantity, suggestedPriceForOne];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/set-product-bid";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ProductId: productId,
        Quantity: quantity,
        SuggestedPriceForOne: suggestedPriceForOne,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function approveBid(
  shopId: number | undefined | null,
  bidUsername: string | undefined | null,
  productId: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, bidUsername, productId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/approve-bid";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        BidUsername: bidUsername,
        ProductId: productId,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function declineBid(
  shopId: number | undefined | null,
  bidUsername: string | undefined | null,
  productId: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, bidUsername, productId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/decline-bid";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        BidUsername: bidUsername,
        ProductId: productId,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function OfferCounterBid(
  shopId: number | undefined | null,
  bidUsername: string | undefined | null,
  productId: number | undefined | null,
  counterPrice: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, bidUsername, productId, counterPrice];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/offer-counter-bid";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        BidUsername: bidUsername,
        ProductId: productId,
        CounterPrice: counterPrice,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function RemoveBid(
  shopId: number | undefined | null,
  bidUsername: string | undefined | null,
  productId: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, bidUsername, productId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/remove-bid";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        BidUsername: bidUsername,
        ProductId: productId,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function ApproveCounterBid(
  shopId: number | undefined | null,
  productId: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, productId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/approve-counter-bid";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ProductId: productId,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function ApproveAppointment(
  shopId: number | undefined | null,
  appointeeUsername: string | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, appointeeUsername];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/approve-appointment";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        AppointeeUsername: appointeeUsername,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function DeclineAppointment(
  shopId: number | undefined | null,
  appointeeUsername: string | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, appointeeUsername];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/decline-appointment";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        AppointeeUsername: appointeeUsername,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}

export async function DeclineCounterBid(
  shopId: number | undefined | null,
  productId: number | undefined | null
): Promise<ClientResponse<void>> {
  const fields: any[] = [shopId, productId];
  if (!checkInput(fields)) return Promise.reject();
  const uri = serverPort + "/api/market/decline-counter-bid";
  try {
    const jsonResponse = await fetch(uri, {
      method: "POST",
      headers: {
        accept: "application/json",
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
      },
      body: JSON.stringify({
        SessionId: getSessionId(),
        ShopId: shopId,
        ProductId: productId,
      }),
    });
    if (!jsonResponse.ok) {
      const errorResponse: ClientResponse<string> = await jsonResponse.json();
      throw new Error(errorResponse.errorMessage);
    }
    const response = await jsonResponse.json();
    // Handle empty response
    if (!response) {
      throw new Error("Empty response received");
    }
    return response;
  } catch (e) {
    return Promise.reject(e);
  }
}
