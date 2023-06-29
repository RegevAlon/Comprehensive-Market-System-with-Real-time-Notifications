import "./App.css";
import HomePage from "./Pages/HomePage.tsx";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Register from "./Pages/Register";
import Login from "./Pages/Login";
import * as Path from "./Paths";
import Market from "./Pages/Market";
import Cart from "./Pages/CartPage";
import Shop from "./Pages/ShopPage";
import Profile from "./Pages/Profile";
import Checkout from "./Pages/Checkout";
import Notifications from "./Pages/Notifications";
import SearchOpen from "./Components/SearchOpen";
import { initSession } from "./Services/SessionService";
import AdminPage from "./Pages/AdminPage.tsx";

const App = () => {
  initSession();
  return (
    <Router>
      <Routes>
        <Route path={Path.pathHome} element={<HomePage />} />
        <Route path={Path.pathLogin} element={<Login />} />
        <Route path={Path.pathRegister} element={<Register />} />
        <Route path={Path.pathMarket} element={<Market />} />
        <Route path={Path.pathShop} element={<Shop />} />
        <Route path={Path.pathCart} element={<Cart />} />
        <Route path={Path.pathProfile} element={<Profile />} />
        <Route path={Path.pathCheckout} element={<Checkout />} />
        <Route path={Path.pathSearch} element={<SearchOpen />} />
        <Route path={Path.pathNotifications} element={<Notifications />} />
        <Route path={Path.pathAdmin} element={<AdminPage />} />
      </Routes>
    </Router>
  );
};

export default App;
