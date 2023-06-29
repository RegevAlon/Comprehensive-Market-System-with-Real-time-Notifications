import React, { useContext } from "react";
import { ThemeContext } from "./ThemeContext";

const ToggleThemeButton: React.FC = () => {
  const themeContext = useContext(ThemeContext);

  if (!themeContext) {
    // Handle the case when the context is null
    alert("null");
    return null;
  }

  const { theme, toggleTheme } = themeContext;

  return (
    <button onClick={toggleTheme}>
      Toggle to {theme === "light" ? "Dark" : "Light"} Mode
    </button>
  );
};

export default ToggleThemeButton;
