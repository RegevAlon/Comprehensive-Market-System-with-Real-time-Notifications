import React from "react";
import ReactDOM from "react-dom/client";
// import ReactDOM from "react-dom";
import App from "./App.tsx";
import { ThemeProvider } from "./Components/ThemeContext";
import "./styles.css";

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);

// ReactDOM.render(
//   <ThemeProvider>
//     <App />
//   </ThemeProvider>,
//   document.getElementById("root")
// );
