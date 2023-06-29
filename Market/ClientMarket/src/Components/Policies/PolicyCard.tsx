import {
  Box,
  Button,
  Card,
  CardContent,
  CardHeader,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";
import React from "react";
import PurchasePolicy from "../../Objects/Policies/PurchasePolicy";

export default function PolicyCard({
  id,
  purchase,
}: {
  id: number;
  purchase: PurchasePolicy;
}) {
  const [open, setOpen] = React.useState(false);
  const handleClickOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };
  return (
    <div>
      <Card elevation={1}>
        <CardHeader title={`Policy id: ${id}`} />
        <CardContent>
          <Box textAlign="center">
            <Button onClick={handleClickOpen}>detailes</Button>
          </Box>
          <Dialog
            open={open}
            onClose={handleClose}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
            fullWidth
            maxWidth="sm"
          >
            <DialogTitle id="alert-dialog-title">
              {`Policy id:  ${id}`}
            </DialogTitle>
            <DialogContent>
              <DialogContentText id="alert-dialog-description">
                {`Description:    ${purchase.toString()}`}
              </DialogContentText>
            </DialogContent>
            <DialogActions>
              <Button onClick={handleClose} autoFocus>
                Close
              </Button>
            </DialogActions>
          </Dialog>
        </CardContent>
      </Card>
    </div>
  );
}
