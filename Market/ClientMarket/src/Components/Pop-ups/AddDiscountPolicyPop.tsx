import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";

export default function AddDiscountPolicyPop({
  handleAddDiscountPolicy,
  open,
  handleClose,
}: {
  handleAddDiscountPolicy: (
    expDate: string,
    subject: string,
    ruleId: number,
    precentage: number
  ) => void;
  open: boolean;
  handleClose: () => void;
}) {
  const [subject, setSubject] = React.useState<string>("");
  const [expDate, setExpDate] = React.useState<string>("");
  const [ruleId, setRuleId] = React.useState<number | null>(null);
  const [precentage, setPrecentage] = React.useState<number | null>(null);

  const resetFields = () => {
    setSubject("");
    setExpDate("");
    setRuleId(null);
    setPrecentage(null);
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleSubmit = async () => {
    ruleId !== null && precentage !== null
      ? handleAddDiscountPolicy(expDate, subject, ruleId, precentage)
      : null;
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
        <DialogTitle>Add New Discount Policy</DialogTitle>
        <DialogContent>
          {makeTextField(
            "ruleId",
            "Enter Rule ID",
            ruleId,
            "number",
            setRuleId
          )}
          {makeTextField(
            "subject",
            "Suject (Category/Product)",
            subject,
            "string",
            setSubject
          )}
          {makeTextField(
            "precentage",
            "Precentage (%)",
            precentage,
            "number",
            setPrecentage
          )}
          {makeTextField("expDate", "", expDate, "date", setExpDate)}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleSubmit}>Submit</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
