import * as React from "react";
import Typography from "@mui/material/Typography";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemText from "@mui/material/ListItemText";
import Grid from "@mui/material/Grid";
import CheckoutDTO from "../../Objects/Checkout";
import BasketItem from "../../Objects/BasketItem";

function OrderReview({
  checkout,
  productsAmounts,
}: {
  checkout: CheckoutDTO;
  productsAmounts: BasketItem[];
}) {
  const payments = [
    { name: "Card type", detail: "Visa" },
    { name: "Card holder", detail: checkout.cardHolder },
    { name: "Card number", detail: checkout.cardNumber },
    { name: "Expiry date", detail: checkout.month + "/" + checkout.year },
  ];
  return (
    <React.Fragment>
      <Typography variant="h6" gutterBottom>
        Order Summary
      </Typography>
      <List disablePadding>
        {Array.from(productsAmounts).map((cartProduct: BasketItem) => (
          <ListItem key={cartProduct.product.name} sx={{ py: 1, px: 0 }}>
            <ListItemText
              primary={`${cartProduct.product.name} ${
                cartProduct.quantity > 1 ? ` X ${cartProduct.quantity}` : ""
              }`}
            />
            <Typography variant="body2">{`${
              cartProduct.product.price * cartProduct.quantity
            } ₪`}</Typography>
          </ListItem>
        ))}
        <ListItem sx={{ py: 1, px: 0 }}>
          <ListItemText primary="Total" />
          <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
            {`${Array.from(productsAmounts).reduce(
              (sum, cartProduct) =>
                sum + cartProduct.priceAfterDiscount * cartProduct.quantity,
              0
            )} ₪`}
          </Typography>
        </ListItem>
      </List>
      <Grid container spacing={2}>
        <Grid item xs={12} sm={6}>
          <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
            Shipping
          </Typography>
          <Typography gutterBottom>{checkout.fullName}</Typography>
          <Typography gutterBottom>{checkout.address}</Typography>
        </Grid>
        <Grid item container direction="column" xs={12} sm={6}>
          <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
            Payment details
          </Typography>
          <Grid container>
            {payments.map((payment) => (
              <React.Fragment key={payment.name}>
                <Grid item xs={6}>
                  <Typography gutterBottom>{payment.name}</Typography>
                </Grid>
                <Grid item xs={6}>
                  <Typography gutterBottom>{payment.detail}</Typography>
                </Grid>
              </React.Fragment>
            ))}
          </Grid>
        </Grid>
      </Grid>
    </React.Fragment>
  );
}

export default OrderReview;
