import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogTitle from "@mui/material/DialogTitle";
import { Currency } from "../../Utils";

export default function ShowCounterBidPop({
  handleApproveCounterBid,
  handleDeclineCounterBid,
  open,
  counterBidPrice,
  handleClose,
}: {
  handleApproveCounterBid: () => void;
  handleDeclineCounterBid: () => void;
  open: boolean;
  counterBidPrice: number;
  handleClose: () => void;
}) {
  const handleCloseClick = () => {
    handleClose();
  };

  return (
    <div>
      <Dialog open={open} onClose={handleCloseClick} fullWidth>
        <DialogTitle>
          Counter Bid Price Is: {counterBidPrice} {Currency}
        </DialogTitle>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleApproveCounterBid}>Approve</Button>
          <Button onClick={handleDeclineCounterBid}>Decline</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
