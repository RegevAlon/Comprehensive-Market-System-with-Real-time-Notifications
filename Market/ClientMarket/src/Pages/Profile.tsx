import * as React from "react";
import Box from "@mui/material/Box";
import AddBusinessIcon from "@mui/icons-material/AddBusiness";
import ShopPop from "../Components/Pop-ups/ProfileShopPop";
import Typography from "@mui/material/Typography";
import {
  Button,
  Dialog,
  FormGroup,
  Stack,
  styled,
  Switch,
} from "@mui/material";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import Navbar from "../Components/Navbar";
import { Card, CardContent } from "@mui/material";
import Grid from "@mui/material/Grid";
import Shop from "../Objects/Shop";
import ShopIcon from "@mui/icons-material/Shop";
import { fetchResponse } from "../Services/GeneralService";
import { getIsGuest, getUserName } from "../Services/SessionService";
import { useNavigate } from "react-router-dom";
import { pathHome, pathMarket } from "../Paths";
import LoadingCircle from "../Components/LoadingCircle";
import AddShopPop from "../Components/Pop-ups/AddShopPop";
import SuccessSnackbar from "../Components/SuccessSnackbar";
import FailureSnackbar from "../Components/FailureSnackbar";
import {
  CloseShop,
  CreateShop,
  GetManagedShops,
  OpenShop,
  serverGetShopInfo,
} from "../Services/MarketService";
import { squaresColor } from "../Utils";
import Rating from "react-rating-stars-component";

const ShopCard = ({
  shop,
  handleChangedShop,
  showSuccess,
}: {
  shop: Shop;
  handleChangedShop: (s: Shop) => void;
  showSuccess: (m: string) => void;
}) => {
  const [openDialog, setOpenDialog] = React.useState(false);
  const [isShopOpen, setIsShopOpen] = React.useState(shop.isOpen);

  const handleClickOpenDialog = () => {
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
  };

  const handleChangeShopOpen = (event: React.ChangeEvent<HTMLInputElement>) => {
    const openShop: boolean = event.target.checked;
    if (openShop)
      OpenShop(getUserName(), shop.id)
        .then(() => {
          showSuccess("Shop " + shop.name + " Opened Successfully");
          setIsShopOpen(true);
          handleChangedShop(shop);
        })
        .catch((e) => {
          alert(e);
        });
    else
      CloseShop(getUserName(), shop.id)
        .then(() => {
          showSuccess("Shop " + shop.name + " Closed Successfully");
          setIsShopOpen(false);
          handleChangedShop(shop);
        })
        .catch((e) => {
          alert(e);
        });
  };

  return (
    <div>
      {openDialog && (
        <ShopPop
          shop={shop}
          handleCloseDialog={handleCloseDialog}
          handleChangedShop={handleChangedShop}
        />
      )}
      <Card
        sx={{
          width: "90%",
          boxShadow: 1,
          borderBlockColor: "black",
          borderRadius: 20,
          backgroundColor: squaresColor,
          p: 3,
        }}
      >
        <CardContent sx={{ display: "flex", flexDirection: "column" }}>
          <Typography sx={{ mb: 3 }} variant="h3">
            {shop.name}
          </Typography>
          <Rating
            count={5}
            size={24}
            value={shop.rating}
            edit={false}
            activeColor="#ffd700"
          />
          {/* <Typography variant="h5" sx={{ mr: 2 }}>
            Rating: {shop.rating} / 5
          </Typography> */}
          <Button
            variant="contained"
            endIcon={<ShopIcon />}
            sx={{ mt: 3, mb: 3 }}
            onClick={handleClickOpenDialog}
            disabled={false}
          >
            Enter As Manager
          </Button>
          <FormGroup>
            <Stack direction="row" spacing={1} alignItems="center">
              <Typography>{isShopOpen ? "Open" : "Close"}</Typography>
              <Switch
                checked={isShopOpen}
                disabled={false}
                onChange={handleChangeShopOpen}
              />
            </Stack>
          </FormGroup>
        </CardContent>
      </Card>
    </div>
  );
};

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

const Item = styled("div")(({ theme }) => ({
  ...theme.typography.body2,
  padding: theme.spacing(3),
  textAlign: "center",
  color: theme.palette.text.secondary,
}));

const ManagedShops = (
  shops: Shop[] | null,
  handleChangedShop: (shop: Shop) => void,
  showSuccessSnack: (m: string) => void
) => {
  return (
    <Grid>
      <div
        style={{
          position: "absolute",
          top: 250,
          left: 30,
          right: 0,
          marginLeft: 10,
          marginRight: 10,
        }}
      >
        <Stack direction="row" spacing={10}>
          {shops !== null
            ? shops.map((s) => (
                <Item>
                  <ShopCard
                    shop={s}
                    handleChangedShop={handleChangedShop}
                    showSuccess={showSuccessSnack}
                  />
                </Item>
              ))
            : null}
        </Stack>
      </div>
    </Grid>
  );
};

function Profile() {
  const [shops, setShops] = React.useState<Shop[] | null>(null);
  const [openShopForm, setOpenShopForm] = React.useState<boolean>(false);
  const [openSuccSnack, setOpenSuccSnack] = React.useState<boolean>(false);
  const [successProductMsg, setSuccessProductMsg] = React.useState<string>("");
  const [openFailSnack, setOpenFailSnack] = React.useState<boolean>(false);
  const [failureProductMsg, setFailureProductMsg] = React.useState<string>("");

  const showFailureSnack = (msg: string) => {
    setOpenFailSnack(true);
    setFailureProductMsg(msg);
  };

  const navigate = useNavigate();
  const showSuccessSnack = (msg: string) => {
    setOpenSuccSnack(true);
    setSuccessProductMsg(msg);
    setOpenShopForm(false);
  };

  const handleAddShop = (shopName: string) => {
    CreateShop(shopName)
      .then((newShop: Shop) =>
        setShops(shops === null ? [newShop] : shops.concat(newShop))
      )
      .then(() => showSuccessSnack("Added " + shopName + " Successfully"))
      .catch(showFailureSnack);
  };

  if (getIsGuest()) {
    alert("You are not allowed to visit this page!");
    navigate(`${pathHome}`);
  }

  React.useEffect(() => {
    GetManagedShops()
      .then((managedShops: Shop[]) => {
        setShops(managedShops);
      })
      .catch((e: any) => {
        alert(e.message);
      });
  }, []);

  const username = getUserName();

  const handleBackToMarket = () => {
    navigate(`${pathMarket}`);
  };

  const handleChangedShop = (changedShop: Shop) => {
    serverGetShopInfo(changedShop.id)
      .then((loadedShop: Shop) => {
        const newShops = shops?.map((currShop) => {
          return currShop.id === loadedShop.id ? loadedShop : currShop;
        });
        console.log(newShops);
        setShops(newShops || null);
      })
      .catch((e) => {
        alert(e);
        setShops([]);
      });
  };

  return shops !== null ? (
    <ThemeProvider theme={theme}>
      <Box>
        <Box>
          <Navbar />
        </Box>
        <Stack direction="column">
          <div
            style={{
              position: "absolute",
              top: 100,
              left: 0,
              right: 0,
              marginLeft: 10,
              marginRight: 10,
            }}
          >
            <Typography variant="h2" align="left" sx={{ ml: 10 }}>
              Hello {username}
            </Typography>
            <Typography variant="h4" align="left" sx={{ mt: 3, ml: 10 }}>
              My Shops:
            </Typography>
          </div>
          {ManagedShops(shops, handleChangedShop, showSuccessSnack)}
          <Button
            sx={{ mt: 65, ml: 1 }}
            variant="contained"
            endIcon={<AddBusinessIcon />}
            onClick={() => setOpenShopForm(true)}
          >
            Create Shop
          </Button>
          <Button
            variant="contained"
            onClick={handleBackToMarket}
            sx={{ mt: 5, ml: 1 }}
          >
            Back To Market
          </Button>
        </Stack>
      </Box>

      <AddShopPop
        open={openShopForm}
        handleAddShop={handleAddShop}
        handleClose={() => setOpenShopForm(false)}
      />
      <Dialog open={openFailSnack}>
        {FailureSnackbar(failureProductMsg, openFailSnack, () =>
          setOpenFailSnack(false)
        )}
      </Dialog>
      {SuccessSnackbar(successProductMsg, openSuccSnack, () =>
        setOpenSuccSnack(false)
      )}
    </ThemeProvider>
  ) : (
    LoadingCircle()
  );
}

export default Profile;
