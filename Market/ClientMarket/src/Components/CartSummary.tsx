import {
  Card,
  Box,
  Button,
  CardContent,
  Stack,
  Typography,
} from "@mui/material";
import { Currency } from "../Utils";
// import { CartProduct } from "../Pages/CartPage";
import ProductSummary from "./ProductSummary";
import Product from "../Objects/Product";
import BasketItem from "../Objects/BasketItem";
import { pathMarket } from "../Paths";

export default function CartSummary(
  totalBeforeDiscount: number,
  totalAfterDiscount: number,
  products: BasketItem[],
  handleRemoveProduct: (product: Product) => void,
  handlePurchase: () => void
) {
  return (
    <>
      <Box sx={{ width: 300, borderRadius: 2, boxShadow: 4 }}>
        <Typography sx={{ mb: 4, ml: 1 }} variant="h5">
          Total: {totalBeforeDiscount}
          {Currency}
        </Typography>
        <Typography variant="h5" sx={{ mb: 4, mt: 1.5, ml: 3 }}>
          After Discount: {totalAfterDiscount}
          {Currency}
        </Typography>
        <Button
          onClick={handlePurchase}
          sx={{ mb: 3, ml: 2 }}
          variant="contained"
          color="success"
        >
          Purchase
        </Button>
        <Button href={pathMarket} variant="contained" sx={{ mb: 3, ml: 2 }}>
          Back To Market
        </Button>
      </Box>
      {products.map((cartProduct: BasketItem) => {
        return ProductSummary(cartProduct, handleRemoveProduct);
      })}
      <Box textAlign="center"></Box>
    </>
  );
}
