import Bid from "./Bid";

export default class Product {
  id: number;
  name: string;
  description: string;
  price: number;
  quantity: number;
  category: string;
  shopId: number;
  keywords: string[];
  reviews: string[];
  rate: number;
  bids: Bid[];
  sellType: number; // regular(0) or bid(1)

  constructor(
    id: number,
    name: string,
    description: string,
    price: number,
    quantity: number,
    category: string,
    shop: number,
    keywords: string[],
    reviews: string[],
    rate: number,
    bids: Bid[],
    sellType: number
  ) {
    this.id = id;
    this.name = name;
    this.description = description;
    this.price = price;
    this.quantity = quantity;
    this.category = category;
    this.shopId = shop;
    this.keywords = keywords;
    this.reviews = reviews;
    this.rate = rate;
    this.bids = bids;
    this.sellType = sellType;
  }
}
