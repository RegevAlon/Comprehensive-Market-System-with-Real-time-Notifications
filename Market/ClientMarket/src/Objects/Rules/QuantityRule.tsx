import Rule, { RuleType, getRuleTypeString } from "./Rule";

class QuantityRule implements Rule {
  id: number;
  subject: string;
  shopId: number;
  type: string;
  minQuantity: number;
  maxQuantity: number;

  constructor(
    id: number,
    subject: string,
    shopId: number,
    minQuantity: number,
    maxQuantity: number
  ) {
    this.id = id;
    this.subject = subject;
    this.shopId = shopId;
    this.type = getRuleTypeString(parseInt(RuleType.Quantity));
    this.minQuantity = minQuantity;
    this.maxQuantity = maxQuantity;
  }
}

export default QuantityRule;
