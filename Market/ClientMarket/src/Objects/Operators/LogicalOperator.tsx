import Rule from "../Rules/Rule";

export enum Operator {
  Or = "0",
  Xor = "1",
  And = "2",
}

export function getOperatorString(name: string) {
  if (name == "0") return "Or";
  if (name == "1") return "Xor";
  if (name == "2") return "And";
  return "NO OPERATOR";
}

export function getNumericOperatorString(name: string) {
  if (name == "0") return "Add";
  if (name == "1") return "Max";
  return "NO OPERATOR";
}

interface LogicalOperator {
  operand1: Rule;
  operand2: Rule;
}

export default LogicalOperator;
