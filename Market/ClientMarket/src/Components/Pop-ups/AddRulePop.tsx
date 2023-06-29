import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";
import { RuleType, getRuleTypeString } from "../../Objects/Rules/Rule";
import { getOperatorString } from "../../Objects/Operators/LogicalOperator";

export default function AddRulePop({
  handleAddSimpleRule,
  handleAddQuantityRule,
  handleAddTotalPriceRule,
  handleAddCompositeRule,
  open,
  handleClose,
}: {
  handleAddSimpleRule: (subject: string) => void;
  handleAddQuantityRule: (
    subject: string,
    minQuantity: number,
    maxQuantity: number
  ) => void;
  handleAddTotalPriceRule: (subject: string, targetPrice: number) => void;
  handleAddCompositeRule: (operator: string, rules: string[]) => void;
  open: boolean;
  handleClose: () => void;
}) {
  const [subject, setSubject] = React.useState<string>("");
  const [ruleType, setRuleType] = React.useState<string>("");
  const [minQuantity, setMinQuantity] = React.useState<number>(0);
  const [maxQuantity, setMaxQuantity] = React.useState<number>(0);
  const [targetPrice, setTargetPrice] = React.useState<number>(0);
  const [operator, setOperator] = React.useState<string>("");
  const [rulesIds, setRulesIds] = React.useState<string>("");

  const rulesCount: number = 4;
  const operatorsCount: number = 3;

  const getRuleTypes = () => {
    let allRules: string = "Rule Type (";
    for (let i = 0; i < rulesCount; i++) {
      allRules += `${i}-` + getRuleTypeString(i) + " / ";
    }
    return allRules;
  };
  const getOperators = () => {
    let allOperators: string = "Operator (";
    for (let i = 0; i < operatorsCount; i++) {
      allOperators += `${i}-` + getOperatorString(`${i}`) + " / ";
    }
    return allOperators;
  };

  let ruleTypes: string = getRuleTypes();
  let operators: string = getOperators();
  ruleTypes = ruleTypes.substring(0, ruleTypes.length - 3);
  operators = operators.substring(0, operators.length - 3);
  const resetFields = () => {
    setSubject("");
    setRuleType("");
    setMinQuantity(0);
    setMaxQuantity(0);
    setTargetPrice(0);
    setOperator("");
    setRulesIds("");
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleSubmit = async () => {
    ruleType === RuleType.Simple ? handleAddSimpleRule(subject) : null;
    ruleType === RuleType.Quantity
      ? handleAddQuantityRule(subject, minQuantity, maxQuantity)
      : null;
    ruleType === RuleType.TotalPrice
      ? handleAddTotalPriceRule(subject, targetPrice)
      : null;
    ruleType === RuleType.Composite
      ? handleAddCompositeRule(operator, rulesIds.split(","))
      : null;
    handleCloseClick();
  };

  const makeTextField = (
    id: string,
    label: string,
    value: string | number,
    type: "text" | "number",
    setValue: any
  ) => {
    return (
      <>
        <TextField
          autoFocus
          margin="dense"
          id={id}
          label={label}
          type={type}
          value={value}
          onChange={makeSetStateFromEvent(setValue)}
          fullWidth
          variant="standard"
        />
      </>
    );
  };

  return (
    <div>
      <Dialog open={open} onClose={handleCloseClick} fullWidth>
        <DialogTitle>Add New Rule</DialogTitle>
        <DialogContent>
          {makeTextField(
            "subject",
            "Subject (Category / Product)",
            subject,
            "text",
            setSubject
          )}
          {makeTextField(
            "ruleType",
            `${ruleTypes})`,
            ruleType,
            "text",
            setRuleType
          )}
          {ruleType === RuleType.Quantity &&
            makeTextField(
              "minQuantity",
              "Minimum Quantity",
              minQuantity,
              "number",
              setMinQuantity
            )}
          {ruleType === RuleType.Quantity &&
            makeTextField(
              "maxQuantity",
              "Maximum Quantity",
              maxQuantity,
              "number",
              setMaxQuantity
            )}
          {ruleType === RuleType.TotalPrice &&
            makeTextField(
              "targetPrice",
              "Target Price",
              targetPrice,
              "number",
              setTargetPrice
            )}
          {ruleType === RuleType.Composite &&
            makeTextField(
              "operator",
              `${operators})`,
              operator,
              "text",
              setOperator
            )}
          {ruleType === RuleType.Composite &&
            makeTextField(
              "rulesIds",
              "Rules ID's (seperated by comma without space)",
              rulesIds,
              "text",
              setRulesIds
            )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleSubmit}>Submit</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
