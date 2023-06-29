import Rule, { RuleType, getRuleTypeString } from "./Rule";
import LogicalOperator, {
  Operator,
  getOperatorString,
} from "../Operators/LogicalOperator";

class CompositeRule implements Rule {
  id: number;
  shopId: number;
  type: string;
  rules: Rule[];
  operator: string;

  constructor(id: number, shopId: number, rules: Rule[], operator: Operator) {
    this.id = id;
    this.shopId = shopId;
    this.type = getRuleTypeString(parseInt(RuleType.Composite));
    this.rules = rules;
    this.operator = getOperatorString(operator);
  }
}

export default CompositeRule;
