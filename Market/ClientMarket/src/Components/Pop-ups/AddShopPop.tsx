import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";

export default function AddShopPop({
  handleAddShop,
  open,
  handleClose,
}: {
  handleAddShop: (shopName: string) => void;
  open: boolean;
  handleClose: () => void;
}) {
  const [name, setName] = React.useState("");

  const resetFields = () => {
    setName("");
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleCreate = async () => {
    handleAddShop(name);
  };

  const makeTextField = (
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
        <DialogTitle>Create New Shop</DialogTitle>
        <DialogContent>
          {makeTextField("shopName", "Shop name", name, "text", setName)}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleCreate}>Create</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
