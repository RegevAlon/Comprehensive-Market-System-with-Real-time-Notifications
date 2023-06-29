import { ThemeProvider } from "@emotion/react";
import {
  Box,
  Button,
  createTheme,
  Grid,
  Stack,
  Typography,
} from "@mui/material";
import * as React from "react";
import { useNavigate } from "react-router-dom";
import CartSummary from "../Components/CartSummary";
import ProductEditCart from "../Components/ProductEditCart";
import SuccessSnackbar from "../Components/SuccessSnackbar";
import Navbar from "../Components/Navbar";
import Product from "../Objects/Product";
import { pathCheckout, pathMarket } from "../Paths";
import { fetchResponse } from "../Services/GeneralService";
import {
  GetShoppingCartInfo,
  removeFromCart,
  updateBasketItemQuantity,
} from "../Services/MarketService";
import { getAllCartProducts } from "../Utils";
import BasketItem from "../Objects/BasketItem";
import DialogTwoOptions from "../Components/DialogTwoOptions";
import Cart from "../Objects/Cart";

const theme = createTheme({
  palette: {
    mode: "light",
  },
  typography: {
    fontFamily: [
      "-apple-system",
      "BlinkMacSystemFont",
      '"Segoe UI"',
      "Roboto",
      '"Helvetica Neue"',
      "Arial",
      "sans-serif",
      '"Apple Color Emoji"',
      '"Segoe UI Emoji"',
      '"Segoe UI Symbol"',
    ].join(","),
  },
});

function MakeProductEditCart(
  cartProduct: BasketItem,
  handleRemoveProductClick: (product: Product) => void,
  handleUpdateQuantity: (product: Product, newQuan: number) => void
) {
  return (
    <>
      {ProductEditCart(
        cartProduct,
        handleRemoveProductClick,
        handleUpdateQuantity
      )}
    </>
  );
}

export default function CartPage() {
  const navigate = useNavigate();
  const [openRemoveDialog, setOpenRemoveDialog] =
    React.useState<boolean>(false);
  const [renderProducts, setRenderProducts] = React.useState<boolean>(false);
  const [ProductToRemove, setProductToRemove] = React.useState<Product | null>(
    null
  );
  const [openRemoveProdSnackbar, setOpenRemoveProdSnackbar] =
    React.useState<boolean>(false);
  const [cartProducts, setCartProducts] = React.useState<BasketItem[]>([]);

  React.useEffect(() => {
    GetShoppingCartInfo()
      .then((cart: Cart) => {
        const allProducts: BasketItem[] = getAllCartProducts(cart);
        setCartProducts(allProducts);
      })
      .catch((e) => {
        alert(e);
      });
  }, [renderProducts]);

  const reloadCartProducts = () => setRenderProducts(!renderProducts);

  const calulateTotal = (cartProducts: BasketItem[]): number => {
    return cartProducts.reduce(
      (total: number, cartProduct: BasketItem) =>
        total + cartProduct.product.price * cartProduct.quantity,
      0
    );
  };

  const calulateTotalAfterDiscount = (cartProducts: BasketItem[]): number => {
    return cartProducts.reduce(
      (total: number, cartProduct: BasketItem) =>
        total + cartProduct.priceAfterDiscount * cartProduct.quantity,
      0
    );
  };

  const handleUpdateQuantity = (product: Product, newQuan: number) => {
    updateBasketItemQuantity(product.shopId, product.id, newQuan)
      .then(() => {
        reloadCartProducts();
      })
      .catch((e) => alert(e));
  };

  const tryRemoveProduct = (product: Product) => {
    fetchResponse(removeFromCart(product.shopId, product.id))
      .then(() => {
        setOpenRemoveProdSnackbar(true);
        reloadCartProducts();
      })
      .catch((e) => {
        alert(e);
      });
  };

  const handleCloseRemoveDialog = (remove: boolean, product: Product) => {
    if (remove) tryRemoveProduct(product);
    setOpenRemoveDialog(false);
  };

  const handleRemoveProductCanClick = (product: Product) => {
    setProductToRemove(product);
    setOpenRemoveDialog(true);
  };

  const handlePurchase = () => {
    navigate(pathCheckout);
  };

  return cartProducts === null || cartProducts.length === 0 ? (
    <ThemeProvider theme={theme}>
      <Navbar />
      <Box>
        <Typography variant="h4" sx={{ marginBottom: 3 }}>
          Your Shopping Cart is Empty
        </Typography>
      </Box>
      <Box textAlign="center">
        <Button href={pathMarket} variant="contained" sx={{ mt: 3, ml: 1 }}>
          Back To Market
        </Button>
      </Box>
    </ThemeProvider>
  ) : (
    <ThemeProvider theme={theme}>
      <Navbar />
      <Stack direction="row">
        <Box sx={{ width: "80%", ml: 3, mt: 2 }}>
          <Grid
            container
            flex={1}
            spacing={2}
            rowSpacing={1}
            columnSpacing={{ xs: 1, sm: 2, md: 3 }}
          >
            {cartProducts.length > 0
              ? cartProducts.map((cartProduct: BasketItem) =>
                  MakeProductEditCart(
                    cartProduct,
                    handleRemoveProductCanClick,
                    handleUpdateQuantity
                  )
                )
              : null}
          </Grid>
        </Box>
        <Box sx={{ width: "20%", mt: 2 }}>
          {CartSummary(
            calulateTotal(cartProducts),
            calulateTotalAfterDiscount(cartProducts),
            cartProducts,
            handleRemoveProductCanClick,
            handlePurchase
          )}
        </Box>
      </Stack>

      {ProductToRemove !== null
        ? DialogTwoOptions(
            ProductToRemove,
            openRemoveDialog,
            handleCloseRemoveDialog
          )
        : null}

      {SuccessSnackbar(
        "Removed " + ProductToRemove?.name + " Successfully",
        openRemoveProdSnackbar,
        () => setOpenRemoveProdSnackbar(false)
      )}
    </ThemeProvider>
  );
}
