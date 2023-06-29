import * as React from "react";
import Button from "@mui/material/Button";
import TextField from "@mui/material/TextField";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { makeSetStateFromEvent } from "../../Utils";
import { getRoleString } from "../../Objects/Role";
import { permissionToString } from "../../Objects/Permission";
import InfoIcon from "@mui/icons-material/Info";
import { Tooltip } from "@mui/material";

export default function AppointPop({
  handleAppoint,
  open,
  handleClose,
}: {
  handleAppoint: (
    appointeeUsername: string,
    role: string,
    permission: string
  ) => void;
  open: boolean;
  handleClose: () => void;
}) {
  const [appointeeName, setName] = React.useState<string>("");
  const [role, setRole] = React.useState<string>("");
  const [permission, setPermission] = React.useState<string>("");

  const rolesCount: number = 3;
  const permissionsCount: number = 9;

  const getRoles = () => {
    let allRoles: string = "";
    for (let i = 0; i < rolesCount; i++) {
      allRoles += getRoleString(i) + `(${i + 1}) `;
    }
    return allRoles;
  };
  const getPermissions = () => {
    let allPermissions: string = "";
    for (let i = 0; i < permissionsCount; i++) {
      allPermissions += permissionToString(i) + `(${i + 1})\n`;
    }
    return allPermissions;
  };

  const roles: string = getRoles();
  const permissions: string = getPermissions();

  const resetFields = () => {
    setName("");
    setRole("");
    setPermission("");
  };

  const handleCloseClick = () => {
    resetFields();
    handleClose();
  };

  const handleSubmit = async () => {
    handleAppoint(appointeeName, role, permission);
    handleCloseClick();
  };

  const makeTextField = (
    id: string,
    label: string,
    value: string,
    type: "text",
    setValue: any
  ) => {
    return (
      <>
        <TextField
          autoFocus
          margin="dense"
          id={id}
          label={label}
          type={type}
          value={value}
          onChange={makeSetStateFromEvent(setValue)}
          fullWidth
          variant="standard"
        />
        {id == "role" && (
          <Tooltip title={roles}>
            <InfoIcon />
          </Tooltip>
        )}
        {id == "permission" && (
          <Tooltip title={permissions}>
            <InfoIcon />
          </Tooltip>
        )}
      </>
    );
  };

  return (
    <div>
      <Dialog open={open} onClose={handleCloseClick} fullWidth>
        <DialogTitle>Appoint Member</DialogTitle>
        <DialogContent>
          {makeTextField(
            "appointeeName",
            "Appointee Name",
            appointeeName,
            "text",
            setName
          )}
          {makeTextField("role", "Role", role, "text", setRole)}
          {makeTextField(
            "permission",
            "Permission",
            permission,
            "text",
            setPermission
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button onClick={handleSubmit}>Submit</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
