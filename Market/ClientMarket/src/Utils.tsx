import Basket from "./Objects/Basket";
import BasketItem from "./Objects/BasketItem";
import Cart from "./Objects/Cart";
import Product from "./Objects/Product";

export let notificationFlag: boolean = true;
export const textColor = "black";
export const squaresColor = "#E8F0FE";
export const Currency = "₪";
export const serverPort = "https://localhost:7209";
export interface IDictionary<T> {
  [index: number]: T;
}
// TextField for example, expect to get this type of function on onChange Event:  (event: React.ChangeEvent<HTMLInputElement>) => void
// instead of making handler for each property that can be changed, this function generates the handlers (using the setState returned from React.useState())
export const makeSetStateFromEvent = (setState: any) => {
  return (event: React.ChangeEvent<HTMLInputElement>) => {
    setState(event.target.value);
  };
};

export const zip = <T, Z>(arr: T[], arr2: Z[]): [T, Z][] =>
  arr.map((a, i) => [a, arr2[i]]);
export const notificationsPort = 4560;
export const logsPort = 4560;

export default function checkInput(fields: any[]): boolean {
  for (const field of fields) {
    if (field === undefined) {
      return false;
    }
  }
  return true;
}

export const getAllCartProducts = (cart: Cart): BasketItem[] => {
  const allProducts: BasketItem[] = [];
  cart !== null
    ? cart.cart.map((basket: Basket) => {
        basket.productsAmount !== null
          ? basket.productsAmount.map((prod: BasketItem) => {
              allProducts.push(prod);
            })
          : null;
      })
    : null;
  return allProducts;
};

export const setNotificationFlag = (state: boolean) => {
  notificationFlag = state;
};
