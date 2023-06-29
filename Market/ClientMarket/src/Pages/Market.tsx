import * as React from "react";

import { Box } from "@mui/material";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import Navbar from "../Components/Navbar";

import { pathShop } from "../Paths";
import ShopGrid from "../Components/ShopGrid";
import { useNavigate } from "react-router-dom";
import Shop from "../Objects/Shop";
import { getMarketInfo } from "../Services/MarketService";

function Market() {
  const theme = createTheme({
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

  const [marketShops, setMarketShops] = React.useState<Shop[] | null>(null);
  const navigate = useNavigate();

  const HandleGoToShop = (shopId: number) => {
    navigate(`${pathShop}?id=${shopId}`);
  };

  React.useEffect(() => {
    getMarketInfo()
      .then((shops: Shop[]) => {
        const openShops: Shop[] = shops.filter((shop: Shop) => shop.isOpen);
        setMarketShops(openShops);
      })
      .catch((e: any) => alert(e));
  }, []);

  return (
    <ThemeProvider theme={theme}>
      <Box>
        <Navbar />
      </Box>
      <Box sx={{ mt: 5 }}>
        {marketShops !== null
          ? marketShops.map((shop: Shop) => {
              return ShopGrid(
                shop.products,
                shop.id,
                shop.name,
                HandleGoToShop
              );
            })
          : null}
      </Box>
    </ThemeProvider>
  );
}

export default Market;
