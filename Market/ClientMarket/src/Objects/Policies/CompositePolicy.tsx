import Rule from "../Rules/Rule";
import { Policy } from "./Policy";

enum NumericOperator {
  Add,
  Max,
}

export default class CompositePolicy implements Policy {
  id: number;
  expirationDate: string;
  rule: Rule;
  description: string;
  precnetage: number;
  policies: Policy[];
  numericOperator: NumericOperator;
  constructor(
    id: number,
    expirationDate: string,
    rule: Rule,
    description: string,
    precnetage: number,
    policies: Policy[],
    numericOperator: NumericOperator
  ) {
    this.id = id;
    this.expirationDate = expirationDate;
    this.rule = rule;
    this.description = description;
    this.precnetage = precnetage;
    this.policies = policies;
    this.numericOperator = numericOperator;
  }
}
