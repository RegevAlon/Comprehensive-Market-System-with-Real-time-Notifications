import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";
import {
  getNumericOperatorString,
  getOperatorString,
} from "../../Objects/Operators/LogicalOperator";

export default function AddCompositeDiscountPolicyPop({
  handleAddCompositeDiscountPolicy,
  open,
  handleClose,
}: {
  handleAddCompositeDiscountPolicy: (
    expDate: string,
    subject: string,
    operator: number,
    policiesIds: number[]
  ) => void;
  open: boolean;
  handleClose: () => void;
}) {
  const [subject, setSubject] = React.useState<string>("");
  const [expDate, setExpDate] = React.useState<string>("");
  const [operator, setOperator] = React.useState<string>("");
  const [policiesIds, setPoliciesIds] = React.useState<string>("");

  const operatorsCount: number = 2;

  const getOperators = () => {
    let allOperators: string = "Operator (";
    for (let i = 0; i < operatorsCount; i++) {
      allOperators += `${i}-` + getNumericOperatorString(`${i}`) + " / ";
    }
    return allOperators;
  };

  let operators: string = getOperators();
  operators = operators.substring(0, operators.length - 3);

  const resetFields = () => {
    setSubject("");
    setExpDate("");
    setOperator("");
    setPoliciesIds("");
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleSubmit = async () => {
    const policyIds: number[] = [];
    policiesIds.split(",").forEach((id: string) => {
      policyIds.push(parseInt(id));
    });
    handleAddCompositeDiscountPolicy(
      expDate,
      subject,
      parseInt(operator),
      policyIds
    );
    handleCloseClick();
  };

  const makeTextField = (
    id: string,
    label: string,
    value: string | number | null,
    type: "date" | "number" | "string",
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
        <DialogTitle>Add New Composite Discount Policy</DialogTitle>
        <DialogContent>
          {makeTextField(
            "subject",
            "Suject (Category/Product)",
            subject,
            "string",
            setSubject
          )}
          {makeTextField("expDate", "", expDate, "date", setExpDate)}
          {makeTextField(
            "operator",
            `${operators})`,
            operator,
            "string",
            setOperator
          )}
          {makeTextField(
            "rulesIds",
            "Policies ID's (seperated by comma without space)",
            policiesIds,
            "string",
            setPoliciesIds
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
