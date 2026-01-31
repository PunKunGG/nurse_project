/**
 * Manage Users Page
 * สำหรับ Teacher/Admin จัดการสิทธิ์ผู้ใช้ (Student/Teacher)
 */

import { supabase } from "../supabaseClient.js";
import { getMyProfile, signOut } from "../auth.js";

// DOM Elements
const usersBody = document.getElementById("usersBody");
const searchInput = document.getElementById("searchInput");
const totalCount = document.getElementById("totalCount");
const studentCount = document.getElementById("studentCount");
const teacherCount = document.getElementById("teacherCount");
const toast = document.getElementById("toast");

let allUsers = [];

// Initialize page
async function init() {
  // Check if user is teacher/admin
  const profile = await getMyProfile();
  if (!profile || (profile.role !== "teacher" && profile.role !== "admin")) {
    window.location.href = "dashboard.html";
    return;
  }

  await loadUsers();

  // Setup search
  searchInput.addEventListener("input", filterUsers);
}

// Load all users from profiles table
async function loadUsers() {
  try {
    const { data, error } = await supabase
      .from("profiles")
      .select("user_id, full_name, student_id, role")
      .order("full_name", { ascending: true });

    if (error) throw error;

    allUsers = data || [];
    renderUsers(allUsers);
    updateStats();
  } catch (error) {
    console.error("Error loading users:", error);
    usersBody.innerHTML = `
      <tr>
        <td colspan="5" class="error-cell">
          เกิดข้อผิดพลาดในการโหลดข้อมูล: ${error.message}
        </td>
      </tr>
    `;
  }
}

// Render users table
function renderUsers(users) {
  if (users.length === 0) {
    usersBody.innerHTML = `
      <tr>
        <td colspan="5" class="empty-cell">
          ไม่พบผู้ใช้
        </td>
      </tr>
    `;
    return;
  }

  usersBody.innerHTML = users
    .map(
      (user, index) => `
    <tr>
      <td>${index + 1}</td>
      <td>${user.full_name || "-"}</td>
      <td>${user.student_id || "-"}</td>
      <td>
        <span class="role-badge role-${user.role || "student"}">
          ${user.role === "teacher" ? "👨‍🏫 Teacher" : user.role === "admin" ? "👑 Admin" : "👤 Student"}
        </span>
      </td>
      <td>
        <select 
          class="role-select" 
          data-user-id="${user.user_id}"
          onchange="window.updateUserRole('${user.user_id}', this.value)"
        >
          <option value="student" ${user.role === "student" ? "selected" : ""}>Student</option>
          <option value="teacher" ${user.role === "teacher" ? "selected" : ""}>Teacher</option>
        </select>
      </td>
    </tr>
  `,
    )
    .join("");
}

// Filter users by search input
function filterUsers() {
  const query = searchInput.value.toLowerCase().trim();

  if (!query) {
    renderUsers(allUsers);
    return;
  }

  const filtered = allUsers.filter(
    (user) =>
      (user.full_name && user.full_name.toLowerCase().includes(query)) ||
      (user.student_id && user.student_id.toLowerCase().includes(query)),
  );

  renderUsers(filtered);
}

// Update user role
window.updateUserRole = async function (userId, newRole) {
  try {
    const { error } = await supabase
      .from("profiles")
      .update({ role: newRole })
      .eq("user_id", userId);

    if (error) throw error;

    // Update local data
    const user = allUsers.find((u) => u.user_id === userId);
    if (user) {
      user.role = newRole;
    }

    updateStats();
    showToast(`เปลี่ยนสิทธิ์เป็น ${newRole} สำเร็จ`, "success");

    // Re-render to update badge
    filterUsers();
  } catch (error) {
    console.error("Error updating role:", error);
    showToast(`เกิดข้อผิดพลาด: ${error.message}`, "error");
    // Reload to reset select
    loadUsers();
  }
};

// Update statistics
function updateStats() {
  const students = allUsers.filter(
    (u) => u.role === "student" || !u.role,
  ).length;
  const teachers = allUsers.filter(
    (u) => u.role === "teacher" || u.role === "admin",
  ).length;

  totalCount.textContent = allUsers.length;
  studentCount.textContent = students;
  teacherCount.textContent = teachers;
}

// Show toast notification
function showToast(message, type = "info") {
  toast.textContent = message;
  toast.className = `toast show ${type}`;

  setTimeout(() => {
    toast.className = "toast";
  }, 3000);
}

// Start
init();
