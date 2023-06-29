import Rule, { RuleType, getRuleTypeString } from "./Rule";

class TotalPriceRule implements Rule {
  id: number;
  subject: string;
  shopId: number;
  type: string;
  totalPrice: number;

  constructor(id: number, subject: string, shopId: number, totalPrice: number) {
    this.id = id;
    this.subject = subject;
    this.shopId = shopId;
    this.type = getRuleTypeString(parseInt(RuleType.TotalPrice));
    this.totalPrice = totalPrice;
  }
}

export default TotalPriceRule;
