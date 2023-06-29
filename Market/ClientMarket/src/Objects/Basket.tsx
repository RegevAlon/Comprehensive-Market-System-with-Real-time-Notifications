import BasketItem from "./BasketItem";

export default class Basket {
  shopName: string;
  productsAmount: BasketItem[];
  totalPrice: number;

  constructor(
    shopName: string,
    productsAmount: BasketItem[],
    totalPrice: number
  ) {
    this.shopName = shopName;
    this.productsAmount = productsAmount;
    this.totalPrice = totalPrice;
  }
}
