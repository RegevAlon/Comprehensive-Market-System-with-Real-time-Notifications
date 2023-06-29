import { Currency } from "../Utils";

class BidInfo {
  id: number;
  prodId: number;
  username: string;
  productName: string;
  quantity: number;
  suggestedPrice: number;
  information: string;
  approved: boolean;
  declined: boolean;
  pending: boolean;
  constructor(
    id: number,
    prodId: number,
    username: string,
    productName: string,
    quantity: number,
    suggestedPrice: number,
    approved: boolean,
    declined: boolean,
    pending: boolean
  ) {
    this.id = id;
    this.prodId = prodId;
    this.username = username;
    this.productName = productName;
    this.quantity = quantity;
    this.suggestedPrice = suggestedPrice;
    this.information = `${this.username} wants to add a bid on ${this.productName}: ${this.quantity} ${this.productName}'s for ${this.suggestedPrice}${Currency} each`;
    this.approved = approved;
    this.declined = declined;
    this.pending = pending;
  }
}

export default BidInfo;
