/**
 * Menu Lock System
 * Manages menu item visibility based on user progress
 *
 * Unlock Flow:
 * 1. Initial: Play Game, Post-Test, Certificate are locked (by default in HTML)
 * 2. After Pre-test: Unlock Play Game and Post-Test
 * 3. After Post-test pass (≥60%): Unlock Certificate
 */

import { supabase } from "./supabaseClient.js";
import { getSession } from "./auth.js";

const MODULE_ID = "module-1";

/**
 * Check user's progress from Supabase
 * @returns {Promise<{hasCompletedPretest: boolean, hasPassedPosttest: boolean}>}
 */
async function checkUserProgress() {
  const session = await getSession();

  if (!session) {
    return { hasCompletedPretest: false, hasPassedPosttest: false };
  }

  const userId = session.user.id;

  // Check Pre-test completion
  const { data: pretestAttempts } = await supabase
    .from("attempts")
    .select("id")
    .eq("user_id", userId)
    .eq("module_id", MODULE_ID)
    .eq("test_type", "pre")
    .limit(1);

  const hasCompletedPretest = pretestAttempts && pretestAttempts.length > 0;

  // Check Post-test pass (≥60%)
  const { data: posttestAttempts } = await supabase
    .from("attempts")
    .select("score_percent")
    .eq("user_id", userId)
    .eq("module_id", MODULE_ID)
    .eq("test_type", "post")
    .gte("score_percent", 60)
    .limit(1);

  const hasPassedPosttest = posttestAttempts && posttestAttempts.length > 0;

  return { hasCompletedPretest, hasPassedPosttest };
}

/**
 * Handle click on locked menu item
 * @param {Event} e
 */
function handleLockedClick(e) {
  e.preventDefault();
  e.stopPropagation();

  const tooltip = e.currentTarget.getAttribute("data-tooltip");
  if (tooltip) {
    showToast(tooltip, "warning");
  }
}

/**
 * Initialize locked menu items (remove href, add click handler)
 * Called immediately when script loads
 */
function initLockedMenuItems() {
  const lockedItems = document.querySelectorAll(".menu-item.locked");
  lockedItems.forEach((item) => {
    // Store and remove href to prevent navigation
    if (item.href) {
      item.setAttribute("data-original-href", item.href);
      item.removeAttribute("href");
    }
    item.setAttribute("aria-disabled", "true");
    item.addEventListener("click", handleLockedClick);
  });
}

/**
 * Unlock a menu item
 * @param {HTMLElement} menuItem
 */
function unlockMenuItem(menuItem) {
  if (!menuItem) return;

  menuItem.classList.remove("locked");
  menuItem.removeAttribute("data-tooltip");
  menuItem.removeAttribute("aria-disabled");

  // Restore original href
  const originalHref = menuItem.getAttribute("data-original-href");
  if (originalHref) {
    menuItem.href = originalHref;
    menuItem.removeAttribute("data-original-href");
  }

  menuItem.removeEventListener("click", handleLockedClick);
}

/**
 * Show toast notification
 * @param {string} message
 * @param {string} type - "success" | "warning" | "error"
 */
function showToast(message, type = "info") {
  // Remove existing toast
  const existingToast = document.querySelector(".menu-lock-toast");
  if (existingToast) existingToast.remove();

  const toast = document.createElement("div");
  toast.className = `menu-lock-toast menu-lock-toast--${type}`;
  toast.innerHTML = `
    <span class="toast-icon">${type === "warning" ? "🔒" : type === "success" ? "🔓" : "ℹ️"}</span>
    <span class="toast-message">${message}</span>
  `;

  document.body.appendChild(toast);

  // Trigger animation
  requestAnimationFrame(() => {
    toast.classList.add("show");
  });

  // Auto remove after 3 seconds
  setTimeout(() => {
    toast.classList.remove("show");
    setTimeout(() => toast.remove(), 300);
  }, 3000);
}

/**
 * Apply menu unlocks based on user progress
 * Should be called on page load
 */
export async function applyMenuLocks() {
  try {
    const { hasCompletedPretest, hasPassedPosttest } =
      await checkUserProgress();

    // Get menu items by data-menu-id
    const gameMenuItem = document.querySelector('[data-menu-id="game"]');
    const posttestMenuItem = document.querySelector(
      '[data-menu-id="posttest"]',
    );
    const certificateMenuItem = document.querySelector(
      '[data-menu-id="certificate"]',
    );

    // Unlock Play Game and Post-Test if pre-test completed
    if (hasCompletedPretest) {
      unlockMenuItem(gameMenuItem);
      unlockMenuItem(posttestMenuItem);
    }

    // Unlock Certificate if post-test passed
    if (hasPassedPosttest) {
      unlockMenuItem(certificateMenuItem);
    }

    console.log("[MenuLock] Progress:", {
      hasCompletedPretest,
      hasPassedPosttest,
    });
  } catch (error) {
    console.error("[MenuLock] Error checking progress:", error);
  }
}

// Initialize locked items immediately (prevent clicks during page load)
initLockedMenuItems();

// Apply unlocks based on user progress when DOM is ready
if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", applyMenuLocks);
} else {
  applyMenuLocks();
}
