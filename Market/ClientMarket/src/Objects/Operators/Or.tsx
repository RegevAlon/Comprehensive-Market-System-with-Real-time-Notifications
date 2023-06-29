import LogicalOperator from "../Operators/LogicalOperator";
import Rule from "../Rules/Rule";

class Or implements LogicalOperator {
  operand1: Rule;
  operand2: Rule;
  constructor(operand1: Rule, operand2: Rule) {
    this.operand1 = operand1;
    this.operand2 = operand2;
  }
}

export default Or;
