/**
 * Mobile Sidebar Toggle
 */

(function () {
  function init() {
    const sidebar = document.querySelector(".sidebar");
    if (!sidebar) return;

    // Create mobile menu toggle if not exists
    if (!document.querySelector(".mobile-menu-toggle")) {
      const toggle = document.createElement("button");
      toggle.className = "mobile-menu-toggle";
      toggle.type = "button";
      toggle.innerHTML = "☰";
      toggle.setAttribute("aria-label", "Toggle menu");
      document.body.appendChild(toggle);
    }

    // Create overlay if not exists
    if (!document.querySelector(".sidebar-overlay")) {
      const overlay = document.createElement("div");
      overlay.className = "sidebar-overlay";
      document.body.appendChild(overlay);
    }

    const toggle = document.querySelector(".mobile-menu-toggle");
    const overlay = document.querySelector(".sidebar-overlay");

    // Toggle sidebar
    function toggleSidebar() {
      sidebar.classList.toggle("open");
      overlay.classList.toggle("open");
    }

    // Close sidebar
    function closeSidebar() {
      sidebar.classList.remove("open");
      overlay.classList.remove("open");
    }

    toggle?.addEventListener("click", toggleSidebar);
    overlay?.addEventListener("click", closeSidebar);

    // Close on escape key
    document.addEventListener("keydown", (e) => {
      if (e.key === "Escape") closeSidebar();
    });

    // Close on resize to desktop
    window.addEventListener("resize", () => {
      if (window.innerWidth > 768) closeSidebar();
    });
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
