/**
 * User Info Loader
 * Shared script to load and display user info across all pages
 * Uses Supabase auth/profiles for consistent data
 */

import { supabase } from "./supabaseClient.js";
import { getMyProfile, signOut } from "./auth.js";

// DOM Elements
const meEl = document.getElementById("me");
const meSubEl = document.getElementById("me_sub");
const logoutBtn = document.getElementById("btn_logout");
const teacherTools = document.getElementById("teacher_tools");
const adminTools = document.getElementById("admin_tools");

// Track if we've already initialized
let initialized = false;

/**
 * Load and display user profile
 */
async function loadUserProfile(user) {
  try {
    // Try to get profile from Supabase
    const { data: profile, error } = await supabase
      .from("profiles")
      .select("full_name, student_id, role")
      .eq("user_id", user.id)
      .maybeSingle();

    if (error) {
      console.error("Error loading profile:", error);
    }

    if (profile) {
      // Show profile data from Supabase
      if (meEl) {
        meEl.innerHTML = `<b>${profile.full_name}</b>`;
      }
      if (meSubEl) {
        meSubEl.textContent = `${profile.student_id || ""} | ${profile.role || "student"}`;
      }

      // Show teacher tools if teacher or admin
      if (
        teacherTools &&
        (profile.role === "teacher" || profile.role === "admin")
      ) {
        teacherTools.style.display = "block";
      }

      // Show admin tools only for admin
      if (adminTools && profile.role === "admin") {
        adminTools.style.display = "block";
      }
    } else {
      // Fallback to user metadata
      if (meEl) {
        meEl.textContent =
          user.user_metadata?.full_name || user.email.split("@")[0];
      }
      if (meSubEl) {
        meSubEl.textContent = user.email;
      }
    }
  } catch (error) {
    console.error("Error loading user info:", error);
    // Fallback display
    if (meEl) {
      meEl.textContent = user.email?.split("@")[0] || "ผู้ใช้";
    }
    if (meSubEl) {
      meSubEl.textContent = user.email || "";
    }
  }
}

/**
 * Setup auth state listener
 */
function initAuth() {
  if (initialized) return;
  initialized = true;

  // Listen for auth state changes (handles session refresh gracefully)
  supabase.auth.onAuthStateChange((event, session) => {
    console.log("Auth state:", event);

    if (event === "SIGNED_OUT" || !session) {
      // Only redirect if truly signed out, not during token refresh
      if (event === "SIGNED_OUT") {
        window.location.href = "login.html";
      }
      return;
    }

    if (
      event === "SIGNED_IN" ||
      event === "TOKEN_REFRESHED" ||
      event === "INITIAL_SESSION"
    ) {
      loadUserProfile(session.user);
    }
  });

  // Also check initial session
  supabase.auth.getSession().then(({ data: { session } }) => {
    if (!session) {
      window.location.href = "login.html";
    } else {
      loadUserProfile(session.user);
    }
  });
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
initAuth();
setupLogout();

// Export for use in other scripts if needed
export { loadUserProfile, initAuth, setupLogout };
