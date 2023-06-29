import * as React from "react";
import { styled, alpha } from "@mui/material/styles";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import Badge from "@mui/material/Badge";
import MenuItem from "@mui/material/MenuItem";
import Menu from "@mui/material/Menu";
import SearchIcon from "@mui/icons-material/Search";
import AccountCircle from "@mui/icons-material/AccountCircle";
import Button from "@mui/material/Button";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import { BadgeProps } from "@mui/material";
import ShoppingCartIcon from "@mui/icons-material/ShoppingCart";
import Stack from "@mui/material/Stack";
import {
  pathCart,
  pathHome,
  pathLogin,
  pathSearch,
  pathProfile,
  pathMarket,
  pathRegister,
  pathNotifications,
  pathAdmin,
} from "../Paths";
import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { Tooltip } from "@mui/material";
import {
  clearSession,
  getIsAdmin,
  getIsGuest,
  initSession,
  setIsAdmin,
} from "../Services/SessionService";
import {
  getProductNumber,
  getMessagesNumber,
  serverLogout,
  serverIsAdmin,
} from "../Services/MarketService";
import { Logout } from "@mui/icons-material";
import NotificationsActiveIcon from "@mui/icons-material/NotificationsActive";
import AdminPanelSettingsIcon from "@mui/icons-material/AdminPanelSettings";
import { fetchResponse } from "../Services/GeneralService";

const StyledBadge = styled(Badge)<BadgeProps>(({ theme }) => ({
  "& .MuiBadge-badge": {
    right: -3,
    top: 13,
    border: `2px solid ${theme.palette.background.paper}`,
    padding: "0 4px",
  },
}));

export default function Navbar() {
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const [numItemsInCart, setNumItemsInCart] = React.useState<number>(0);
  const [numOfNotifications, setNumOfNotifications] = React.useState<number>(0);
  const [isAdmin, setIsAdmin] = React.useState<boolean>(false);

  const handleLogin = () => {
    navigate(`${pathLogin}`);
  };

  const handleRegister = () => {
    navigate(`${pathRegister}`);
  };

  const handleProfileMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleSearch = () => {
    navigate(pathSearch);
  };

  const handleClickMarket = () => {
    navigate(pathMarket);
  };

  const handleClickAdminPage = () => {
    navigate(pathAdmin);
  };

  const handleMyAccountClick = () => {
    setAnchorEl(null);
    if (getIsGuest()) navigate(`${pathLogin}`);
    else navigate(`${pathProfile}`);
  };

  const menuId = "primary-search-account-menu";
  const renderMenuAccount = (
    <Menu
      anchorEl={anchorEl}
      anchorOrigin={{
        vertical: "top",
        horizontal: "right",
      }}
      id={menuId}
      keepMounted
      transformOrigin={{
        vertical: "top",
        horizontal: "right",
      }}
      open={Boolean(anchorEl)}
      onClose={handleMenuClose}
    >
      {getIsGuest() ? (
        <>
          <MenuItem onClick={handleLogin}>Login</MenuItem>
          <MenuItem onClick={handleRegister}>Register</MenuItem>
        </>
      ) : (
        <MenuItem onClick={handleMyAccountClick}>Profile</MenuItem>
      )}
    </Menu>
  );

  React.useEffect(() => {
    if (!getIsGuest()) {
      getMessagesNumber()
        .then((notifications: number) => {
          setNumOfNotifications(notifications);
        })
        .catch((e: any) => alert(e));
    }
  }, []);

  React.useEffect(() => {
    if (!getIsGuest()) {
      getProductNumber()
        .then((amount: number) => {
          setNumItemsInCart(amount);
        })
        .catch((e) => {
          alert(e);
        });
    }
  }, []);

  React.useEffect(() => {
    if (!getIsGuest()) {
      serverIsAdmin()
        .then((res: boolean) => {
          setIsAdmin(res);
        })
        .catch((e) => {
          alert(e);
        });
    }
  }, []);

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

  const handleLogout = () => {
    if (!getIsGuest()) {
      serverLogout();
      clearSession();
      initSession();
      navigate(pathHome);
    }
  };

  return (
    <ThemeProvider theme={theme}>
      <AppBar position="sticky">
        <Toolbar
          sx={{
            display: "flex",
            justifyContent: "space-between",
            position: "fixed",
            top: 0,
            left: "50%",
            transform: "translateX(-50%)",
          }}
        >
          <div>
            <Stack direction="row" spacing={2}>
              <Typography
                onClick={handleClickMarket}
                variant="h6"
                noWrap
                component="div"
                sx={{
                  color: "black",
                  display: { xs: "none", sm: "block" },
                  "&:hover": {
                    cursor: "pointer",
                  },
                }}
              >
                Team12B Market
              </Typography>
              <Button
                variant="outlined"
                color="primary"
                startIcon={<SearchIcon />}
                type="submit"
                onClick={handleSearch}
              >
                Search Products
              </Button>
            </Stack>

            <Box sx={{}} />
          </div>
          {/* <div>
            <Box
              component="form"
              noValidate
              onSubmit={(e: any) => {
                handleSearch();
              }}
            ></Box>
          </div> */}
          <div>
            <Box sx={{ display: { xs: "none", md: "flex" } }}>
              <Tooltip title="Cart">
                <IconButton
                  aria-label="cart"
                  size="large"
                  color="default"
                  component={Link}
                  to={pathCart}
                >
                  <StyledBadge badgeContent={numItemsInCart} color="secondary">
                    <ShoppingCartIcon />
                  </StyledBadge>
                </IconButton>
              </Tooltip>
              {getIsGuest() ? null : (
                <Tooltip title="Notifications">
                  <IconButton
                    aria-label="notification"
                    size="small"
                    color="default"
                    component={Link}
                    to={pathNotifications}
                  >
                    <StyledBadge
                      badgeContent={numOfNotifications}
                      color="secondary"
                    >
                      <NotificationsActiveIcon />
                    </StyledBadge>
                  </IconButton>
                </Tooltip>
              )}
              {getIsGuest() ? null : (
                <Tooltip title="Logout">
                  <IconButton
                    size="large"
                    edge="end"
                    aria-label="account of current user"
                    aria-controls={menuId}
                    aria-haspopup="true"
                    onClick={handleLogout}
                    color="default"
                  >
                    <Logout />
                  </IconButton>
                </Tooltip>
              )}
              {!isAdmin ? null : (
                <Tooltip title="Admin View">
                  <IconButton
                    size="large"
                    edge="end"
                    aria-label="Admin View"
                    aria-controls={menuId}
                    aria-haspopup="true"
                    onClick={handleClickAdminPage}
                    color="default"
                  >
                    <AdminPanelSettingsIcon />
                  </IconButton>
                </Tooltip>
              )}
              <IconButton
                size="large"
                edge="end"
                aria-label="account of current user"
                aria-controls={menuId}
                aria-haspopup="true"
                onClick={handleProfileMenuOpen}
                color="default"
              >
                <AccountCircle />
              </IconButton>
            </Box>
          </div>
        </Toolbar>
      </AppBar>
      {renderMenuAccount}
    </ThemeProvider>
  );
}
