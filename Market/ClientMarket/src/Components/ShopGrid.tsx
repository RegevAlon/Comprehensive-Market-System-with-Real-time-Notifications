import {
  Box,
  Button,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import ExitToAppIcon from "@mui/icons-material/ExitToApp";
import Product from "../Objects/Product";
import { squaresColor } from "../Utils";

function ShopGrid(
  products: Product[],
  shopId: number,
  shopName: string,
  handleGoToShop: (shopId: number) => void
) {
  const productsRows: Product[] = products;

  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
        marginTop: 3,
      }}
    >
      <Box
        sx={{
          width: "90%",
          boxShadow: 1,
          borderBlockColor: "black",
          borderRadius: 20,
          backgroundColor: squaresColor,
          p: 3,
        }}
      >
        <Typography variant="h4" sx={{ marginBottom: 3 }}>
          {shopName}
        </Typography>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell
                color="#242424"
                sx={{ fontWeight: "bold", color: "black" }}
              >
                Product
              </TableCell>
              <TableCell sx={{ fontWeight: "bold", color: "black" }}>
                Price
              </TableCell>
              <TableCell sx={{ fontWeight: "bold", color: "black" }}>
                Category
              </TableCell>
              <TableCell sx={{ fontWeight: "bold", color: "black" }}>
                Quantity
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {productsRows.map((product) => (
              <TableRow key={product.id}>
                <TableCell>{product.name}</TableCell>
                <TableCell>{product.price}</TableCell>
                <TableCell>{product.category}</TableCell>
                <TableCell>{product.quantity}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
        <Button
          variant="contained"
          size="large"
          sx={{ marginTop: 3 }}
          startIcon={<ExitToAppIcon />}
          onClick={() => handleGoToShop(shopId)}
        >
          Go to {shopName}
        </Button>
      </Box>
    </Box>
  );
}

export default ShopGrid;
