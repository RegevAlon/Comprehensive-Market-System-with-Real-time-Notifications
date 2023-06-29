import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";

export default function AddCounterBidPop({
  handleOfferCounterBid,
  open,
  handleClose,
}: {
  handleOfferCounterBid: (counterBid: number) => void;
  open: boolean;
  handleClose: () => void;
}) {
  const [counterBid, setCounterBid] = React.useState<number>(0);

  const resetFields = () => {
    setCounterBid(0);
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleSubmit = async () => {
    handleOfferCounterBid(counterBid);
  };

  const userTextField = (
    id: string,
    label: string,
    value: number,
    type: "number",
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
        <DialogTitle>Add Review</DialogTitle>
        <DialogContent>
          {userTextField(
            "counterBid",
            "Set Your Counter Bid",
            counterBid,
            "number",
            setCounterBid
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
