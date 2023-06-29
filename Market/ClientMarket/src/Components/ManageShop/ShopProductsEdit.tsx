import * as React from "react";
import { DataGrid, GridEditCellProps, GridColDef } from "@mui/x-data-grid";
import { Box, Button, Dialog, Stack, Typography } from "@mui/material";
import Product from "../../Objects/Product";
import Shop from "../../Objects/Shop";
import { fetchResponse } from "../../Services/GeneralService";
import FailureSnackbar from "../FailureSnackbar";
import SuccessSnackbar from "../SuccessSnackbar";
import AddProductPop from "../Pop-ups/AddProductPop";
import {
  ServerUpdateProductPrice,
  removeProduct,
  serverAddProduct,
  serverGetShopInfo,
  updateProductName,
  updateProductQuantity,
} from "../../Services/MarketService";
import ShowReviewPop from "../Pop-ups/ShowReviewPop";
import { Currency, squaresColor, textColor } from "../../Utils";

export default function ShopProductsEdit({
  shop,
  handleChangedShop,
}: {
  shop: Shop;
  handleChangedShop: (s: Shop) => void;
}) {
  const initSize: number = 5;

  const [pageSize, setPageSize] = React.useState<number>(initSize);
  const [rows, setRows] = React.useState<Product[]>([]);
  const [openProductForm, setOpenProductForm] = React.useState<boolean>(false);
  const [openFailSnack, setOpenFailSnack] = React.useState<boolean>(false);
  const [failureProductMsg, setFailureProductMsg] = React.useState<string>("");
  const [openSuccSnack, setOpenSuccSnack] = React.useState<boolean>(false);
  const [successProductMsg, setSuccessProductMsg] = React.useState<string>("");
  const [selectionModel, setSelectionModel] = React.useState<number[]>([]);
  const [chosenIds, setChosenIds] = React.useState<number[]>([]);
  const [openShowReviewPop, setOpenShowReviewPop] =
    React.useState<boolean>(false);
  const [reviewProdId, setReviewProdId] = React.useState<number>(-1);

  const columns: GridColDef[] = [
    {
      field: "name",
      headerName: "Product Name",
      type: "string",
      flex: 1,
      editable: true,
      align: "center",
      headerAlign: "center",
      headerClassName: "header-class",
      cellClassName: "cell-class",
    },
    {
      field: "price",
      headerName: `Price (${Currency})`,
      type: "number",
      flex: 1,
      editable: true,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "quantity",
      headerName: "Available Quantity",
      description: "Product current quantity in shop inventory",
      type: "number",
      flex: 1,
      editable: true,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "description",
      headerName: "Description",
      type: "number",
      flex: 1,
      editable: false,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "category",
      headerName: "Category",
      type: "string",
      flex: 1,
      editable: false,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "rate",
      headerName: "Rating",
      type: "number",
      flex: 1,
      editable: false,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "Sell Type",
      headerName: "sellType",
      flex: 1.2,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <span>{params.row.sellType ? "Bid" : "Regular"}</span>
      ),
    },
    {
      field: "reviews",
      headerName: "Reviews",
      flex: 1.2,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Button
          variant="contained"
          color="primary"
          onClick={() => {
            setOpenShowReviewPop(true);
            setReviewProdId(params.row.id);
          }}
        >
          Show Reviews
        </Button>
      ),
    },
  ];

  const showSuccessSnack = (msg: string) => {
    setOpenSuccSnack(true);
    setSuccessProductMsg(msg);
    setOpenProductForm(false);
  };

  const showFailureSnack = (msg: string) => {
    setOpenFailSnack(true);
    setFailureProductMsg(msg);
  };

  React.useEffect(() => {
    setRows(shop.products);
  }, [shop.products]);

  function updateAvailableQuantity(product: Product, newQuantity: number) {
    fetchResponse(updateProductQuantity(shop.id, product.id, newQuantity))
      .then(() => serverGetShopInfo(shop.id))
      .then((shop: Shop) => {
        setRows(shop.products);
        handleChangedShop(shop);
        showSuccessSnack(
          `Changed ${product.name} amount in inventory to ${newQuantity}`
        );
      })
      .catch((e) => {
        showFailureSnack(e);
      });
  }

  const updateProductPrice = (product: Product, newPrice: number) => {
    fetchResponse(ServerUpdateProductPrice(shop.id, product.id, newPrice))
      .then(() => serverGetShopInfo(shop.id))
      .then((shop: Shop) => {
        setRows(shop.products);
        handleChangedShop(shop);
        showSuccessSnack(`Changed ${product.name} price to ${newPrice}`);
      })
      .catch((e) => {
        showFailureSnack(e);
      });
  };

  const handleUpdateProductName = (product: Product, newName: string) => {
    fetchResponse(updateProductName(shop.id, product.id, newName))
      .then(() => serverGetShopInfo(shop.id))
      .then((shop: Shop) => {
        setRows(shop.products);
        handleChangedShop(shop);
        showSuccessSnack(`Changed ${product.name} name to ${newName}`);
      })
      .catch((e) => {
        showFailureSnack(e);
      });
  };

  const handleCellEditStop = (
    params: GridEditCellProps,
    event: React.SyntheticEvent
  ) => {
    rows.map((row) => {
      if (row.id === params.id) {
        if (params.field === "quantity")
          updateAvailableQuantity(
            row,
            parseInt((event.target as HTMLInputElement).value)
          );
        if (params.field === "price")
          updateProductPrice(
            row,
            parseInt((event.target as HTMLInputElement).value)
          );
        if (params.field === "name")
          handleUpdateProductName(
            row,
            (event.target as HTMLInputElement).value
          );
      }
    });
    // setRows(updatedRows);
    // console.log("Cell editing stopped", params);
  };

  const handleChangedProducts = (changedShop: Shop) => {
    serverGetShopInfo(changedShop.id)
      .then((shop: Shop) => {
        setRows(shop.products);
      })
      .catch((e: any) => {
        showFailureSnack(e);
      });
  };

  const handleSelectionChanged = (newSelection: any) => {
    console.log(newSelection);
    const chosenIds: number[] = newSelection;
    setSelectionModel(newSelection);
    setChosenIds(chosenIds);
  };

  const handleAddProduct = (
    productName: string,
    price: number,
    quantity: number,
    category: string,
    description: string,
    keyWords: string[],
    setBid: number
  ) => {
    fetchResponse(
      serverAddProduct(
        shop.id,
        productName,
        setBid,
        description,
        price,
        quantity,
        category,
        keyWords
      )
    )
      .then(() => {
        handleChangedProducts(shop);
        handleChangedShop(shop);
        showSuccessSnack("Added " + productName + " Successfully");
      })
      .catch(showFailureSnack);
  };

  const handleRemoveProduct = () => {
    chosenIds.forEach((prodId: number) => {
      fetchResponse(removeProduct(shop.id, prodId))
        .then(() => {
          handleChangedProducts(shop);
          handleChangedShop(shop);
          showSuccessSnack("Removed Successfully");
        })
        .catch(showFailureSnack);
    });
  };

  const handleRemove = async () => {
    handleRemoveProduct();
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
              // onCellEditStop={(params: any) => handleCellEdit(params)}
              onCellEditStop={handleCellEditStop}
              disableSelectionOnClick
              checkboxSelection
              onRowSelectionModelChange={handleSelectionChanged}
            />
            <Stack
              direction="row"
              justifyContent="space-between"
              width={"95vw"}
            >
              <AddProductPop
                handleAddProduct={handleAddProduct}
                open={openProductForm}
                handleClose={() => setOpenProductForm(false)}
                handleOpen={() => setOpenProductForm(true)}
              />
              <Box>
                <Button
                  sx={{ mt: 4 }}
                  color="error"
                  variant="contained"
                  disabled={chosenIds.length === 0}
                  onClick={handleRemove}
                >
                  Remove Selected Products
                </Button>
              </Box>
            </Stack>
          </div>
        </div>
      </div>
      {openShowReviewPop && (
        <ShowReviewPop
          open={openShowReviewPop}
          prodId={reviewProdId}
          shopId={shop.id}
          handleClose={() => setOpenShowReviewPop(false)}
        />
      )}
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
