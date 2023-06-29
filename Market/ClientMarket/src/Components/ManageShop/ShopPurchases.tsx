import * as React from "react";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { Box, Stack } from "@mui/material";
import Shop from "../../Objects/Shop";
import Purchase from "../../Objects/Purchase";
import { showShopPurchaseHistory } from "../../Services/MarketService";
import { Currency, squaresColor, textColor } from "../../Utils";

const columns: GridColDef[] = [
  {
    field: "id",
    headerName: "ID",
    type: "number",
    flex: 0.5,
    align: "center",
    headerAlign: "center",
  },
  {
    field: "price",
    headerName: `Price (${Currency})`,
    type: "number",
    flex: 1,
    align: "center",
    headerAlign: "center",
  },
  {
    field: "purchaseStatus",
    headerName: "Purchase Status",
    type: "string",
    flex: 1,
    align: "center",
    headerAlign: "center",
  },
];

export default function ShopPurchases({
  shop,
  handleChangedShop,
}: {
  shop: Shop;
  handleChangedShop: (s: Shop) => void;
}) {
  const initSize: number = 5;

  const [pageSize, setPageSize] = React.useState<number>(initSize);
  const [rows, setRows] = React.useState<Purchase[]>([]);

  React.useEffect(() => {
    showShopPurchaseHistory(shop.id)
      .then((purchases: Purchase[]) => setRows(purchases))
      .catch((e) => {
        alert(e);
      });
  }, [shop]);

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
              disableSelectionOnClick
            />
          </div>
        </div>
      </div>
    </Box>
  );
}
