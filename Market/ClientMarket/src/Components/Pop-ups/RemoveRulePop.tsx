import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";

export default function RemoveRulePop({
  handleRemoveRule,
  open,
  handleClose,
}: {
  handleRemoveRule: (ruleId: number) => void;
  open: boolean;
  handleClose: () => void;
}) {
  const [ruleId, setRuleId] = React.useState<number | null>(null);

  const resetFields = () => {
    setRuleId(null);
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleRemove = async () => {
    ruleId !== null ? handleRemoveRule(ruleId) : null;
    handleCloseClick();
  };

  const makeTextField = (
    id: string,
    label: string,
    value: number | null,
    type: "number",
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
        <DialogTitle>Remove Rule</DialogTitle>
        <DialogContent>
          {makeTextField("ruleId", "Rule ID", ruleId, "number", setRuleId)}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleRemove}>Remove</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
