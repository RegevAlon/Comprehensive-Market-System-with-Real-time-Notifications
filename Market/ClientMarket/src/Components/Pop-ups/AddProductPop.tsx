import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { Currency, makeSetStateFromEvent } from "../../Utils";
import { FormControlLabel, Checkbox } from "@mui/material";

export default function AddProductPop({
  handleAddProduct,
  open,
  handleClose,
  handleOpen,
}: {
  handleAddProduct: (
    productName: string,
    price: number,
    quantity: number,
    category: string,
    description: string,
    keyWords: string[],
    setBid: number
  ) => void;
  open: boolean;
  handleClose: () => void;
  handleOpen: () => void;
}) {
  const [name, setName] = React.useState("");
  const [category, setCategory] = React.useState("");
  const [price, setPrice] = React.useState(0);
  const [quantity, setQuantity] = React.useState(0);
  const [description, setDescription] = React.useState<string>("");
  const [keyWords, setKeywords] = React.useState<string>("");
  const [isBidPrice, setIsBidPrice] = React.useState<boolean>(false);

  const handleCheckboxChange = (event: any) => {
    setIsBidPrice(event.target.checked);
  };

  const resetFields = () => {
    setName("");
    setCategory("");
    setPrice(0);
    setQuantity(0);
    setDescription("");
    setKeywords("");
    setIsBidPrice(false);
  };

  const handleOpenClick = () => {
    resetFields();
    handleOpen();
  };

  const handleSubmit = async () => {
    const keyWordsList: string[] = keyWords.split(",");
    handleAddProduct(
      name,
      price,
      quantity,
      category,
      description,
      keyWordsList,
      isBidPrice ? 1 : 0
    );
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
      <Button variant="contained" sx={{ mt: 4 }} onClick={handleOpenClick}>
        Add New Product
      </Button>
      <Dialog open={open} onClose={handleClose} fullWidth>
        <DialogTitle>Add Product</DialogTitle>
        <DialogContent>
          {userTextField("productName", "Product Name", name, "text", setName)}
          {userTextField("price", "Price", price, "number", setPrice)}
          {userTextField(
            "quantity",
            "Quantity",
            quantity,
            "number",
            setQuantity
          )}
          {userTextField(
            "description",
            "Description",
            description,
            "text",
            setDescription
          )}
          {userTextField("category", "Category", category, "text", setCategory)}
          {userTextField(
            "keywords",
            "Key Words (split by comma)",
            keyWords,
            "text",
            setKeywords
          )}
          <FormControlLabel
            control={
              <Checkbox
                checked={isBidPrice}
                onChange={handleCheckboxChange}
                color="primary"
              />
            }
            label="Set Bid Price"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleSubmit}>Submit</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
