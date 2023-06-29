import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Container from "@mui/material/Container";
import Paper from "@mui/material/Paper";
import Stepper from "@mui/material/Stepper";
import Step from "@mui/material/Step";
import StepLabel from "@mui/material/StepLabel";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import AddressForm from "../Components/PaymentCheckout/AddressForm";
import PaymentForm from "../Components/PaymentCheckout/PaymentForm";
import OrderReview from "../Components/PaymentCheckout/OrderReview";
import CheckoutDTO from "../Objects/Checkout";
import { pathCart, pathMarket } from "../Paths";
import { indigo } from "@mui/material/colors";
import { useNavigate } from "react-router-dom";
import LoadingCircle from "../Components/LoadingCircle";
import BasketItem from "../Objects/BasketItem";
import Cart from "../Objects/Cart";
import {
  GetShoppingCartInfo,
  serverPurchaseShoppingCart,
} from "../Services/MarketService";
import { getAllCartProducts } from "../Utils";

const steps = ["Shipping address", "Payment details", "Review your order"];

function getStepContent(
  step: number,
  checkout: CheckoutDTO,
  productsAmount: BasketItem[]
) {
  switch (step) {
    case 0:
      return <AddressForm checkout={checkout} />;
    case 1:
      return <PaymentForm checkout={checkout} />;
    case 2:
      return (
        <OrderReview checkout={checkout} productsAmounts={productsAmount} />
      );
    default:
      throw new Error("Unknown step");
  }
}

const theme = createTheme({
  palette: {
    primary: {
      main: indigo[500],
    },
    background: {
      default: "#fff",
    },
  },
});
const checkout = new CheckoutDTO();
export default function Checkout() {
  const navigate = useNavigate();
  const [cartProducts, setCartProducts] = React.useState<BasketItem[]>([]);
  const [stepCounter, setStepCounter] = React.useState(0);
  const [succeeded, setSucceeded] = React.useState<boolean>(false);
  const [errorMessage, setErrorMessage] = React.useState<string>("");
  const [orderPlaced, setOrderPlaced] = React.useState(false);
  const handleNext = () => {
    setStepCounter(stepCounter + 1);
  };

  React.useEffect(() => {
    GetShoppingCartInfo()
      .then((cart: Cart) => {
        const allProducts: BasketItem[] = getAllCartProducts(cart);
        setCartProducts(allProducts);
      })
      .catch((e) => {
        alert(e);
      });
  }, []);

  React.useEffect(() => {
    if (stepCounter === steps.length) placeOrder();
  }, [stepCounter]);

  const handleBack = () => {
    setStepCounter(stepCounter - 1);
  };

  const handleClickCancel = () => {
    navigate(`${pathCart}`);
  };

  function placeOrder() {
    serverPurchaseShoppingCart(checkout)
      .then(() => {
        setSucceeded(true);
        return true;
      })
      .then(setOrderPlaced)
      .catch((e: any) => {
        setErrorMessage(e.toString());
        setSucceeded(false);
        setOrderPlaced(true);
      });
  }
  function thankForOrder() {
    return (
      <React.Fragment>
        <Typography variant="h5" gutterBottom>
          Thank you for your order.
        </Typography>
        <Box textAlign="center">
          <Button href={pathMarket} variant="contained" sx={{ mt: 3, ml: 1 }}>
            Back To Home Page
          </Button>
        </Box>
      </React.Fragment>
    );
  }

  function orderError() {
    return (
      <>
        <Typography variant="h5" gutterBottom>
          Couldn't complete the order.
        </Typography>
        <Typography variant="subtitle1">{errorMessage}</Typography>
        <Box textAlign="center">
          <Button href={pathCart} variant="contained" sx={{ mt: 3, ml: 1 }}>
            Back To my cart
          </Button>
        </Box>
      </>
    );
  }

  function getCurrentStep(
    stepCounter: number,
    checkout: CheckoutDTO,
    productsAmount: BasketItem[]
  ) {
    return (
      <React.Fragment>
        {getStepContent(stepCounter, checkout, productsAmount)}
        <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
          {stepCounter === 0 ? (
            <Button onClick={handleClickCancel} sx={{ mt: 3, ml: 1 }}>
              Cancel
            </Button>
          ) : (
            <Button onClick={handleBack} sx={{ mt: 3, ml: 1 }}>
              Back
            </Button>
          )}
          <Button
            variant="contained"
            onClick={handleNext}
            sx={{ mt: 3, ml: 1 }}
          >
            {stepCounter === steps.length - 1 ? "Place order" : "Next"}
          </Button>
        </Box>
      </React.Fragment>
    );
  }

  return cartProducts == null ? (
    LoadingCircle()
  ) : (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Container component="main" maxWidth="sm" sx={{ mb: 4 }}>
        <Paper
          variant="outlined"
          sx={{ my: { xs: 3, md: 6 }, p: { xs: 2, md: 3 } }}
        >
          <Typography component="h1" variant="h4" align="center">
            Checkout
          </Typography>
          <Stepper activeStep={stepCounter} sx={{ pt: 3, pb: 5 }}>
            {steps.map((label) => (
              <Step key={label}>
                <StepLabel>{label}</StepLabel>
              </Step>
            ))}
          </Stepper>
          {orderPlaced
            ? succeeded
              ? thankForOrder()
              : orderError()
            : stepCounter === steps.length // If its last step
            ? LoadingCircle()
            : getCurrentStep(stepCounter, checkout, cartProducts)}
        </Paper>
      </Container>
    </ThemeProvider>
  );
}
