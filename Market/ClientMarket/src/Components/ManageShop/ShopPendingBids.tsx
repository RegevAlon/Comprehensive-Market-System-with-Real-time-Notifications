import * as React from "react";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { Box, Button, Stack, ThemeProvider, createTheme } from "@mui/material";
import Shop from "../../Objects/Shop";
import { squaresColor, textColor } from "../../Utils";
import {
  OfferCounterBid,
  RemoveBid,
  approveBid,
  declineBid,
  serverGetShopInfo,
} from "../../Services/MarketService";
import Product from "../../Objects/Product";
import Bid from "../../Objects/Bid";
import { getUserName } from "../../Services/SessionService";
import BidInfo from "../../Objects/BidInfo";
import AddCounterBidPop from "../Pop-ups/AddCounterBidPop";

const theme = createTheme({
  palette: {
    primary: {
      main: "#4caf50", // Green color
    },
  },
});

export default function ShopPendingAppointments({
  shop,
  handleChangedShop,
  handleCloseDialog,
}: {
  shop: Shop;
  handleChangedShop: (s: Shop) => void;
  handleCloseDialog: any;
}) {
  const initSize: number = 5;

  const [pageSize, setPageSize] = React.useState<number>(initSize);
  const [requests, setRequests] = React.useState<BidInfo[]>([]);
  const [bidUsername, setBidUsername] = React.useState<string>("");
  const [selectedProdId, setSelectedProdId] = React.useState<string>("");
  const [approvedCount, setApprovedCount] = React.useState<number>(0);
  const [declinedCount, setDeclinedCount] = React.useState<number>(0);
  const [openCounterBidPop, setOpenCounterBidPop] =
    React.useState<boolean>(false);

  const columns: GridColDef[] = [
    {
      field: "id",
      headerName: "Bid ID",
      type: "number",
      flex: 0,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "prodId",
      headerName: "Product ID",
      type: "number",
      flex: 0,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "username",
      headerName: "Bid Username",
      type: "string",
      flex: 0.2,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "information",
      headerName: "Bid Request",
      type: "string",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "",
      headerName: "",
      flex: 0.25,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Button
          variant="outlined"
          onClick={() => {
            setBidUsername(params.row.username);
            setSelectedProdId(params.row.prodId);
            setOpenCounterBidPop(true);
          }}
        >
          Counter Bid
        </Button>
      ),
    },
    {
      field: "'",
      headerName: "",
      flex: 0.25,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <ThemeProvider theme={theme}>
          <Button
            variant={params.row.approved ? "contained" : "outlined"}
            onClick={() => {
              handleApproveBid(params.row.prodId, params.row.username);
            }}
          >
            Approve
          </Button>
        </ThemeProvider>
      ),
    },
    {
      field: ".",
      headerName: "",
      flex: 0.25,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Button
          variant={params.row.declined ? "contained" : "outlined"}
          color="error"
          onClick={() => {
            handleDeclineBid(params.row.prodId, params.row.username);
          }}
        >
          Decline
        </Button>
      ),
    },
    {
      field: ",",
      headerName: "",
      flex: 0.25,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Button
          variant="contained"
          color="error"
          onClick={() => {
            handleRemoveBid(params.row.username, params.row.prodId);
          }}
        >
          Remove Bid
        </Button>
      ),
    },
  ];

  React.useEffect(() => {
    serverGetShopInfo(shop.id)
      .then((shop: Shop) => {
        extractRequests(shop);
      })
      .catch((e) => {
        alert(e);
      });
  }, [requests]);

  const extractRequests = (shop: Shop) => {
    const requests: BidInfo[] = [];
    const username: string | null = getUserName();
    let bidIdCounter = 1;
    shop.products.forEach((product: Product) => {
      product.bids.forEach((bid: Bid) => {
        if (bid.isClosed) return;
        if (bid.pendingRequests?.includes(username !== null ? username : "")) {
          const bidInfo: BidInfo = new BidInfo(
            bidIdCounter,
            product.id,
            bid.biddingMember,
            product.name,
            bid.quantity,
            bid.suggestedPrice,
            false,
            false,
            true
          );
          requests.push(bidInfo);
        } else if (
          bid.approveRequests?.includes(username !== null ? username : "")
        ) {
          const bidInfo: BidInfo = new BidInfo(
            bidIdCounter,
            product.id,
            bid.biddingMember,
            product.name,
            bid.quantity,
            bid.suggestedPrice,
            true,
            false,
            false
          );
          requests.push(bidInfo);
        } else if (
          bid.dissapproveRequests?.includes(username !== null ? username : "")
        ) {
          const bidInfo: BidInfo = new BidInfo(
            bidIdCounter,
            product.id,
            bid.biddingMember,
            product.name,
            bid.quantity,
            bid.suggestedPrice,
            false,
            true,
            false
          );
          requests.push(bidInfo);
        }
        bidIdCounter++;
      });
    });
    setRequests(requests);
  };

  const handleApproveBid = (prodId: string, username: string) => {
    approveBid(shop.id, username, parseInt(prodId))
      .then(() =>
        serverGetShopInfo(shop.id).then((newShop: Shop) => {
          handleChangedShop(newShop);
        })
      )
      .catch((e) => {
        alert(e);
      });
  };

  const handleCounterBid = (counterBid: number) => {
    OfferCounterBid(shop.id, bidUsername, parseInt(selectedProdId), counterBid)
      .then(() =>
        serverGetShopInfo(shop.id).then((newShop: Shop) => {
          handleChangedShop(newShop);
        })
      )
      .catch((e) => {
        alert(e);
      });
  };

  const handleDeclineBid = (prodId: string, username: string) => {
    declineBid(shop.id, username, parseInt(prodId))
      .then(() =>
        serverGetShopInfo(shop.id).then((newShop: Shop) => {
          handleChangedShop(newShop);
        })
      )
      .catch((e) => {
        alert(e);
      });
  };

  const handleRemoveBid = (bidUsername: string, prodId: string) => {
    RemoveBid(shop.id, bidUsername, parseInt(prodId))
      .then(() =>
        serverGetShopInfo(shop.id).then((newShop: Shop) => {
          handleChangedShop(newShop);
        })
      )
      .catch((e) => {
        alert(e);
      });
  };

  return (
    <Box
      sx={{
        width: "97%",
        boxShadow: 1,
        borderRadius: 4,
        p: 3,
        backgroundColor: squaresColor,
      }}
    >
      <Stack direction="row">{}</Stack>
      <div style={{ height: "50vh", width: "100%" }}>
        <div style={{ display: "flex", height: "100%" }}>
          <div style={{ flexGrow: 1 }}>
            <DataGrid
              rows={requests}
              columns={columns}
              sx={{
                width: "95vw",
                height: "50vh",
                color: textColor,
                "& .MuiDataGrid-cell:hover": {
                  color: "primary.main",
                  border: 1,
                },
              }}
              pageSize={pageSize}
              onPageSizeChange={(newPageSize: any) => setPageSize(newPageSize)}
              rowsPerPageOptions={[initSize, initSize + 5, initSize + 10]}
              pagination
            />
          </div>
        </div>
      </div>
      {openCounterBidPop && (
        <AddCounterBidPop
          handleOfferCounterBid={handleCounterBid}
          open={openCounterBidPop}
          handleClose={() => setOpenCounterBidPop(false)}
        />
      )}
    </Box>
  );
}
