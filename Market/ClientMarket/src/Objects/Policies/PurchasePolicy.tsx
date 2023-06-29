import Rule from "../Rules/Rule";
import { Policy } from "./Policy";

export default class PurchasePolicy implements Policy {
  id: number;
  expirationDate: string;
  rule: Rule;
  description: string;
  constructor(
    id: number,
    expirationDate: string,
    rule: Rule,
    description: string
  ) {
    this.id = id;
    this.expirationDate = expirationDate;
    this.rule = rule;
    this.description = description;
  }
}
