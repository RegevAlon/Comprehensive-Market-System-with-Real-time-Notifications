import * as React from "react";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { Box, Button, Dialog, Stack } from "@mui/material";
import Shop from "../../Objects/Shop";
import { useNavigate } from "react-router-dom";
import FailureSnackbar from "../FailureSnackbar";
import SuccessSnackbar from "../SuccessSnackbar";
import {
  addPurchasePolicy,
  removePolicy,
  serverGetShopInfo,
} from "../../Services/MarketService";
import RemovePurchasePolicyPop from "../Pop-ups/RemovePurchasePolicyPop";
import AddPurchasePolicyPop from "../Pop-ups/AddPurchasePolicyPop";
import PurchasePolicyInfo from "../../Objects/Policies/PurchasePolicyInfo";
import { squaresColor, textColor } from "../../Utils";

const columns: GridColDef[] = [
  {
    field: "id",
    headerName: "Policy ID",
    type: "number",
    flex: 0.5,
    align: "center",
    headerAlign: "center",
  },
  {
    field: "description",
    headerName: "Policy Description",
    type: "string",
    flex: 1,
    align: "center",
    headerAlign: "center",
  },
  {
    field: "expirationDate",
    headerName: "Exp. Date",
    type: "string",
    flex: 1,
    align: "center",
    headerAlign: "center",
  },
];

export default function ShopPurchasePolicies({
  shop,
  handleChangedShop,
}: {
  shop: Shop;
  handleChangedShop: (s: Shop) => void;
}) {
  const initSize: number = 5;

  const [pageSize, setPageSize] = React.useState<number>(initSize);
  const [rows, setRows] = React.useState<PurchasePolicyInfo[]>([]);
  const [selectionModel, setSelectionModel] = React.useState<number[]>([]);
  const [chosenIds, setChosenIds] = React.useState<number[]>([]);
  const [openFailSnack, setOpenFailSnack] = React.useState<boolean>(false);
  const [failureProductMsg, setFailureProductMsg] = React.useState<string>("");
  const [openSuccSnack, setOpenSuccSnack] = React.useState<boolean>(false);
  const [successProductMsg, setSuccessProductMsg] = React.useState<string>("");
  const [openAddPolicyPop, setOpenAddPolicyPop] =
    React.useState<boolean>(false);
  const [openRemovePolicyPop, setOpenRemovePolicyPop] =
    React.useState<boolean>(false);

  const showSuccessSnack = (msg: string) => {
    setOpenSuccSnack(true);
    setSuccessProductMsg(msg);
  };

  const showFailureSnack = (msg: string) => {
    setOpenFailSnack(true);
    setFailureProductMsg(msg);
  };

  const handleSelectionChanged = (newSelection: any) => {
    const chosenIds: number[] = newSelection;
    setSelectionModel(newSelection);
    setChosenIds(chosenIds);
  };

  const handleRemovePurchasePolicy = (policyId: number) => {
    removePolicy(shop.id, policyId, "PurchasePolicy")
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.purchasePolicies);
        handleChangedShop(shop);
        showSuccessSnack("Removed Policy Successfully");
      })
      .catch((e) => {
        alert(e);
        showFailureSnack("Coludn't remove this policy");
      });
  };

  const handleAddPurchasePolicy = (
    expDate: string,
    subject: string,
    ruleId: number
  ) => {
    addPurchasePolicy(shop.id, expDate, subject, ruleId)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.purchasePolicies);
        handleChangedShop(shop);
        showSuccessSnack("Added Policy Successfully");
      })
      .catch((e) => {
        alert(e);
        showFailureSnack("Coludn't add this policy");
      });
  };

  React.useEffect(() => {
    setRows(shop.purchasePolicies);
  }, [shop]);

  const handleRemovePolicyPop = () => {
    setOpenRemovePolicyPop(true);
  };

  const handleAddPolicyPop = () => {
    setOpenAddPolicyPop(true);
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
              onSelectionModelChange={handleSelectionChanged}
            />
            <Stack
              direction="row"
              justifyContent="space-between"
              width={"95vw"}
            >
              <Box>
                <Button
                  variant="contained"
                  sx={{ mt: 4 }}
                  onClick={handleAddPolicyPop}
                >
                  Add Purchase Policy
                </Button>
                {openAddPolicyPop && (
                  <AddPurchasePolicyPop
                    handleAddPurchasePolicy={handleAddPurchasePolicy}
                    open={openAddPolicyPop}
                    handleClose={() => setOpenAddPolicyPop(false)}
                  />
                )}
              </Box>
              <Box>
                <Button
                  sx={{ mt: 4 }}
                  color="error"
                  variant="contained"
                  onClick={handleRemovePolicyPop}
                >
                  Remove Policy
                </Button>
                {openRemovePolicyPop && (
                  <RemovePurchasePolicyPop
                    handleRemovePurchasePolicy={handleRemovePurchasePolicy}
                    open={openRemovePolicyPop}
                    handleClose={() => setOpenRemovePolicyPop(false)}
                  />
                )}
              </Box>
            </Stack>
          </div>
        </div>
      </div>
      <Dialog open={openFailSnack}>
        {FailureSnackbar(failureProductMsg, openFailSnack, () =>
          setOpenFailSnack(false)
        )}
      </Dialog>
      {SuccessSnackbar(successProductMsg, openSuccSnack, () =>
        setOpenSuccSnack(false)
      )}
    </Box>
  );
}
