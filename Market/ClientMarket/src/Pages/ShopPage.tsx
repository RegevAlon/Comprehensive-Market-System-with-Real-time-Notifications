import * as React from "react";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import { Box, Button, Stack } from "@mui/material";
import Navbar from "../Components/Navbar";
import Product from "../Objects/Product";
import Shop from "../Objects/Shop";
import { pathMarket, pathRegister } from "../Paths";
import { useLocation, useNavigate } from "react-router-dom";
import ShopToolbar from "../Components/ShopToolbar";
import {
  ApproveCounterBid,
  DeclineCounterBid,
  RemoveBid,
  addReview,
  serverAddToCart,
  serverGetShopInfo,
  setBidOnProduct,
} from "../Services/MarketService";
import LoadingCircle from "../Components/LoadingCircle";
import SuccessSnackbar from "../Components/SuccessSnackbar";

import classnames from "classnames";
import DiscountPolicyInfo from "../Objects/Policies/DiscountPolicyInfo";
import PurchasePolicyInfo from "../Objects/Policies/PurchasePolicyInfo";
import RuleInfo from "../Objects/Rules/RuleInfo";
import AddReviewPop from "../Components/Pop-ups/AddReviewPop";
import ShowReviewPop from "../Components/Pop-ups/ShowReviewPop";
import AddBidPop from "../Components/Pop-ups/AddBidPop";
import { Currency, squaresColor } from "../Utils";
import ShowCounterBidPop from "../Components/Pop-ups/ShowCounterBidPop";
import Bid from "../Objects/Bid";
import { getIsGuest, getUserName } from "../Services/SessionService";

const styles = {
  container: {
    maxWidth: "600px",
    margin: "0 auto",
    padding: "20px",
  },
  heading: {
    fontSize: "24px",
    fontWeight: "bold",
    marginBottom: "20px",
  },
  messageList: {
    listStyle: "none",
    padding: "0",
  },
  messageItem: {
    backgroundColor: squaresColor,
    padding: "10px",
    marginBottom: "10px",
    borderRadius: "4px",
  },
  loadingMessage: {
    fontStyle: "italic",
    color: "#888",
  },
};

interface ProductSettings {
  ShowCounterBid: boolean;
  ShowRemoveBid: boolean;
  CurrentBid: number;
  CounterBid: number;
}

export default function ShopPage() {
  const navigate = useNavigate();
  const [selectedProductsIds, setSelectedProductsIds] = React.useState<
    number[]
  >([]);
  const [productButtonsMap, setProductButtonsMap] = React.useState<
    Record<number, ProductSettings>
  >({});
  const [products, setProducts] = React.useState<Product[]>([]);
  const location = useLocation();
  const searchParams = new URLSearchParams(location.search);
  const id = searchParams.get("id");
  const shopId = id ? parseInt(id, 10) : undefined;
  const [openSnack, setOpenSnack] = React.useState<boolean>(false);
  const [addToCartMsg, setAddToCartMsg] = React.useState<string>("");
  const [policies, setPolicies] = React.useState<string[]>([]);
  const [rules, setRules] = React.useState<string[]>([]);
  const [openReviewPop, setOpenReviewPop] = React.useState<boolean>(false);
  const [openBidPop, setOpenBidPop] = React.useState<boolean>(false);
  const [bidProdPrice, setBidProdPrice] = React.useState<number>(0);
  const [counterBidPrice, setCounterBidPrice] = React.useState<number>(0);
  const [openShowReviewPop, setOpenShowReviewPop] =
    React.useState<boolean>(false);
  const [reviewProdId, setReviewProdId] = React.useState<number>(-1);
  const [openCounterBidPop, setOpenCounterBidPop] =
    React.useState<boolean>(false);

  const addProductSettings = (productId: number, settings: ProductSettings) => {
    setProductButtonsMap((prevMap) => ({
      ...prevMap,
      [productId]: settings,
    }));
  };

  React.useEffect(() => {
    serverGetShopInfo(shopId)
      .then((shop: Shop) => {
        setProducts(shop.products);
        const policies: string[] = [];
        shop.discountPolicies.forEach((disP: DiscountPolicyInfo) =>
          policies.push(disP.description.replace("Subject:", ""))
        );
        shop.purchasePolicies.forEach((purP: PurchasePolicyInfo) =>
          policies.push(purP.description.replace("Subject:", ""))
        );
        setPolicies(policies);
        const rules: string[] = [];
        shop.rules.forEach((rule: RuleInfo) =>
          rules.push(rule.description.replace("Subject:", ""))
        );
        setRules(rules);

        const username: string | null = getUserName();
        const bidProducts: number[] = [];
        shop.products.forEach((product: Product) => {
          product.bids.forEach((bid: Bid) => {
            let showCounter = false;
            let showRemove = false;
            let currentBidPrice = bid.suggestedPrice;
            let counterBid = -1;
            if (bid.biddingMember == username) {
              showRemove = true;
              bidProducts.push(product.id);
            }
            if (bid.biddingMember == username && bid.bidderApproved == false) {
              counterBid = bid.suggestedPrice;
              showCounter = true;
              bidProducts.push(product.id);
            }
            const settings: ProductSettings = {
              ShowCounterBid: showCounter,
              ShowRemoveBid: showRemove,
              CurrentBid: currentBidPrice,
              CounterBid: counterBid,
            };
            addProductSettings(product.id, settings);
          });
        });
      })
      .catch((e) => {
        alert(e);
        navigate(pathMarket);
      });
  }, [products]);

  const handleBackToMarket = () => {
    navigate(`${pathMarket}`);
  };

  const columns: GridColDef[] = [
    {
      field: "name",
      headerName: "Product Name",
      type: "string",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "price",
      headerName: `Price (${Currency})`,
      type: "number",
      flex: 0.9,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "quantity",
      headerName: "Quantity",
      type: "number",
      flex: 1,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "category",
      headerName: "Category",
      type: "string",
      flex: 0.8,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "rate",
      headerName: "Rating",
      type: "number",
      flex: 0.7,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "reviews",
      headerName: "Reviews",
      flex: 0.7,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => (
        <Button
          variant="outlined"
          color="primary"
          onClick={() => {
            setOpenShowReviewPop(true);
            setReviewProdId(params.row.id);
          }}
        >
          Reviews
        </Button>
      ),
    },
    {
      field: "Rate",
      flex: 1.2,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => {
        if (getIsGuest()) {
          return (
            <Button
              variant="outlined"
              color="primary"
              onClick={() => {
                handleSignUp();
              }}
            >
              Sign Up
            </Button>
          );
        } else {
          return (
            <Button
              variant="outlined"
              color="primary"
              onClick={() => {
                setOpenReviewPop(true);
                setReviewProdId(params.row.id);
              }}
            >
              Add Review
            </Button>
          );
        }
      },
    },
    {
      field: "Bid",
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => {
        if (params.row.sellType !== 1) {
          return <span>No Bid Option</span>;
        }
        if (getIsGuest()) {
          return (
            <Button
              variant="outlined"
              color="primary"
              onClick={() => {
                handleSignUp();
              }}
            >
              Sign Up
            </Button>
          );
        } else {
          return (
            <Button
              variant="outlined"
              color="primary"
              onClick={() => {
                setOpenBidPop(true);
                setReviewProdId(params.row.id);
                setBidProdPrice(params.row.price);
              }}
            >
              Add Bid
            </Button>
          );
        }
      },
    },
    {
      field: "Counter Bid",
      flex: 1.2,
      align: "center",
      headerAlign: "center",
      hideable: true,
      renderCell: (params) => {
        if (getIsGuest() && params.row.sellType === 1) {
          return (
            <Button
              variant="outlined"
              color="primary"
              onClick={() => {
                handleSignUp();
              }}
            >
              Sign Up
            </Button>
          );
        } else {
          if (params.row.sellType !== 1) {
            return null;
          }
          if (productButtonsMap[params.row.id]?.ShowCounterBid) {
            return (
              <Button
                variant="outlined"
                color="primary"
                onClick={() => {
                  setOpenCounterBidPop(true);
                  setReviewProdId(params.row.id);
                }}
              >
                Counter Bid
              </Button>
            );
          }
        }
      },
    },
    {
      field: "'",
      headerName: "Current Bid",
      flex: 1,
      align: "center",
      headerAlign: "center",
      renderCell: (params) => {
        if (getIsGuest() && params.row.sellType === 1) {
          return (
            <Button
              variant="outlined"
              color="primary"
              onClick={() => {
                handleSignUp();
              }}
            >
              Sign Up
            </Button>
          );
        } else {
          if (productButtonsMap[params.row.id]?.ShowRemoveBid) {
            return <span>{productButtonsMap[params.row.id]?.CurrentBid}</span>;
          }
        }
      },
    },
    {
      field: "Remove Bid",
      flex: 1.2,
      align: "center",
      headerAlign: "center",
      hideable: true,
      renderCell: (params) => {
        if (getIsGuest() && params.row.sellType === 1) {
          return (
            <Button
              variant="outlined"
              color="primary"
              onClick={() => {
                handleSignUp();
              }}
            >
              Sign Up
            </Button>
          );
        } else {
          if (params.row.sellType !== 1) {
            return null;
          }
          if (productButtonsMap[params.row.id]?.ShowRemoveBid) {
            return (
              <Button
                variant="outlined"
                color="error"
                onClick={() => {
                  handleRemoveBid(params.row.id);
                }}
              >
                Remove Bid
              </Button>
            );
          }
        }
      },
    },
  ];

  const handleAddReview = (rate: number, comment: string) => {
    addReview(shopId, reviewProdId, rate, comment)
      .then(() => serverGetShopInfo(shopId))
      .then((newShop: Shop) => {
        setProducts(newShop.products);
        setOpenReviewPop(false);
      })
      .catch((e) => {
        alert(e);
      });
  };

  const handleAddBid = (quantity: number, bidPrice: number) => {
    setBidOnProduct(shopId, reviewProdId, quantity, bidPrice)
      .then(() => serverGetShopInfo(shopId))
      .then((newShop: Shop) => {
        setProducts(newShop.products);
        setOpenBidPop(false);
      })
      .catch((e) => {
        alert(e);
      });
  };

  const handleApproveCounterBid = () => {
    ApproveCounterBid(shopId, reviewProdId)
      .then(() => serverGetShopInfo(shopId))
      .then((newShop: Shop) => {
        setProducts(newShop.products);
        setOpenCounterBidPop(false);
      })
      .catch((e) => {
        alert(e);
      });
  };

  const handleSignUp = () => {
    navigate(pathRegister);
  };

  const handleDeclineCounterBid = () => {
    DeclineCounterBid(shopId, reviewProdId)
      .then(() => serverGetShopInfo(shopId))
      .then((newShop: Shop) => {
        setProducts(newShop.products);
        setOpenCounterBidPop(false);
      })
      .catch((e) => {
        alert(e);
      });
    setOpenBidPop(false);
  };

  const updateSelection = (newSelection: any) => {
    const chosenIds: number[] = newSelection;
    setSelectedProductsIds(chosenIds);
  };

  const handleNewSelection = (newSelectionModel: any) => {
    updateSelection(newSelectionModel);
  };

  const handleAddToCart = () => {
    const prods = products || [];
    const chosenProducts: Product[] = prods.filter((p) =>
      selectedProductsIds.includes(p.id)
    );
    chosenProducts.forEach((product: Product) => {
      chosenProducts.length > 1
        ? alert(`Adding ${product.name} to cart`)
        : null;
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

  const handleRemoveBid = (prodId: string) => {
    RemoveBid(shopId, getUserName(), parseInt(prodId))
      .then(() => serverGetShopInfo(shopId))
      .then((newShop: Shop) => {
        alert("Removed Bid Successfully");
        setProducts(newShop.products);
      })
      .catch((e) => {
        alert(e);
      });
  };

  return products === null ? (
    LoadingCircle()
  ) : (
    <Box>
      <Navbar />
      {policies === null || policies.length === 0 ? null : (
        <div style={styles.container}>
          <h1 style={styles.heading}>Shop Policies</h1>
          <ul style={styles.messageList}>
            {policies.map((policy, index) => (
              <li
                key={index}
                style={styles.messageItem}
                className={classnames(styles.messageItem)}
              >
                {policy}
              </li>
            ))}
          </ul>
        </div>
      )}
      {rules === null || rules.length === 0 ? null : (
        <div style={styles.container}>
          <h1 style={styles.heading}>Shop Rules</h1>
          <ul style={styles.messageList}>
            {rules.map((rule, index) => (
              <li
                key={index}
                style={styles.messageItem}
                className={classnames(styles.messageItem)}
              >
                {rule}
              </li>
            ))}
          </ul>
        </div>
      )}
      {ShopToolbar(selectedProductsIds.length, handleAddToCart)}
      <Box
        sx={{
          width: "110%",
          boxShadow: 1,
          borderRadius: 4,
          p: 3,
          backgroundColor: squaresColor,
        }}
      >
        <Stack direction="row">{}</Stack>
        <Box height="50vh" width="100%">
          <Box display="flex" height="100%">
            <Box flexGrow={1}>
              <DataGrid
                rows={products}
                columns={columns}
                sx={{
                  width: "80vw",
                }}
                pagination
                checkboxSelection
                disableRowSelectionOnClick
                onRowSelectionModelChange={handleNewSelection}
              />
              <Button
                variant="contained"
                onClick={handleBackToMarket}
                sx={{ mt: 8, ml: 1 }}
              >
                Back To Market
              </Button>
            </Box>
          </Box>
        </Box>
      </Box>
      {openReviewPop && (
        <AddReviewPop
          handleAddReview={handleAddReview}
          open={openReviewPop}
          handleClose={() => setOpenReviewPop(false)}
        />
      )}
      {openShowReviewPop && (
        <ShowReviewPop
          open={openShowReviewPop}
          prodId={reviewProdId}
          shopId={shopId}
          handleClose={() => setOpenShowReviewPop(false)}
        />
      )}
      {openBidPop && (
        <AddBidPop
          handleAddBid={handleAddBid}
          open={openBidPop}
          currPrice={bidProdPrice}
          handleClose={() => setOpenBidPop(false)}
        />
      )}
      {openCounterBidPop && (
        <ShowCounterBidPop
          handleApproveCounterBid={handleApproveCounterBid}
          handleDeclineCounterBid={handleDeclineCounterBid}
          open={openCounterBidPop}
          counterBidPrice={productButtonsMap[reviewProdId]?.CurrentBid}
          handleClose={() => setOpenCounterBidPop(false)}
        />
      )}
      {SuccessSnackbar(addToCartMsg, openSnack, () => setOpenSnack(false))}
    </Box>
  );
}
