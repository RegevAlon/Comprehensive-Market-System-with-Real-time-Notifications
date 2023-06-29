import Basket from "./Basket";

export default class Cart {
  totalPrice: number;
  cart: Basket[]; // shop id

  constructor(totalPrice: number, cart: Basket[]) {
    this.totalPrice = totalPrice;
    this.cart = cart;
  }
}

