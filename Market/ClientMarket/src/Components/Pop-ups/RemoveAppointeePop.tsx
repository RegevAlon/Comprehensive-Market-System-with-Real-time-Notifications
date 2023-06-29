import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";

export default function RemoveAppointeePop({
  handleRemoveAppointee,
  open,
  shopId,
  handleClose,
}: {
  handleRemoveAppointee: (appointeeUsername: string, shopId: number) => void;
  open: boolean;
  shopId: number;
  handleClose: () => void;
}) {
  const [appointeeName, setName] = React.useState<string>("");

  const resetFields = () => {
    setName("");
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleRemove = async () => {
    handleRemoveAppointee(appointeeName, shopId);
    handleCloseClick();
  };

  const makeTextField = (
    id: string,
    label: string,
    value: string,
    type: "text",
    setValue: any
  ) => {
    return (
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
    );
  };

  return (
    <div>
      <Dialog open={open} onClose={handleCloseClick} fullWidth>
        <DialogTitle>Remove Appointee</DialogTitle>
        <DialogContent>
          {makeTextField(
            "appointeeName",
            "Appointee Name",
            appointeeName,
            "text",
            setName
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleRemove}>Remove Appointee</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
