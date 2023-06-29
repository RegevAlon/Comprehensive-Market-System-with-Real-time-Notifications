import * as React from "react";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { Box, Button, Dialog, Stack } from "@mui/material";
import Shop from "../../Objects/Shop";
import FailureSnackbar from "../FailureSnackbar";
import SuccessSnackbar from "../SuccessSnackbar";
import {
  addCompositePolicy,
  addDiscountPolicy,
  removePolicy,
  serverGetShopInfo,
} from "../../Services/MarketService";
import AddDiscountPolicyPop from "../Pop-ups/AddDiscountPolicyPop";
import RemoveDiscountPolicyPop from "../Pop-ups/RemoveDiscountPolicyPop";
import DiscountPolicyInfo from "../../Objects/Policies/DiscountPolicyInfo";
import { squaresColor, textColor } from "../../Utils";
import AddCompositeDiscountPolicyPop from "../Pop-ups/AddCompositeDiscountPolicyPop";

class DiscountPolicyRow {
  id: number;
  expirationDate: string;
  ruleId: number;
  ruleType: string;
  description: string;
  precentage: number;
  constructor(
    id: number,
    expirationDate: string,
    ruleId: number,
    ruleType: string,
    description: string,
    precentage: number
  ) {
    this.id = id;
    this.expirationDate = expirationDate;
    this.ruleId = ruleId;
    this.ruleType = ruleType;
    this.description = description;
    this.precentage = precentage;
  }
}

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

export default function ShopDiscountPolicies({
  shop,
  handleChangedShop,
}: {
  shop: Shop;
  handleChangedShop: (s: Shop) => void;
}) {
  const initSize: number = 5;

  const [pageSize, setPageSize] = React.useState<number>(initSize);
  const [rows, setRows] = React.useState<DiscountPolicyInfo[]>([]);
  const [openFailSnack, setOpenFailSnack] = React.useState<boolean>(false);
  const [failureProductMsg, setFailureProductMsg] = React.useState<string>("");
  const [openSuccSnack, setOpenSuccSnack] = React.useState<boolean>(false);
  const [successProductMsg, setSuccessProductMsg] = React.useState<string>("");
  const [openAddPolicyPop, setOpenAddPolicyPop] =
    React.useState<boolean>(false);
  const [openRemovePolicyPop, setOpenRemovePolicyPop] =
    React.useState<boolean>(false);
  const [openAddCompositePolicyPop, setOpenAddCompositePolicyPop] =
    React.useState<boolean>(false);

  const showSuccessSnack = (msg: string) => {
    setOpenSuccSnack(true);
    setSuccessProductMsg(msg);
  };

  const showFailureSnack = (msg: string) => {
    setOpenFailSnack(true);
    setFailureProductMsg(msg);
  };

  const handleRemoveDiscountPolicy = (policyId: number) => {
    removePolicy(shop.id, policyId, "DiscountPolicy")
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.discountPolicies);
        handleChangedShop(shop);
        showSuccessSnack("Removed Policy Successfully");
      })
      .catch((e) => {
        alert(e);
        showFailureSnack("Coludn't remove this policy");
      });
  };

  const handleAddDiscountPolicy = (
    expDate: string,
    subject: string,
    ruleId: number,
    precentage: number
  ) => {
    addDiscountPolicy(shop.id, expDate, subject, ruleId, precentage / 100)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.discountPolicies);
        handleChangedShop(shop);
        showSuccessSnack("Added Policy Successfully");
      })
      .catch((e) => {
        alert(e);
        showFailureSnack("Coludn't add this policy");
      });
  };

  const handleAddCompositeDiscountPolicy = (
    expDate: string,
    subject: string,
    operator: number,
    policiesIds: number[]
  ) => {
    addCompositePolicy(shop.id, expDate, subject, operator, policiesIds)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.discountPolicies);
        handleChangedShop(shop);
        showSuccessSnack("Added Policy Successfully");
      })
      .catch((e) => {
        alert(e);
        showFailureSnack("Coludn't add this policy");
      });
  };

  React.useEffect(() => {
    setRows(shop.discountPolicies);
  }, [shop]);

  const handleRemovePolicyPop = () => {
    setOpenRemovePolicyPop(true);
  };

  const handleAddCompositePolicyPop = () => {
    setOpenAddCompositePolicyPop(true);
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
                  Add Discount Policy
                </Button>
                {openAddPolicyPop && (
                  <AddDiscountPolicyPop
                    handleAddDiscountPolicy={handleAddDiscountPolicy}
                    open={openAddPolicyPop}
                    handleClose={() => setOpenAddPolicyPop(false)}
                  />
                )}
              </Box>
              <Box>
                <Button
                  sx={{ mt: 4 }}
                  color="primary"
                  variant="contained"
                  onClick={handleAddCompositePolicyPop}
                >
                  Add Composite Policy
                </Button>
                {openAddCompositePolicyPop && (
                  <AddCompositeDiscountPolicyPop
                    handleAddCompositeDiscountPolicy={
                      handleAddCompositeDiscountPolicy
                    }
                    open={openAddCompositePolicyPop}
                    handleClose={() => setOpenAddCompositePolicyPop(false)}
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
                  <RemoveDiscountPolicyPop
                    handleRemoveDiscountPolicy={handleRemoveDiscountPolicy}
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
