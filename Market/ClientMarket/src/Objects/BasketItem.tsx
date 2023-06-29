import Product from "./Product";

export default class BasketItem {
  product: Product;
  priceAfterDiscount: number;
  quantity: number;
  constructor(product: Product, quantity: number) {
    this.product = product;
    this.quantity = quantity;
    this.priceAfterDiscount = 0;
  }
}
