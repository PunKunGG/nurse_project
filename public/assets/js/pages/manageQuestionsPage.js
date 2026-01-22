/**
 * Manage Questions Page JavaScript
 * Handles CRUD operations for questions via Supabase
 */

import { supabase } from "../supabaseClient.js";
import { getSession } from "../auth.js";

// State
let allQuestions = [];
let currentFilter = "pre";
let editingQuestionId = null;

// DOM Elements
const questionsBody = document.getElementById("questionsBody");
const totalCount = document.getElementById("totalCount");
const preCount = document.getElementById("preCount");
const postCount = document.getElementById("postCount");

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
    showToast("คุณไม่มีสิทธิ์เข้าถึงหน้านี้", "error");
    setTimeout(() => {
      window.location.href = "dashboard.html";
    }, 2000);
    return;
  }

  await loadQuestions();
}

/**
 * Load all questions from Supabase
 */
async function loadQuestions() {
  try {
    const { data, error } = await supabase
      .from("questions")
      .select("*")
      .order("test_type", { ascending: true })
      .order("question_number", { ascending: true });

    if (error) throw error;

    allQuestions = data || [];
    updateStats();
    renderQuestions();
  } catch (error) {
    console.error("Error loading questions:", error);
    questionsBody.innerHTML = `
      <tr>
        <td colspan="5" class="empty-cell">
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
  const preQuestions = allQuestions.filter((q) => q.test_type === "pre");
  const postQuestions = allQuestions.filter((q) => q.test_type === "post");

  totalCount.textContent = allQuestions.length;
  preCount.textContent = preQuestions.length;
  postCount.textContent = postQuestions.length;
}

/**
 * Render questions table
 */
function renderQuestions() {
  let filtered = allQuestions;

  if (currentFilter !== "all") {
    filtered = allQuestions.filter((q) => q.test_type === currentFilter);
  }

  if (filtered.length === 0) {
    questionsBody.innerHTML = `
      <tr>
        <td colspan="5" class="empty-cell">
          📝 ยังไม่มีข้อสอบ${currentFilter === "all" ? "" : ` (${currentFilter === "pre" ? "Pre-test" : "Post-test"})`}
        </td>
      </tr>
    `;
    return;
  }

  questionsBody.innerHTML = filtered
    .map(
      (q) => `
      <tr data-id="${q.id}">
        <td>${q.question_number}</td>
        <td><span class="type-badge ${q.test_type}">${q.test_type === "pre" ? "Pre-test" : "Post-test"}</span></td>
        <td class="question-text-cell" title="${escapeHtml(q.question_text)}">${escapeHtml(q.question_text)}</td>
        <td><span class="answer-badge">${q.correct_answer}</span></td>
        <td>
          <div class="action-btns">
            <button class="action-btn edit" onclick="editQuestion('${q.id}')" title="แก้ไข">✏️</button>
            <button class="action-btn delete" onclick="deleteQuestion('${q.id}')" title="ลบ">🗑️</button>
          </div>
        </td>
      </tr>
    `,
    )
    .join("");
}

/**
 * Filter questions by type
 */
function filterQuestions(type) {
  currentFilter = type;

  // Update active tab
  document.querySelectorAll(".tab-btn").forEach((btn) => {
    btn.classList.toggle("active", btn.dataset.type === type);
  });

  renderQuestions();
}

/**
 * Open add question modal
 */
function openAddModal() {
  editingQuestionId = null;
  document.getElementById("modalTitle").textContent = "เพิ่มข้อสอบใหม่";
  document.getElementById("questionForm").reset();

  // Set default options template
  document.getElementById("optionsJson").value = `[
  {"key": "ก", "text": ""},
  {"key": "ข", "text": ""},
  {"key": "ค", "text": ""},
  {"key": "ง", "text": ""}
]`;

  // Auto-set next question number
  const filtered = allQuestions.filter(
    (q) => q.test_type === document.getElementById("testType").value,
  );
  document.getElementById("questionNumber").value = filtered.length + 1;

  openModal();
}

/**
 * Edit question
 */
function editQuestion(id) {
  const question = allQuestions.find((q) => q.id === id);
  if (!question) return;

  editingQuestionId = id;
  document.getElementById("modalTitle").textContent = "แก้ไขข้อสอบ";

  // Populate form
  document.getElementById("testType").value = question.test_type;
  document.getElementById("questionNumber").value = question.question_number;
  document.getElementById("questionText").value = question.question_text;
  document.getElementById("optionsJson").value = JSON.stringify(
    question.options,
    null,
    2,
  );
  document.getElementById("correctAnswer").value = question.correct_answer;

  openModal();
}

/**
 * Save question (add or edit)
 */
async function saveQuestion(event) {
  event.preventDefault();

  const saveBtn = document.getElementById("saveBtn");
  saveBtn.disabled = true;
  saveBtn.textContent = "กำลังบันทึก...";

  try {
    // Parse options
    let options;
    try {
      options = JSON.parse(document.getElementById("optionsJson").value);
    } catch (e) {
      throw new Error("รูปแบบ JSON ของตัวเลือกไม่ถูกต้อง");
    }

    const questionData = {
      test_type: document.getElementById("testType").value,
      question_number: parseInt(
        document.getElementById("questionNumber").value,
      ),
      question_text: document.getElementById("questionText").value.trim(),
      options: options,
      correct_answer: document.getElementById("correctAnswer").value,
      updated_at: new Date().toISOString(),
    };

    if (editingQuestionId) {
      // Update existing
      const { error } = await supabase
        .from("questions")
        .update(questionData)
        .eq("id", editingQuestionId);

      if (error) throw error;
      showToast("แก้ไขข้อสอบสำเร็จ!", "success");
    } else {
      // Insert new
      questionData.created_at = new Date().toISOString();
      const { error } = await supabase.from("questions").insert(questionData);

      if (error) throw error;
      showToast("เพิ่มข้อสอบสำเร็จ!", "success");
    }

    closeModal();
    await loadQuestions();
  } catch (error) {
    console.error("Error saving question:", error);
    showToast(`เกิดข้อผิดพลาด: ${error.message}`, "error");
  } finally {
    saveBtn.disabled = false;
    saveBtn.textContent = "บันทึก";
  }
}

/**
 * Delete question - show confirmation
 */
let deletingQuestionId = null;

function deleteQuestion(id) {
  const question = allQuestions.find((q) => q.id === id);
  if (!question) return;

  deletingQuestionId = id;
  document.getElementById("deleteQuestionText").textContent =
    `"${question.question_text.substring(0, 100)}..."`;

  document.getElementById("deleteModal").classList.add("active");
  document.body.style.overflow = "hidden";
}

/**
 * Confirm delete
 */
async function confirmDelete() {
  if (!deletingQuestionId) return;

  try {
    const { error } = await supabase
      .from("questions")
      .delete()
      .eq("id", deletingQuestionId);

    if (error) throw error;

    showToast("ลบข้อสอบสำเร็จ!", "success");
    closeDeleteModal();
    await loadQuestions();
  } catch (error) {
    console.error("Error deleting question:", error);
    showToast(`เกิดข้อผิดพลาด: ${error.message}`, "error");
  }
}

/**
 * Modal controls
 */
function openModal() {
  document.getElementById("questionModal").classList.add("active");
  document.body.style.overflow = "hidden";
}

function closeModal() {
  document.getElementById("questionModal").classList.remove("active");
  document.body.style.overflow = "";
  editingQuestionId = null;
}

function closeDeleteModal() {
  document.getElementById("deleteModal").classList.remove("active");
  document.body.style.overflow = "";
  deletingQuestionId = null;
}

/**
 * Toast notification
 */
function showToast(message, type = "success") {
  const toast = document.getElementById("toast");
  toast.textContent = message;
  toast.className = `toast ${type} show`;

  setTimeout(() => {
    toast.classList.remove("show");
  }, 3000);
}

/**
 * Escape HTML
 */
function escapeHtml(text) {
  const div = document.createElement("div");
  div.textContent = text;
  return div.innerHTML;
}

// Close modal on escape
document.addEventListener("keydown", (e) => {
  if (e.key === "Escape") {
    closeModal();
    closeDeleteModal();
  }
});

// Close modal on overlay click
document.getElementById("questionModal").addEventListener("click", (e) => {
  if (e.target === e.currentTarget) closeModal();
});

document.getElementById("deleteModal").addEventListener("click", (e) => {
  if (e.target === e.currentTarget) closeDeleteModal();
});

// Export functions to window for onclick handlers
window.filterQuestions = filterQuestions;
window.openAddModal = openAddModal;
window.editQuestion = editQuestion;
window.deleteQuestion = deleteQuestion;
window.saveQuestion = saveQuestion;
window.confirmDelete = confirmDelete;
window.closeModal = closeModal;
window.closeDeleteModal = closeDeleteModal;

// Initialize on load
init();
