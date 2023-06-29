import {
  Dialog,
  DialogTitle,
  DialogActions,
  Button,
  DialogContent,
} from "@mui/material";
import React from "react";
import Product from "../../Objects/Product";
import Shop from "../../Objects/Shop";
import { serverGetShopInfo } from "../../Services/MarketService";

export default function ShowReviewPop({
  open,
  prodId,
  shopId,
  handleClose,
}: {
  open: boolean;
  prodId: number;
  shopId: number | undefined;
  handleClose: () => void;
}) {
  const [reviews, setReviews] = React.useState<string[]>([]);

  React.useEffect(() => {
    serverGetShopInfo(shopId)
      .then((shop: Shop) => {
        shop.products.forEach((product: Product) => {
          if (product.id === prodId) {
            setReviews(product.reviews );
          }
        });
      })
      .catch((e) => {
        alert(e);
      });
  }, []);

  return (
    <div>
      <Dialog open={open} onClose={handleClose} fullWidth>
        <DialogTitle align="center">
          {reviews.length > 0
            ? "Product Reviews"
            : "No Reviews for This Product"}
        </DialogTitle>
        <DialogContent>
          {reviews.map((review, index) => (
            <div key={index}>{`${index + 1}- ${review}`}</div>
          ))}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
