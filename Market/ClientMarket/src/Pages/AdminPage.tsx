import React, { useState, useEffect } from "react";
import {
  cancelMembership,
  getActiveMembers,
  getAllMembers,
} from "../Services/MarketService";
import { fetchResponse } from "../Services/GeneralService";
import "../AdminPage.css";
import Navbar from "../Components/Navbar";
import { Button } from "@mui/material";
import { pathMarket } from "../Paths";
import { useNavigate } from "react-router-dom";

const AdminPage: React.FC = () => {
  const [members, setMembers] = useState<string[]>([]);
  const [activeUsers, setActiveUsers] = useState<string[]>([]);
  const [numOfActiveUsers, setNumOfActiveUsers] = useState<number>(0);
  const [numOfUsers, setNumOfUsers] = useState<number>(0);
  const navigate = useNavigate();

  useEffect(() => {
    fetchResponse(getAllMembers())
      .then((allMembers: string[]) => {
        setMembers(allMembers);
        setNumOfUsers(allMembers.length);
      })
      .catch((e: any) => alert(e));

    fetchResponse(getActiveMembers())
      .then((activeUsers: string[]) => {
        setActiveUsers(activeUsers);
        setNumOfActiveUsers(activeUsers.length);
      })
      .catch((e: any) => alert(e));
  }, []);

  const removeMember = (usernameToRemove: string) => {
    cancelMembership(usernameToRemove)
      .then(() => {
        fetchResponse(getAllMembers()).then((allMembers: string[]) => {
          setMembers(allMembers);
          setNumOfUsers(allMembers.length);
        });

        fetchResponse(getActiveMembers()).then((activeUsers: string[]) => {
          setActiveUsers(activeUsers);
          setNumOfActiveUsers(activeUsers.length);
        });
      })
      .catch((e: any) => alert(e));
  };

  const handleBackToMarket = () => {
    navigate(pathMarket);
  };

  return (
    <>
      <Navbar />
      <div className="admin-page">
        <h1 className="admin-page__title">Admin Page</h1>
        {/* Members List */}
        <h2 className="admin-page__subtitle">All Members</h2>
        <ul className="admin-page__list">
          {members.map((userName) => (
            <li key={userName} className="admin-page__list-item">
              <span className="admin-page__member-name">{userName}</span>
              <button
                className="admin-page__remove-button"
                onClick={() => removeMember(userName)}
              >
                Remove Member
              </button>
            </li>
          ))}
        </ul>
        {/* Active Users List */}
        <h2 className="admin-page__subtitle">
          Active Users: {numOfActiveUsers}/{numOfUsers}
        </h2>
        <ul className="admin-page__list">
          {activeUsers.map((userName) => (
            <li key={userName} className="admin-page__list-item">
              <span className="admin-page__member-name">● {userName}</span>
            </li>
          ))}
        </ul>
      </div>
      <Button
        variant="contained"
        onClick={handleBackToMarket}
        sx={{ mt: 8, ml: 1 }}
      >
        Back To Market
      </Button>
    </>
  );
};

export default AdminPage;
