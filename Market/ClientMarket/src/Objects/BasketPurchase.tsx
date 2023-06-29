import Basket from "./Basket";

class BasketPurchase {
  price: number;
  basket: Basket;
  purchaseStatus: string;

  constructor(price: number, basket: Basket, purchaseStatus: string) {
    this.price = price;
    this.basket = basket;
    this.purchaseStatus = purchaseStatus;
  }
}

export default BasketPurchase;
