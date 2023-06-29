import Rule from "../Rules/Rule";

export interface Policy {
  id: number;
  expirationDate: string;
  rule: Rule;
  description: string;
}
