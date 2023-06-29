// export enum RuleType {
//   Simple = "Simple",
//   Quantity = "Quantity",
//   TotalPrice = "TotalPrice",
//   Composite = "Composite",
// }
export enum RuleType {
  Simple = "0",
  Quantity = "1",
  TotalPrice = "2",
  Composite = "3",
}

export function getRuleTypeString(id: number) {
  if (id == 0) return "Simple";
  if (id == 1) return "Quantity";
  if (id == 2) return "TotalPrice";
  if (id == 3) return "Composite";
  return "NO RULE TYPE";
}

interface Rule {
  id: number;
  shopId: number;
  type: string;
}

export default Rule;
