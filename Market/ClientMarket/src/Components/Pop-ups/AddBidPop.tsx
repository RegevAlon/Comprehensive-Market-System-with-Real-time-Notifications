import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";

export default function AddBidPop({
  handleAddBid,
  open,
  currPrice,
  handleClose,
}: {
  handleAddBid: (quantity: number, bidPrice: number) => void;
  open: boolean;
  currPrice: number;
  handleClose: () => void;
}) {
  const [bid, setBid] = React.useState<number>(currPrice);
  const [quantity, setQuantity] = React.useState<number>(0);

  const resetFields = () => {
    setBid(currPrice);
    setQuantity(0);
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleSubmit = async () => {
    handleAddBid(quantity, bid);
  };

  const userTextField = (
    id: string,
    label: string,
    value: string | number,
    type: "text" | "number",
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
        <DialogTitle>Add Bid</DialogTitle>
        <DialogContent>
          {userTextField(
            "bid",
            "Enter your bid for each product",
            bid,
            "number",
            setBid
          )}
        </DialogContent>
        <DialogContent>
          {userTextField(
            "quantity",
            "Enter the product quantity of your bid",
            quantity,
            "number",
            setQuantity
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
