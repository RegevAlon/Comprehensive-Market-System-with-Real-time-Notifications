import React, { useState } from "react";
import { fetchResponse } from "../Services/GeneralService";
import { Box, Button, Dialog, Stack, Switch, Typography } from "@mui/material";
import Navbar from "../Components/Navbar";
import classnames from "classnames";
import {
  getMessages,
  notificationOff,
  notificationOn,
} from "../Services/MarketService";
import { pathMarket } from "../Paths";
import { notificationFlag, setNotificationFlag, squaresColor } from "../Utils";
import FailureSnackbar from "../Components/FailureSnackbar";
import SuccessSnackbar from "../Components/SuccessSnackbar";

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
    borderRadius: "100px",
    borderColor: "black",
  },
  loadingMessage: {
    fontStyle: "italic",
    color: "#888",
  },
};

const Notification: React.FC = () => {
  const [loading, setLoading] = useState<boolean>(true);
  const [notifications, setNotifications] = useState<string[]>([]);
  const [openFailSnack, setOpenFailSnack] = React.useState<boolean>(false);
  const [openSuccSnack, setOpenSuccSnack] = React.useState<boolean>(false);
  const [successMsg, setSuccessMsg] = React.useState<string>("");
  const [failureMsg, setFailureMsg] = React.useState<string>("");

  React.useEffect(() => {
    fetchResponse(getMessages())
      .then((messages: string[]) => {
        setNotifications(messages);
        setLoading(false);
      })
      .catch((e: any) => {
        alert(e);
        setLoading(false);
      });
  }, []);

  const showSuccessSnack = (msg: string) => {
    setOpenSuccSnack(true);
    setSuccessMsg(msg);
  };

  const showFailureSnack = (msg: string) => {
    setOpenFailSnack(true);
    setFailureMsg(msg);
  };

  const handleChangeNotificationMode = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const notificationMode: boolean = event.target.checked;
    if (notificationMode)
      notificationOn()
        .then(() => {
          setNotificationFlag(true);
          showSuccessSnack("Notifications Turned On Successfully");
        })
        .catch((e) => {
          showFailureSnack("Could not turn notifications on");
          alert(e);
        });
    else
      notificationOff()
        .then(() => {
          setNotificationFlag(false);
          showSuccessSnack("Notifications Turned Off Successfully");
        })
        .catch((e) => {
          showFailureSnack("Could not turn notifications off");
          alert(e);
        });
  };

  return (
    <>
      <Box>
        <Navbar />
      </Box>

      {notifications === null || notifications.length === 0 ? (
        <>
          <h1 style={styles.heading}>No Notifications In Your Inbox</h1>
          <Stack sx={{ ml: 3 }} direction="row" spacing={1} alignItems="center">
            <Typography>Allow Notifications</Typography>
            <Switch
              checked={notificationFlag}
              disabled={false}
              onChange={handleChangeNotificationMode}
            />
          </Stack>
        </>
      ) : (
        <>
          <h1 style={styles.heading}>Notifications</h1>
          <Stack sx={{ ml: 3 }} direction="row" spacing={1} alignItems="center">
            <Typography>Allow Notifications</Typography>
            <Switch
              checked={notificationFlag}
              disabled={false}
              onChange={handleChangeNotificationMode}
            />
          </Stack>
          <div style={styles.container}>
            {loading ? (
              <p style={styles.loadingMessage}>Loading messages...</p>
            ) : (
              <ul style={styles.messageList}>
                {notifications.map((message, index) => (
                  <li
                    key={index}
                    style={styles.messageItem}
                    className={classnames(styles.messageItem)}
                  >
                    {index + 1}. {message}
                  </li>
                ))}
              </ul>
            )}
          </div>
        </>
      )}
      <Box textAlign="center">
        <Button href={pathMarket} variant="contained" sx={{ mt: 3, ml: 1 }}>
          Back To Market
        </Button>
      </Box>
      <Dialog open={openFailSnack}>
        {FailureSnackbar(failureMsg, openFailSnack, () =>
          setOpenFailSnack(false)
        )}
      </Dialog>
      {SuccessSnackbar(successMsg, openSuccSnack, () =>
        setOpenSuccSnack(false)
      )}
    </>
  );
};

export default Notification;
