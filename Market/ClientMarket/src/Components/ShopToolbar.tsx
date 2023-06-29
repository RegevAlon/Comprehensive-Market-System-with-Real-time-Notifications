import { alpha, Box, Fab, Toolbar, Tooltip, Typography } from "@mui/material";
import { AddShoppingCart } from "@mui/icons-material";

function ShopToolbar(numSelected: number, handleAddToCart: () => void) {
  return (
    <>
      <Toolbar
        sx={{
          pl: { sm: 2 },
          pr: { xs: 1, sm: 1 },
          ...(numSelected > 0 && {
            bgcolor: (theme) =>
              alpha(
                theme.palette.primary.main,
                theme.palette.action.activatedOpacity
              ),
          }),
        }}
      >
        {numSelected > 0 ? (
          <>
            <Typography
              sx={{ flex: "1 1 100%" }}
              color="inherit"
              variant="subtitle1"
              component="div"
            >
              Selected {numSelected} Products
            </Typography>
            <Tooltip title="Add To Cart">
              <Fab
                size="medium"
                color="primary"
                aria-label="add"
                onClick={handleAddToCart}
              >
                <AddShoppingCart />
              </Fab>
            </Tooltip>
          </>
        ) : null}
      </Toolbar>
    </>
  );
}

export default ShopToolbar;
