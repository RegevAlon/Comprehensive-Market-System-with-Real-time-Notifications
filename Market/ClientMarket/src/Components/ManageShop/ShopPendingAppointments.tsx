import * as React from "react";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { Box, Button, Stack, ThemeProvider, createTheme } from "@mui/material";
import Shop from "../../Objects/Shop";
import { squaresColor, textColor } from "../../Utils";
import PendingAgreementInfo from "../../Objects/PendingAgreementInfo";
import { getUserName } from "../../Services/SessionService";
import {
  ApproveAppointment,
  DeclineAppointment,
  serverGetShopInfo,
} from "../../Services/MarketService";
import PendingAgreement from "../../Objects/PendingAgreement";

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
  const [requests, setRequests] = React.useState<PendingAgreementInfo[]>([]);

  const columns: GridColDef[] = [
    {
      field: "id",
      headerName: "Agreement ID",
      type: "number",
      flex: 0.2,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "description",
      headerName: "Request",
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
        <ThemeProvider theme={theme}>
          <Button
            variant={params.row.approved ? "contained" : "outlined"}
            onClick={() => {
              handleApproveAppointment(params.row.appointeeUsername);
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
            handleDeclineAppointment(params.row.appointeeUsername);
          }}
        >
          Decline
        </Button>
      ),
    },
  ];

  React.useEffect(() => {
    serverGetShopInfo(shop.id)
      .then((newShop: Shop) => {
        extractAgreements(newShop);
        handleChangedShop(newShop);
      })
      .catch((e) => {
        alert(e);
      });
  }, [requests]);

  const extractAgreements = (newShop: Shop) => {
    const requests: PendingAgreementInfo[] = [];
    const username: string | null = getUserName();
    let agreementIdCounter = 1;
    newShop.pendingAgreements?.forEach((agreement: PendingAgreement) => {
      if (agreement.pendings.includes(username !== null ? username : "")) {
        const paInfo: PendingAgreementInfo = new PendingAgreementInfo(
          agreementIdCounter,
          `${agreement.appointer} wants to manage ${agreement.appointee} to owner`,
          agreement.appointee,
          false,
          false,
          true
        );
        requests.push(paInfo);
      } else if (
        agreement.approved.includes(username !== null ? username : "")
      ) {
        const paInfo: PendingAgreementInfo = new PendingAgreementInfo(
          agreementIdCounter,
          `${agreement.appointer} wants to manage ${agreement.appointee} to owner`,
          agreement.appointee,
          true,
          false,
          false
        );
        requests.push(paInfo);
      } else if (
        agreement.declined.includes(username !== null ? username : "")
      ) {
        const paInfo: PendingAgreementInfo = new PendingAgreementInfo(
          agreementIdCounter,
          `${agreement.appointer} wants to manage ${agreement.appointee} to owner`,
          agreement.appointee,
          false,
          true,
          false
        );
        requests.push(paInfo);
      }
      agreementIdCounter++;
    });
    setRequests(requests);
  };

  const handleApproveAppointment = (appointeeUsername: string) => {
    ApproveAppointment(shop.id, appointeeUsername)
      .then(() =>
        serverGetShopInfo(shop.id).then((newShop: Shop) => {
          handleChangedShop(newShop);
        })
      )
      .catch((e) => {
        alert(e);
      });
  };

  const handleDeclineAppointment = (appointeeUsername: string) => {
    DeclineAppointment(shop.id, appointeeUsername)
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
    </Box>
  );
}
