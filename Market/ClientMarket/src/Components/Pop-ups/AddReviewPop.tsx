import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";

export default function AddReviewPop({
  handleAddReview,
  open,
  handleClose,
}: {
  handleAddReview: (rate: number, comment: string) => void;
  open: boolean;
  handleClose: () => void;
}) {
  const [rate, setRate] = React.useState<number>(0);
  const [comment, setComment] = React.useState<string>("");

  const resetFields = () => {
    setRate(0);
    setComment("");
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleSubmit = async () => {
    handleAddReview(rate, comment);
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
        <DialogTitle>Add Review</DialogTitle>
        <DialogContent>
          {userTextField("rate", "Rate ( 0-5 )", rate, "number", setRate)}
          {userTextField("comment", "Comment", comment, "text", setComment)}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleSubmit}>Submit</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
