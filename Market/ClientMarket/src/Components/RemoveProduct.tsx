import { Fab, Icon, IconButton, Tooltip } from "@mui/material";
import Product from "../Objects/Product";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import { AddShoppingCart } from "@mui/icons-material";

export default function RemoveProduct(
  product: Product,
  handleRemoveProduct: (product: Product) => void
) {
  return (
    <div>
      <Tooltip title="Remove Product">
        <IconButton
          sx={{ display: "flex" }}
          onClick={() => handleRemoveProduct(product)}
        >
          <Icon sx={{ display: "flex" }}>
            <DeleteForeverIcon fontSize="medium" sx={{ color: "black" }} />
          </Icon>
        </IconButton>
      </Tooltip>
    </div>
  );
}
