export default class DiscountPolicyInfo {
  id: number;
  expirationDate: string;
  description: string;
  constructor(id: number, expirationDate: string, description: string) {
    this.id = id;
    this.expirationDate = expirationDate;
    this.description = description;
  }
}
