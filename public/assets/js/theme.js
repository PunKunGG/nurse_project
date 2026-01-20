/**
 * Theme Toggle - Dark/Light Mode
 * Saves preference to localStorage
 */

(function () {
  const STORAGE_KEY = "nurse-theme";

  // Get saved theme or prefer system
  function getSavedTheme() {
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved) return saved;

    // Check system preference
    if (
      window.matchMedia &&
      window.matchMedia("(prefers-color-scheme: dark)").matches
    ) {
      return "dark";
    }
    return "light";
  }

  // Apply theme
  function applyTheme(theme) {
    document.body.classList.toggle("dark", theme === "dark");
    updateToggleIcon();
  }

  // Update toggle button icon
  function updateToggleIcon() {
    const toggle = document.querySelector(".theme-toggle");
    if (toggle) {
      const isDark = document.body.classList.contains("dark");
      toggle.textContent = isDark ? "☀️" : "🌙";
      toggle.setAttribute(
        "aria-label",
        isDark ? "Switch to light mode" : "Switch to dark mode",
      );
    }
  }

  // Toggle theme
  function toggleTheme() {
    const isDark = document.body.classList.contains("dark");
    const newTheme = isDark ? "light" : "dark";
    localStorage.setItem(STORAGE_KEY, newTheme);
    applyTheme(newTheme);
  }

  // Initialize
  function init() {
    // Apply saved theme immediately
    applyTheme(getSavedTheme());

    // Create toggle button if not exists
    if (!document.querySelector(".theme-toggle")) {
      const toggle = document.createElement("button");
      toggle.className = "theme-toggle";
      toggle.type = "button";
      toggle.setAttribute("aria-label", "Toggle dark mode");
      document.body.appendChild(toggle);
      updateToggleIcon();
    }

    // Add click handler
    document
      .querySelector(".theme-toggle")
      ?.addEventListener("click", toggleTheme);

    // Listen for system preference changes
    window
      .matchMedia("(prefers-color-scheme: dark)")
      .addEventListener("change", (e) => {
        if (!localStorage.getItem(STORAGE_KEY)) {
          applyTheme(e.matches ? "dark" : "light");
        }
      });
  }

  // Run when DOM is ready
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
