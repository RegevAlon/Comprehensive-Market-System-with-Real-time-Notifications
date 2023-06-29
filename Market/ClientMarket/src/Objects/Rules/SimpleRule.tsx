import Rule, { RuleType, getRuleTypeString } from "./Rule";

class SimpleRule implements Rule {
  id: number;
  subject: string;
  shopId: number;
  type: string;

  constructor(id: number, subject: string, shopId: number) {
    this.id = id;
    this.subject = subject;
    this.shopId = shopId;
    this.type = getRuleTypeString(parseInt(RuleType.Simple));
  }
}

export default SimpleRule;
