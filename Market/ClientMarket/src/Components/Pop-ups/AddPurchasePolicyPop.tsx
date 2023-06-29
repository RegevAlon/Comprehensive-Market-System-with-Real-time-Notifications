import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";

export default function AddPurchasePolicyPop({
  handleAddPurchasePolicy,
  open,
  handleClose,
}: {
  handleAddPurchasePolicy: (
    expDate: string,
    subject: string,
    ruleId: number
  ) => void;
  open: boolean;
  handleClose: () => void;
}) {
  const [subject, setSubject] = React.useState<string>("");
  const [expDate, setExpDate] = React.useState<string>("");
  const [ruleId, setRuleId] = React.useState<number | null>(null);

  const resetFields = () => {
    setSubject("");
    setExpDate("");
    setRuleId(null);
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleSubmit = async () => {
    ruleId !== null ? handleAddPurchasePolicy(expDate, subject, ruleId) : null;
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
        <DialogTitle>Add New Purchase Policy</DialogTitle>
        <DialogContent>
          {makeTextField(
            "ruldId",
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
