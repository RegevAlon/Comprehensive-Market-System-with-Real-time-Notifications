import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import { Stack } from "@mui/material";
import Product from "../Objects/Product";
import RemoveProduct from "./RemoveProduct";
import { Currency } from "../Utils";
import BasketItem from "../Objects/BasketItem";

export default function ProductSummary(
  cartProduct: BasketItem,
  handleRemoveProduct: (product: Product) => void
) {
  return (
    <Box sx={{ width: 300, borderRadius: 2, boxShadow: 4 }}>
      <Box sx={{ ml: 2, mb: 2 }}>
        <Typography sx={{ mt: 2, mb: 1.5 }} variant="h5">
          {cartProduct.product.name} x {cartProduct.quantity}
        </Typography>
        <Stack
          direction="row"
          sx={{ display: "flex", justifyContent: "space-between" }}
        >
          <Typography sx={{ mb: 1.5 }} variant="h6">
            Total: {cartProduct.priceAfterDiscount * cartProduct.quantity}
            {Currency}
          </Typography>
          {RemoveProduct(cartProduct.product, handleRemoveProduct)}
        </Stack>
      </Box>
    </Box>
  );
}
