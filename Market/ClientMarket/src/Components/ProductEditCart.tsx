import {
  Card,
  Box,
  Button,
  CardContent,
  Paper,
  Stack,
  Typography,
  TextField,
  CardActions,
  Dialog,
  Tooltip,
} from "@mui/material";
import UpdateIcon from "@mui/icons-material/Update";
import Product from "../Objects/Product";
import { Currency, squaresColor } from "../Utils";
import RemoveProduct from "./RemoveProduct";
import { getIsGuest } from "../Services/SessionService";
import BasketItem from "../Objects/BasketItem";

function UpdateQuantityComponent(
  product: Product,
  handleUpdateQuantity: (product: Product, newQuan: number) => void
) {
  const handleQuantity = (event: React.FormEvent<HTMLFormElement>): void => {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    handleUpdateQuantity(product, Number(data.get("quantity")));
  };

  return (
    <Stack direction="column">
      <Box component="form" noValidate onSubmit={handleQuantity}>
        <Stack direction="row">
          <TextField
            id="quantity"
            name="quantity"
            type="number"
            InputLabelProps={{
              shrink: true,
            }}
            placeholder="Change Quantity"
            size="small"
          />
          <Tooltip title="Update Quantity">
            <Button
              color="primary"
              variant="contained"
              // color="inherit"
              size="small"
              sx={{ ml: 1 }}
              startIcon={<UpdateIcon fontSize="small" />}
              type="submit"
            >
              Update
            </Button>
          </Tooltip>
        </Stack>
      </Box>
    </Stack>
  );
}

function ProductContent(cartProduct: BasketItem) {
  return (
    <Box sx={{ backgroundColor: squaresColor }}>
      <Typography variant="h3" component="div">
        {cartProduct.product.name}
      </Typography>
      <Typography variant="h6">
        Price: {cartProduct.product.price}
        {Currency}
      </Typography>
      {cartProduct.priceAfterDiscount != cartProduct.product.price ? (
        <Typography variant="h6">
          After Discount: {cartProduct.priceAfterDiscount}
          {Currency}
        </Typography>
      ) : null}
      <Typography variant="h6">Quantity: {cartProduct.quantity}</Typography>
    </Box>
  );
}

export default function ProductEditCart(
  cartProduct: BasketItem,
  handleRemoveProductClick: (product: Product) => void,
  handleUpdateQuantity: (product: Product, newQuan: number) => void
) {
  return (
    <Card
      sx={{
        m: 2,
        boxShadow: 1,
        borderBlockColor: "black",
        borderRadius: 20,
        backgroundColor: squaresColor,
        p: 3,
      }}
      component={Paper}
    >
      <CardContent sx={{ display: "flex", flexDirection: "column" }}>
        {ProductContent(cartProduct)}
      </CardContent>
      <CardActions disableSpacing>
        {UpdateQuantityComponent(cartProduct.product, handleUpdateQuantity)}
        <Box sx={{ ml: "auto", mt: "auto" }}>
          {RemoveProduct(cartProduct.product, handleRemoveProductClick)}
        </Box>
      </CardActions>
    </Card>
  );
}
