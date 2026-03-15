import { supabase } from "../supabaseClient.js";
import { getMyProfile } from "../auth.js";

const certificateBody = document.getElementById("certificateBody");
const searchInput = document.getElementById("searchInput");
const totalCertificates = document.getElementById("totalCertificates");
const uniqueRecipients = document.getElementById("uniqueRecipients");
const avgCertificateScore = document.getElementById("avgCertificateScore");
const latestIssuedDate = document.getElementById("latestIssuedDate");

let allCertificates = [];

function escapeHtml(text) {
  const div = document.createElement("div");
  div.textContent = text == null ? "" : String(text);
  return div.innerHTML;
}

function formatDate(dateStr, options) {
  if (!dateStr) return "-";

  const date = new Date(dateStr);
  if (Number.isNaN(date.getTime())) return "-";

  return date.toLocaleDateString(
    "th-TH",
    options || {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    },
  );
}

function updateStats() {
  totalCertificates.textContent = allCertificates.length;
  uniqueRecipients.textContent = new Set(
    allCertificates.map((item) => item.user_id),
  ).size;

  if (allCertificates.length > 0) {
    const totalScore = allCertificates.reduce(
      (sum, item) => sum + Number(item.score_percent || 0),
      0,
    );
    avgCertificateScore.textContent =
      Math.round(totalScore / allCertificates.length) + "%";
    latestIssuedDate.textContent = formatDate(allCertificates[0].issued_at, {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  } else {
    avgCertificateScore.textContent = "0%";
    latestIssuedDate.textContent = "-";
  }
}

function renderCertificates(certificates) {
  if (!certificates.length) {
    certificateBody.innerHTML = `
      <tr>
        <td colspan="8" class="empty-cell">ยังไม่พบผู้ได้รับเกียรติบัตร</td>
      </tr>
    `;
    return;
  }

  certificateBody.innerHTML = certificates
    .map(
      (item, index) => `
        <tr>
          <td>${index + 1}</td>
          <td>${escapeHtml(item.full_name || "ไม่ระบุชื่อ")}</td>
          <td>${escapeHtml(item.student_id || "-")}</td>
          <td>${escapeHtml(item.module_id || "-")}</td>
          <td><span class="score-badge high">${escapeHtml(item.score_percent)}%</span></td>
          <td>${escapeHtml(item.cert_code || "-")}</td>
          <td>${formatDate(item.issued_at)}</td>
          <td><span class="status-badge passed">ออกแล้ว</span></td>
        </tr>
      `,
    )
    .join("");
}

function filterCertificates() {
  const query = searchInput.value.toLowerCase().trim();

  if (!query) {
    renderCertificates(allCertificates);
    return;
  }

  const filtered = allCertificates.filter((item) => {
    const fullName = (item.full_name || "").toLowerCase();
    const studentId = (item.student_id || "").toLowerCase();
    const certCode = (item.cert_code || "").toLowerCase();

    return (
      fullName.includes(query) ||
      studentId.includes(query) ||
      certCode.includes(query)
    );
  });

  renderCertificates(filtered);
}

async function loadCertificates() {
  try {
    const { data: certificates, error: certificatesError } = await supabase
      .from("certificates")
      .select("user_id, module_id, score_percent, cert_code, issued_at")
      .order("issued_at", { ascending: false });

    if (certificatesError) throw certificatesError;

    const { data: profiles, error: profilesError } = await supabase
      .from("profiles")
      .select("user_id, full_name, student_id");

    if (profilesError) throw profilesError;

    const profileMap = {};
    (profiles || []).forEach((profile) => {
      profileMap[profile.user_id] = profile;
    });

    allCertificates = (certificates || []).map((item) => ({
      ...item,
      full_name: profileMap[item.user_id]?.full_name || "ไม่ระบุชื่อ",
      student_id: profileMap[item.user_id]?.student_id || "-",
    }));

    updateStats();
    renderCertificates(allCertificates);
  } catch (error) {
    console.error("Error loading certificates:", error);
    certificateBody.innerHTML = `
      <tr>
        <td colspan="8" class="empty-cell">
          ⚠️ ไม่สามารถโหลดข้อมูลผู้ได้รับเกียรติบัตรได้: ${escapeHtml(error.message)}
        </td>
      </tr>
    `;
  }
}

window.exportCSV = function exportCSV() {
  if (!allCertificates.length) {
    alert("ไม่มีข้อมูลสำหรับ export");
    return;
  }

  const headers = [
    "ลำดับ",
    "ชื่อ-นามสกุล",
    "รหัสนักศึกษา",
    "Module",
    "คะแนน Post-test",
    "Certificate Code",
    "วันที่ออก",
  ];

  const rows = allCertificates.map((item, index) => [
    index + 1,
    item.full_name || "-",
    item.student_id || "-",
    item.module_id || "-",
    `${item.score_percent || 0}%`,
    item.cert_code || "-",
    formatDate(item.issued_at),
  ]);

  const csv = [headers.join(","), ...rows.map((row) => row.join(","))].join(
    "\n",
  );
  const blob = new Blob(["\ufeff" + csv], {
    type: "text/csv;charset=utf-8;",
  });
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  link.download = `certificates_${new Date().toISOString().split("T")[0]}.csv`;
  link.click();
};

async function init() {
  const profile = await getMyProfile();
  if (!profile || (profile.role !== "teacher" && profile.role !== "admin")) {
    window.location.href = "dashboard.html";
    return;
  }

  searchInput.addEventListener("input", filterCertificates);
  await loadCertificates();
}

init();
