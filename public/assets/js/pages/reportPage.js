/**
 * Report Page JavaScript
 * Displays student scores and statistics
 */

import { supabase } from "../supabaseClient.js";
import { getSession } from "../auth.js";

// State
let allAttempts = [];
let currentFilter = "all";

// DOM Elements
const reportBody = document.getElementById("reportBody");
const totalStudents = document.getElementById("totalStudents");
const totalAttempts = document.getElementById("totalAttempts");
const passedCount = document.getElementById("passedCount");
const avgScore = document.getElementById("avgScore");

/**
 * Initialize page
 */
async function init() {
  // Check if user is teacher/admin
  const session = await getSession();
  if (!session) {
    window.location.href = "login.html";
    return;
  }

  const { data: profile } = await supabase
    .from("profiles")
    .select("role")
    .eq("user_id", session.user.id)
    .maybeSingle();

  if (!profile || (profile.role !== "teacher" && profile.role !== "admin")) {
    alert("คุณไม่มีสิทธิ์เข้าถึงหน้านี้");
    window.location.href = "dashboard.html";
    return;
  }

  await loadAttempts();
}

/**
 * Load all attempts from Supabase with profile data
 */
async function loadAttempts() {
  try {
    // Load attempts
    const { data: attempts, error: attemptsError } = await supabase
      .from("attempts")
      .select("*")
      .order("submitted_at", { ascending: false });

    if (attemptsError) throw attemptsError;

    // Load profiles separately
    const { data: profiles, error: profilesError } = await supabase
      .from("profiles")
      .select("user_id, full_name, student_id");

    if (profilesError) throw profilesError;

    // Create a map for quick lookup
    const profileMap = {};
    profiles?.forEach((p) => {
      profileMap[p.user_id] = p;
    });

    // Merge data
    allAttempts = (attempts || []).map((a) => ({
      ...a,
      full_name: profileMap[a.user_id]?.full_name || "ไม่ระบุชื่อ",
      student_id: profileMap[a.user_id]?.student_id || "-",
    }));

    updateStats();
    renderAttempts();
  } catch (error) {
    console.error("Error loading attempts:", error);
    reportBody.innerHTML = `
      <tr>
        <td colspan="7" class="empty-cell">
          ⚠️ ไม่สามารถโหลดข้อมูลได้: ${error.message}
        </td>
      </tr>
    `;
  }
}

/**
 * Update statistics
 */
function updateStats() {
  // Unique students
  const uniqueStudents = new Set(allAttempts.map((a) => a.user_id));
  totalStudents.textContent = uniqueStudents.size;

  // Total attempts
  totalAttempts.textContent = allAttempts.length;

  // Passed count
  const passed = allAttempts.filter((a) => a.passed).length;
  passedCount.textContent = passed;

  // Average score
  if (allAttempts.length > 0) {
    const avg =
      allAttempts.reduce((sum, a) => sum + (a.score_percent || 0), 0) /
      allAttempts.length;
    avgScore.textContent = Math.round(avg) + "%";
  } else {
    avgScore.textContent = "0%";
  }
}

/**
 * Render attempts table
 */
function renderAttempts() {
  let filtered = allAttempts;

  if (currentFilter !== "all") {
    filtered = allAttempts.filter((a) => a.test_type === currentFilter);
  }

  if (filtered.length === 0) {
    reportBody.innerHTML = `
      <tr>
        <td colspan="7" class="empty-cell">
          📝 ยังไม่มีข้อมูลการทำข้อสอบ${currentFilter === "all" ? "" : ` (${currentFilter === "pre" ? "Pre-test" : "Post-test"})`}
        </td>
      </tr>
    `;
    return;
  }

  reportBody.innerHTML = filtered
    .map(
      (a, index) => `
      <tr>
        <td>${index + 1}</td>
        <td>${escapeHtml(a.full_name)}</td>
        <td>${escapeHtml(a.student_id)}</td>
        <td><span class="type-badge ${a.test_type}">${a.test_type === "pre" ? "Pre-test" : "Post-test"}</span></td>
        <td><span class="score-badge ${getScoreClass(a.score_percent)}">${a.score_percent}%</span></td>
        <td><span class="status-badge ${a.passed ? "passed" : "failed"}">${a.passed ? "ผ่าน" : "ไม่ผ่าน"}</span></td>
        <td>${formatDate(a.submitted_at)}</td>
      </tr>
    `,
    )
    .join("");
}

/**
 * Filter attempts by type
 */
function filterAttempts(type) {
  currentFilter = type;

  // Update active tab
  document.querySelectorAll(".tab-btn").forEach((btn) => {
    btn.classList.toggle("active", btn.dataset.type === type);
  });

  renderAttempts();
}

/**
 * Export to CSV
 */
function exportCSV() {
  if (allAttempts.length === 0) {
    alert("ไม่มีข้อมูลสำหรับ export");
    return;
  }

  const headers = [
    "ลำดับ",
    "ชื่อ-นามสกุล",
    "รหัสนักศึกษา",
    "ประเภท",
    "คะแนน",
    "สถานะ",
    "วันที่ทำ",
  ];
  const rows = allAttempts.map((a, index) => [
    index + 1,
    a.full_name,
    a.student_id,
    a.test_type === "pre" ? "Pre-test" : "Post-test",
    a.score_percent + "%",
    a.passed ? "ผ่าน" : "ไม่ผ่าน",
    formatDate(a.submitted_at),
  ]);

  const csv = [headers.join(","), ...rows.map((r) => r.join(","))].join("\n");
  const blob = new Blob(["\ufeff" + csv], { type: "text/csv;charset=utf-8;" });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = `report_${new Date().toISOString().split("T")[0]}.csv`;
  link.click();
}

/**
 * Helper functions
 */
function getScoreClass(score) {
  if (score >= 80) return "high";
  if (score >= 60) return "medium";
  return "low";
}

function formatDate(dateStr) {
  if (!dateStr) return "-";
  const d = new Date(dateStr);
  return d.toLocaleDateString("th-TH", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function escapeHtml(text) {
  const div = document.createElement("div");
  div.textContent = text || "";
  return div.innerHTML;
}

// Export functions to window
window.filterAttempts = filterAttempts;
window.exportCSV = exportCSV;

// Initialize on load
init();
