import * as React from "react";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { Box, Button, Dialog, Stack } from "@mui/material";
import Shop from "../../Objects/Shop";
import FailureSnackbar from "../FailureSnackbar";
import { useNavigate } from "react-router-dom";
import { pathProfile } from "../../Paths";
import SuccessSnackbar from "../SuccessSnackbar";
import {
  AddCompositeRule,
  AddQuantityRule,
  AddSimpleRule,
  AddTotalPriceRule,
  serverGetShopInfo,
} from "../../Services/MarketService";
import AddRulePop from "../Pop-ups/AddRulePop";
import RemoveRulePop from "../Pop-ups/RemoveRulePop";
import RuleInfo from "../../Objects/Rules/RuleInfo";
import { squaresColor, textColor } from "../../Utils";

const columns: GridColDef[] = [
  {
    field: "id",
    headerName: "Rule ID",
    type: "number",
    flex: 0.25,
    align: "center",
    headerAlign: "center",
  },
  {
    field: "description",
    headerName: "Description",
    type: "string",
    flex: 1,
    align: "center",
    headerAlign: "center",
  },
];

export default function ShopRules({
  shop,
  handleChangedShop,
}: {
  shop: Shop;
  handleChangedShop: (s: Shop) => void;
}) {
  let rowId = 0;
  const initSize: number = 5;

  const navigate = useNavigate();
  const [pageSize, setPageSize] = React.useState<number>(initSize);
  const [rows, setRows] = React.useState<RuleInfo[]>([]);
  const [openFailSnack, setOpenFailSnack] = React.useState<boolean>(false);
  const [failureAppointMsg, setFailureProductMsg] = React.useState<string>("");
  const [openSuccSnack, setOpenSuccSnack] = React.useState<boolean>(false);
  const [successAppointMsg, setSuccessProductMsg] = React.useState<string>("");
  const [openAddRulePop, setOpenAddRulePop] = React.useState<boolean>(false);

  React.useEffect(() => {
    setRows(shop.rules);
  }, [shop]);

  const handleAddSimpleRule = (subject: string) => {
    AddSimpleRule(shop.id, subject)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.rules);
        handleChangedShop(shop);
      })
      .catch((e) => {
        alert(e);
      });
  };

  const handleAddQuantityRule = (
    subject: string,
    minQuantity: number,
    maxQuantity: number
  ) => {
    AddQuantityRule(shop.id, subject, minQuantity, maxQuantity)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.rules);
        handleChangedShop(shop);
      })
      .catch((e) => {
        alert(e);
      });
  };

  const handleAddTotalPriceRule = (subject: string, targetPrice: number) => {
    AddTotalPriceRule(shop.id, subject, targetPrice)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.rules);
        handleChangedShop(shop);
      })
      .catch((e) => {
        alert(e);
      });
  };

  const handleAddCompositeRule = (operator: string, rulesIds: string[]) => {
    const rules: number[] = [];
    rulesIds.forEach((ruleId: string) => {
      rules.push(parseInt(ruleId));
    });
    AddCompositeRule(shop.id, parseInt(operator), rules)
      .then(() => serverGetShopInfo(shop.id))
      .then((newShop: Shop) => {
        setRows(newShop.rules);
        handleChangedShop(shop);
      })
      .catch((e) => {
        alert(e);
      });
  };

  const handleAddRulePop = () => {
    setOpenAddRulePop(true);
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
              disableSelectionOnClick
              getRowId={() => rowId++}
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
                  onClick={handleAddRulePop}
                >
                  Add Rule
                </Button>
                {openAddRulePop && (
                  <AddRulePop
                    handleAddSimpleRule={handleAddSimpleRule}
                    handleAddQuantityRule={handleAddQuantityRule}
                    handleAddTotalPriceRule={handleAddTotalPriceRule}
                    handleAddCompositeRule={handleAddCompositeRule}
                    open={openAddRulePop}
                    handleClose={() => setOpenAddRulePop(false)}
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
