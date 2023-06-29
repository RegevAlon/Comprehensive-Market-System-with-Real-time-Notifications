import React, { useState } from "react";
import {
  Checkbox,
  FormControlLabel,
  TextField,
  FormGroup,
  Container,
  Typography,
  Box,
  Button,
  Stack,
} from "@mui/material";
import { pathMarket, pathSearch, pathShop } from "../Paths";
import SearchIcon from "@mui/icons-material/Search";
import { serverSearchProducts } from "../Services/MarketService";
import Product from "../Objects/Product";
import Navbar from "./Navbar";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { serverAddToCart } from "../Services/MarketService";
import ShopToolbar from "./ShopToolbar";
import SuccessSnackbar from "./SuccessSnackbar";
import { Currency, squaresColor } from "../Utils";
import { useNavigate } from "react-router-dom";

type SearchType = {
  Name: boolean;
  Category: boolean;
  Keywords: boolean;
};

type FilterType = {
  ProductRating: boolean;
  PriceRange: boolean;
  Category: boolean;
};

const SearchOpen = () => {
  const columns: GridColDef[] = [
    {
      field: "name",
      headerName: "Product",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "price",
      headerName: `Price (${Currency})`,
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "quantity",
      headerName: "Quantity",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "category",
      headerName: "Category",
      flex: 1,
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
      field: "",
      headerName: "Shop",
      flex: 1.2,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => {
        return (
          <Button
            variant="outlined"
            color="primary"
            onClick={() => {
              HandleGoToShop(params.row.shopId);
            }}
          >
            Go To Shop
          </Button>
        );
      },
    },
  ];

  const [selectedSearchTypes, setSelectedSearchTypes] = useState<SearchType>({
    Name: true,
    Category: false,
    Keywords: false,
  });

  const [selectedFilterTypes, setSelectedFilterTypes] = useState<FilterType>({
    ProductRating: false,
    PriceRange: false,
    Category: false,
  });
  const [selectionModel, setSelectionModel] = React.useState<number[]>([]);
  const [selectedProductsIds, setSelectedProductsIds] = React.useState<
    number[]
  >([]);
  const [msg, setMsg] = useState<string>("");
  const [openSnack, setOpenSnack] = React.useState<boolean>(false);
  const [addToCartMsg, setAddToCartMsg] = React.useState<string>("");
  const [resultProducts, setResultProducts] = useState<Product[]>([]);
  const [lowPrice, setLowPrice] = useState<string>("");
  const [highPrice, setHighPrice] = useState<string>("");
  const [lowRate, setLowRate] = useState<string>("");
  const [highRate, setHighRate] = useState<string>("");
  const [category, setCategory] = useState<string>("");
  const navigate = useNavigate();

  const HandleGoToShop = (shopId: number) => {
    navigate(`${pathShop}?id=${shopId}`);
  };

  const updateSearchFields = () => {
    if (
      !(
        selectedSearchTypes.Name ||
        selectedSearchTypes.Keywords ||
        selectedSearchTypes.Category
      )
    ) {
      setMsg("");
    }
  };

  const updateFilterFields = () => {
    if (!selectedFilterTypes.PriceRange) {
      setLowPrice("");
      setHighPrice("");
    }
    if (!selectedFilterTypes.ProductRating) {
      setLowRate("");
      setHighRate("");
    }
    if (!selectedFilterTypes.Category) {
      setCategory("");
    }
  };

  const handleSearchTypeChange = (event: any) => {
    setSelectedSearchTypes({
      ...selectedSearchTypes,
      [event.target.name]: event.target.checked,
    });
    updateSearchFields();
  };

  const handleFilterTypeChange = (event: any) => {
    setSelectedFilterTypes({
      ...selectedFilterTypes,
      [event.target.name]: event.target.checked,
    });
    updateFilterFields();
  };

  const updateSelection = (newSelection: any) => {
    const chosenIds: number[] = newSelection;
    setSelectionModel(newSelection);
    setSelectedProductsIds(chosenIds);
  };

  const handleNewSelection = (newSelectionModel: any) => {
    updateSelection(newSelectionModel);
  };

  const searchResults = () => {
    const NO_VALUE: number = -1;
    const NAME: number = 0;
    const KEYWORDS: number = 1;
    const CATEGORY: number = 2;
    const PRICE_RANGE: number = 0;
    const PRODUCT_RATING: number = 1;
    const FILTER_CATEGORY: number = 2;
    const sTypes: number[] = [];
    const fTypes: number[] = [];

    selectedSearchTypes.Name ? sTypes.push(NAME) : sTypes.push(NO_VALUE);
    selectedSearchTypes.Keywords
      ? sTypes.push(KEYWORDS)
      : sTypes.push(NO_VALUE);
    selectedSearchTypes.Category
      ? sTypes.push(CATEGORY)
      : sTypes.push(NO_VALUE);

    selectedFilterTypes.PriceRange
      ? fTypes.push(PRICE_RANGE)
      : fTypes.push(NO_VALUE);
    selectedFilterTypes.ProductRating
      ? fTypes.push(PRODUCT_RATING)
      : fTypes.push(NO_VALUE);
    selectedFilterTypes.Category
      ? fTypes.push(FILTER_CATEGORY)
      : fTypes.push(NO_VALUE);

    serverSearchProducts(
      msg,
      sTypes,
      fTypes,
      lowPrice.length == 0 ? -1 : parseInt(lowPrice),
      highPrice.length == 0 ? -1 : parseInt(highPrice),
      lowRate.length == 0 ? -1 : parseInt(lowRate),
      highRate.length == 0 ? -1 : parseInt(highRate),
      category
    )
      .then((prods: Product[]) => {
        if (prods.length == 0) {
          alert("No products matched your criteria");
        }
        setResultProducts(prods);
      })
      .catch((e: any) => alert(e));
  };

  const handleSearch = () => {
    searchResults();
  };

  const handleFailToAdd = (products: Product[]) => {
    const failString: string =
      "Failed to add the following products to your cart:\n" +
      products.map((product: Product) => product.name);
    alert(failString);
  };

  const handleAddToCart = () => {
    const prods = resultProducts || [];
    const chosenProducts: Product[] = prods.filter((p) =>
      selectedProductsIds.includes(p.id)
    );
    chosenProducts.forEach((product: Product) => {
      serverAddToCart(product.shopId, product.id, 1)
        .then(() => {
          setOpenSnack(true);
          setAddToCartMsg(`Added ${product.name} to cart successfully`);
        })
        .catch((e) => {
          alert(e);
        });
    });
  };

  const handleLowPriceChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setLowPrice(event.currentTarget.value);
  };

  const handleHighPriceChange = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    setHighPrice(event.currentTarget.value);
  };

  const handleLowRateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setLowRate(event.currentTarget.value);
  };

  const handleHighRateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setHighRate(event.currentTarget.value);
  };

  const handleMsgChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setMsg(event.currentTarget.value);
  };

  const handleCategoryChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setCategory(event.currentTarget.value);
  };

  return (
    <Container>
      <Navbar />
      {ShopToolbar(selectedProductsIds.length, handleAddToCart)}
      {resultProducts.length !== 0 ? (
        <>
          <Box
            sx={{
              m: 2,
              boxShadow: 1,
              borderBlockColor: "black",
              borderRadius: 20,
              backgroundColor: squaresColor,
              p: 4,
            }}
          >
            <Typography variant="h4" component="h1" gutterBottom>
              Search Results
            </Typography>
            <DataGrid
              rows={resultProducts}
              columns={columns}
              sx={{
                width: "60vw",
              }}
              pagination
              checkboxSelection
              disableRowSelectionOnClick
              onRowSelectionModelChange={handleNewSelection}
              isCellEditable={() => false}
            />
          </Box>
          <Stack
            direction={"row"}
            justifyContent="space-between"
            width={"62vw"}
          >
            <Box textAlign="center">
              <Button
                href={pathSearch}
                variant="contained"
                sx={{ mt: 3, ml: 1 }}
              >
                Back To Search
              </Button>
            </Box>
            <Box textAlign="center">
              <Button
                href={pathMarket}
                variant="contained"
                sx={{ mt: 3, ml: 1 }}
              >
                Back To Market
              </Button>
            </Box>
          </Stack>
          {SuccessSnackbar(addToCartMsg, openSnack, () => setOpenSnack(false))}
        </>
      ) : (
        <>
          <Typography variant="h4" component="h1" gutterBottom>
            Search Page
          </Typography>
          <Typography variant="h5" component="h2">
            Search Type:
          </Typography>
          <FormGroup>
            {Object.keys(selectedSearchTypes).map((key) => (
              <FormControlLabel
                control={
                  <Checkbox
                    checked={selectedSearchTypes[key as keyof SearchType]}
                    onChange={handleSearchTypeChange}
                    name={key.toString()}
                  />
                }
                label={key}
                key={key}
              />
            ))}
          </FormGroup>
          <TextField
            label="Search..."
            variant="outlined"
            className="customTextField"
            sx={{ mt: 3, ml: 1 }}
            onChange={handleMsgChange}
          />
          <Typography variant="h5" component="h2" sx={{ mt: 2, ml: 1 }}>
            Filter Type:
          </Typography>
          <FormGroup>
            {Object.keys(selectedFilterTypes).map((key) => (
              <FormControlLabel
                control={
                  <Checkbox
                    checked={selectedFilterTypes[key as keyof FilterType]}
                    onChange={handleFilterTypeChange}
                    name={key}
                  />
                }
                label={key}
                key={key}
              />
            ))}
          </FormGroup>
          <Stack direction={"column"}>
            {selectedFilterTypes.PriceRange && (
              <Stack direction={"row"}>
                <TextField
                  label="Low Price"
                  variant="outlined"
                  className="customTextField"
                  sx={{ mt: 3, ml: 1 }}
                  onChange={handleLowPriceChange}
                />
                <TextField
                  label="High Price"
                  variant="outlined"
                  className="customTextField"
                  sx={{ mt: 3, ml: 1 }}
                  onChange={handleHighPriceChange}
                />
              </Stack>
            )}
            {selectedFilterTypes.ProductRating && (
              <Stack direction={"row"}>
                <TextField
                  label="Low Rate"
                  variant="outlined"
                  className="customTextField"
                  sx={{ mt: 3, ml: 1 }}
                  onChange={handleLowRateChange}
                />
                <TextField
                  label="High Rate"
                  variant="outlined"
                  className="customTextField"
                  sx={{ mt: 3, ml: 1 }}
                  onChange={handleHighRateChange}
                />
              </Stack>
            )}
            {selectedFilterTypes.Category && (
              <TextField
                label="Category"
                variant="outlined"
                className="customTextField"
                sx={{ mt: 3, ml: 1 }}
                onChange={handleCategoryChange}
              />
            )}
          </Stack>
          <Stack direction={"column"}>
            <Box textAlign="center" display="flex" justifyContent="center">
              <Button
                variant="outlined"
                color="inherit"
                startIcon={<SearchIcon />}
                type="submit"
                sx={{ mt: 3 }}
                onClick={handleSearch}
              >
                Search
              </Button>
            </Box>
          </Stack>
          <Box textAlign="center">
            <Button href={pathMarket} variant="contained" sx={{ mt: 3, ml: 1 }}>
              Back To Market
            </Button>
          </Box>
        </>
      )}
    </Container>
  );
};

export default SearchOpen;
