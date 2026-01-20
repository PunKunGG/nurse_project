/**
 * User Info Loader
 * Shared script to load and display user info across all pages
 * Uses Supabase auth/profiles for consistent data
 */

import { getSession, getMyProfile, signOut } from "./auth.js";

// DOM Elements
const meEl = document.getElementById("me");
const meSubEl = document.getElementById("me_sub");
const logoutBtn = document.getElementById("btn_logout");
const teacherTools = document.getElementById("teacher_tools");

/**
 * Initialize user info display
 */
async function initUserInfo() {
  try {
    const session = await getSession();
    if (!session) {
      window.location.href = "login.html";
      return;
    }

    // Try to get profile from Supabase
    const profile = await getMyProfile();

    if (profile) {
      // Show profile data from Supabase
      if (meEl) {
        meEl.innerHTML = `<b>${profile.full_name}</b>`;
      }
      if (meSubEl) {
        meSubEl.textContent = `${profile.student_id || ""} | ${profile.role || "student"}`;
      }

      // Show teacher tools if applicable
      if (
        teacherTools &&
        (profile.role === "teacher" || profile.role === "admin")
      ) {
        teacherTools.style.display = "block";
      }
    } else {
      // Fallback to session user info
      if (meEl) {
        meEl.textContent =
          session.user.user_metadata?.full_name ||
          session.user.email.split("@")[0];
      }
      if (meSubEl) {
        meSubEl.textContent = session.user.email;
      }
    }
  } catch (error) {
    console.error("Error loading user info:", error);
    // Fallback display
    if (meEl) {
      meEl.textContent = "ผู้ใช้";
    }
    if (meSubEl) {
      meSubEl.textContent = "";
    }
  }
}

/**
 * Setup logout button handler
 */
function setupLogout() {
  if (logoutBtn) {
    logoutBtn.addEventListener("click", async () => {
      try {
        await signOut();
        window.location.href = "index.html";
      } catch (error) {
        console.error("Logout error:", error);
        window.location.href = "login.html";
      }
    });
  }
}

// Initialize on load
initUserInfo();
setupLogout();

// Export for use in other scripts if needed
export { initUserInfo, setupLogout };
