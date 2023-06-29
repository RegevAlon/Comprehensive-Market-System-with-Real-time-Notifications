import * as React from "react";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { Box, Button, Dialog, Stack } from "@mui/material";
import Shop from "../../Objects/Shop";
import FailureSnackbar from "../FailureSnackbar";
import SuccessSnackbar from "../SuccessSnackbar";
import Appointment from "../../Objects/Appointment";
import RemoveAppointeePop from "../Pop-ups/RemoveAppointeePop";
import AppointPop from "../Pop-ups/AppointPop";
import {
  ChangePermission,
  RemoveAppointee,
  appoint,
  serverGetShopInfo,
} from "../../Services/MarketService";
import Permission from "../../Objects/Permission";
import Roles from "../../Objects/Role";
import { squaresColor, textColor } from "../../Utils";
import ChangePermissionPop from "../Pop-ups/ChangePermissionPop";

const columns: GridColDef[] = [
  {
    field: "id",
    headerName: "Appointment ID",
    type: "number",
    flex: 0.5,
    align: "center",
    editable: false,
    headerAlign: "center",
  },
  {
    field: "member",
    headerName: "Member",
    type: "string",
    flex: 0.5,
    align: "center",
    editable: false,
    headerAlign: "center",
  },
  {
    field: "role",
    headerName: "Role",
    type: "string",
    flex: 0.5,
    align: "center",
    editable: false,
    headerAlign: "center",
  },
  {
    field: "appointer",
    headerName: "Appointer",
    type: "string",
    flex: 0.5,
    align: "center",
    editable: false,
    headerAlign: "center",
  },
  {
    field: "permission",
    headerName: "Permission",
    type: "string",
    flex: 1,
    align: "center",
    headerAlign: "center",
  },
  {
    field: "appointees",
    headerName: "Appointees",
    type: "string[]",
    flex: 1,
    align: "center",
    headerAlign: "center",
  },
];

export default function ShopAppointments({
  shop,
  handleChangedShop,
}: {
  shop: Shop;
  handleChangedShop: (s: Shop) => void;
}) {
  const initSize: number = 5;

  const [pageSize, setPageSize] = React.useState<number>(initSize);
  const [rows, setRows] = React.useState<Appointment[]>([]);
  const [openFailSnack, setOpenFailSnack] = React.useState<boolean>(false);
  const [failureAppointMsg, setFailureAppointMsg] = React.useState<string>("");
  const [openSuccSnack, setOpenSuccSnack] = React.useState<boolean>(false);
  const [successAppointMsg, setSuccessAppointMsg] = React.useState<string>("");
  const [openAppointPop, setOpenAppointPop] = React.useState<boolean>(false);
  const [openRemovePop, setOpenRemovePop] = React.useState<boolean>(false);
  const [openChangePermissionPop, setOpenChangePermissionPop] =
    React.useState<boolean>(false);

  React.useEffect(() => {
    let idCounter: number = 0;
    serverGetShopInfo(shop.id)
      .then((newShop: Shop) => {
        setRows(newShop.appointments);
        newShop.appointments.forEach((appoint: Appointment) => {
          appoint.id = idCounter;
          idCounter = idCounter + 1;
        });
        handleChangedShop(newShop);
      })
      .catch((e) => {
        alert(e);
      });
  }, [shop.appointments]);

  const handleRemoveAppointee = (appointee: string) => {
    RemoveAppointee(appointee, shop.id)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.appointments);
        handleChangedShop(shop);
      })
      .catch((e) => {
        alert(e);
      });
  };

  const handleAppoint = (
    appointee: string,
    roleMsg: string,
    permissionMsg: string
  ) => {
    const role: number | null =
      roleMsg === "1"
        ? Roles.Founder + 1
        : roleMsg === "2"
        ? Roles.Owner + 1
        : roleMsg === "3"
        ? Roles.Manager + 1
        : null;

    const permission: number | null =
      permissionMsg === "1"
        ? Permission.ManageSupply
        : permissionMsg === "2"
        ? Permission.Appoint
        : permissionMsg === "3"
        ? Permission.Policy
        : permissionMsg === "4"
        ? Permission.UserApplications
        : permissionMsg === "5"
        ? Permission.ShopPurchaseHistory
        : permissionMsg === "6"
        ? Permission.ShopAppointmentsInfo
        : permissionMsg === "7"
        ? Permission.OpenCloseShop
        : permissionMsg === "8"
        ? Permission.BidsPermissions
        : permissionMsg === "9"
        ? Permission.All
        : null;

    appoint(appointee, shop.id, role, permission)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        let idCounter: number = 0;
        newShop.appointments.forEach((appoint: Appointment) => {
          appoint.id = idCounter;
          idCounter = idCounter + 1;
        });
        setRows(newShop.appointments);
        handleChangedShop(newShop);
        setOpenSuccSnack(true);
        setSuccessAppointMsg(
          `Appointment of ${appointee} succeeded and waiting for managers approvals`
        );
      })
      .catch((e) => {
        setOpenFailSnack(true);
        setFailureAppointMsg(`Appointment of ${appointee} Failed`);
        alert(e);
      });
  };

  const handleChangePermission = (appointee: string, permissionMsg: string) => {
    const permission: number | null =
      permissionMsg === "1"
        ? Permission.ManageSupply
        : permissionMsg === "2"
        ? Permission.Appoint
        : permissionMsg === "3"
        ? Permission.Policy
        : permissionMsg === "4"
        ? Permission.UserApplications
        : permissionMsg === "5"
        ? Permission.ShopPurchaseHistory
        : permissionMsg === "6"
        ? Permission.ShopAppointmentsInfo
        : permissionMsg === "7"
        ? Permission.OpenCloseShop
        : permissionMsg === "8"
        ? Permission.BidsPermissions
        : permissionMsg === "9"
        ? Permission.All
        : null;

    ChangePermission(appointee, shop.id, permission)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        let idCounter: number = 0;
        newShop.appointments.forEach((appoint: Appointment) => {
          appoint.id = idCounter;
          idCounter = idCounter + 1;
        });
        setRows(newShop.appointments);
        handleChangedShop(newShop);
        setOpenSuccSnack(true);
        setSuccessAppointMsg(`Permission of ${appointee} has changed`);
      })
      .catch((e) => {
        setOpenFailSnack(true);
        setFailureAppointMsg(`Failed to change permission to ${appointee}`);
        alert(e);
      });
  };

  const handleRemovePop = () => {
    setOpenRemovePop(true);
  };

  const handleAppointPop = () => {
    setOpenAppointPop(true);
  };

  const handleChangePermissionPop = () => {
    setOpenChangePermissionPop(true);
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
              rows={rows}
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
            <Stack
              direction="row"
              justifyContent="space-between"
              width={"95vw"}
            >
              <Box>
                <Button
                  sx={{ mt: 4 }}
                  color="primary"
                  variant="contained"
                  onClick={handleAppointPop}
                >
                  Appoint
                </Button>
                {openAppointPop && (
                  <AppointPop
                    handleAppoint={handleAppoint}
                    open={openAppointPop}
                    handleClose={() => setOpenAppointPop(false)}
                  />
                )}
              </Box>
              <Box>
                <Button
                  sx={{ mt: 4 }}
                  color="primary"
                  variant="contained"
                  onClick={handleChangePermissionPop}
                >
                  Change Permission
                </Button>
                {openChangePermissionPop && (
                  <ChangePermissionPop
                    handleChangePermission={handleChangePermission}
                    open={openChangePermissionPop}
                    handleClose={() => setOpenChangePermissionPop(false)}
                  />
                )}
              </Box>
              <Box>
                <Button
                  sx={{ mt: 4 }}
                  color="error"
                  variant="contained"
                  onClick={handleRemovePop}
                >
                  Remove Appointee
                </Button>
                {openRemovePop && (
                  <RemoveAppointeePop
                    handleRemoveAppointee={handleRemoveAppointee}
                    open={openRemovePop}
                    shopId={shop.id}
                    handleClose={() => setOpenRemovePop(false)}
                  />
                )}
              </Box>
            </Stack>
          </div>
        </div>
      </div>
      <Dialog open={openFailSnack}>
        {FailureSnackbar(failureAppointMsg, openFailSnack, () =>
          setOpenFailSnack(false)
        )}
      </Dialog>
      {SuccessSnackbar(successAppointMsg, openSuccSnack, () =>
        setOpenSuccSnack(false)
      )}
    </Box>
  );
}
