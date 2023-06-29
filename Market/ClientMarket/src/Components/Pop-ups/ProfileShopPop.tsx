import * as React from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import { AppBar, Dialog, IconButton, Tab, Tabs, Toolbar } from "@mui/material";
import Shop from "../../Objects/Shop";
import CloseIcon from "@mui/icons-material/Close";
import ShopProductsEdit from "../ManageShop/ShopProductsEdit";
import ShopPurchases from "../ManageShop/ShopPurchases";
import ShopPurchasePolicies from "../ManageShop/ShopPurchasePolicies";
import ShopDiscountPolicies from "../ManageShop/ShopDiscountPolicies";
import ShopAppointments from "../ManageShop/ShopAppointments";
import ShopRules from "../ManageShop/ShopRules";
import ShopPendingAppointments from "../ManageShop/ShopPendingAppointments";
import ShopPendingBids from "../ManageShop/ShopPendingBids";

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ p: 3 }}>
          <Typography>{children}</Typography>
        </Box>
      )}
    </div>
  );
}

export function ShopTabs({
  shop,
  handleChangedShop,
  handleCloseDialog,
}: {
  shop: Shop;
  handleChangedShop: (s: Shop) => void;
  handleCloseDialog: any;
}) {
  const [value, setValue] = React.useState(0);
  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  return (
    <>
      <Box sx={{ width: "100%", mr: 3, backgroundColor: "#fff" }}>
        <Box
          sx={{
            borderBottom: 1,
            borderColor: "divider",
            backgroundColor: "#fff",
          }}
        >
          <Tabs value={value} onChange={handleChange}>
            <Tab sx={{ color: "#333", fontSize: 20 }} label="Products" id="0" />
            <Tab
              sx={{ color: "#333", fontSize: 20 }}
              label="Purchase History"
              id="1"
            />
            <Tab sx={{ color: "#333", fontSize: 20 }} label="Rules" id="2" />
            <Tab
              sx={{ color: "#333", fontSize: 20 }}
              label="Purchase Policies"
              id="3"
            />
            <Tab
              sx={{ color: "#333", fontSize: 20 }}
              label="Discount Policies"
              id="4"
            />
            <Tab
              sx={{ color: "#333", fontSize: 20 }}
              label="Appointments"
              id="5"
            />
            <Tab
              sx={{ color: "#333", fontSize: 20 }}
              label="Pending Appointments"
              id="6"
            />
            <Tab
              sx={{ color: "#333", fontSize: 20 }}
              label="Pending Bids"
              id="7"
            />
          </Tabs>
        </Box>
        <TabPanel value={value} index={0}>
          <ShopProductsEdit shop={shop} handleChangedShop={handleChangedShop} />
        </TabPanel>
        <TabPanel value={value} index={1}>
          <ShopPurchases shop={shop} handleChangedShop={handleChangedShop} />
        </TabPanel>
        <TabPanel value={value} index={2}>
          <ShopRules shop={shop} handleChangedShop={handleChangedShop} />
        </TabPanel>
        <TabPanel value={value} index={3}>
          <ShopPurchasePolicies
            shop={shop}
            handleChangedShop={handleChangedShop}
          />
        </TabPanel>
        <TabPanel value={value} index={4}>
          <ShopDiscountPolicies
            shop={shop}
            handleChangedShop={handleChangedShop}
          />
        </TabPanel>
        <TabPanel value={value} index={5}>
          <ShopAppointments shop={shop} handleChangedShop={handleChangedShop} />
        </TabPanel>
        <TabPanel value={value} index={6}>
          <ShopPendingAppointments
            shop={shop}
            handleChangedShop={handleChangedShop}
            handleCloseDialog={handleCloseDialog}
          />
        </TabPanel>
        <TabPanel value={value} index={7}>
          <ShopPendingBids
            shop={shop}
            handleChangedShop={handleChangedShop}
            handleCloseDialog={handleCloseDialog}
          />
        </TabPanel>
      </Box>
    </>
  );
}

export default function ShopPop({
  shop,
  handleCloseDialog,
  handleChangedShop,
}: {
  shop: Shop;
  handleCloseDialog: any;
  handleChangedShop: (s: Shop) => void;
}) {
  return (
    <Dialog fullScreen open={true} onClose={handleCloseDialog}>
      <AppBar sx={{ position: "relative", backgroundColor: "#fff" }}>
        <Toolbar>
          <Typography
            sx={{ ml: 2, flex: 1, color: "black", fontSize: 20 }}
            variant="h6"
            component="div"
          >
            Insights {shop.name}
          </Typography>
          <IconButton
            edge="start"
            color="default"
            onClick={handleCloseDialog}
            aria-label="close"
          >
            <CloseIcon />
          </IconButton>
        </Toolbar>
      </AppBar>
      <ShopTabs
        shop={shop}
        handleChangedShop={handleChangedShop}
        handleCloseDialog={handleCloseDialog}
      />
    </Dialog>
  );
}
