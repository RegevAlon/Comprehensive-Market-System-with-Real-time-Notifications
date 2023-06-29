import Rule from "../Rules/Rule";
import { Policy } from "./Policy";

export default class DiscountPolicy implements Policy {
  id: number;
  expirationDate: string;
  rule: Rule;
  description: string;
  precentage: number;
  constructor(
    id: number,
    expirationDate: string,
    rule: Rule,
    description: string,
    precentage: number
  ) {
    this.id = id;
    this.expirationDate = expirationDate;
    this.rule = rule;
    this.description = description;
    this.precentage = precentage;
  }
}
