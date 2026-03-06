/**
 * Post-test Quiz JavaScript
 * Handles quiz logic with timer for the post-test page
 */

// Sample Questions Data - Post-test
const questions = [
  {
    id: 1,
    text: "ผู้สูงอายุ เพศชาย 72 ปี มีประวัติเป็นโรคเบาหวานและโรคความดันโลหิตสูง รับประทานยาลดความดันโลหิต หลังลุกจากเตียงมีอาการหน้ามืด เดินเซ และล้มในห้องน้ำ พยาบาลควรนึกถึงสาเหตุใดเป็นอันดับแรก",
    options: [
      { key: "ก", text: "ภาวะขาดสารอาหาร" },
      { key: "ข", text: "ผลข้างเคียงของยาและภาวะความดันตกเมื่อลุกเปลี่ยนท่า" },
      { key: "ค", text: "ภาวะสมองเสื่อม" },
      { key: "ง", text: "การนอนหลับไม่เพียงพอ" },
    ],
    answer: "ข",
  },
  {
    id: 2,
    text: "ผู้สูงอายุ เพศชาย 72 ปี มีประวัติเป็นโรคเบาหวานและโรคความดันโลหิตสูง รับประทานยาลดความดันโลหิต หลังลุกจากเตียงมีอาการหน้ามืด เดินเซ และล้มในห้องน้ำ ควรให้คำแนะนำอย่างไรสำหรับผู้สูงอายุรายนี้เพื่อป้องกันการหกล้ม",
    options: [
      { key: "ก", text: "จำกัดการเดินของผู้ป่วย" },
      { key: "ข", text: "จัดสภาพแวดล้อมให้ปลอดภัยและแนะนำลุกเปลี่ยนท่าช้า ๆ" },
      { key: "ค", text: "ใส่สายสวนปัสสาวะเพื่อลดการลุกเข้าห้องน้ำ" },
      { key: "ง", text: "งดยาลดความดันโลหิตทันที" },
    ],
    answer: "ข",
  },
  {
    id: 3,
    text: "ผู้สูงอายุ เพศหญิง 80 ปี นอนติดเตียงจากภาวะโรคหลอดเลือดสมองเป็นเวลา 2 สัปดาห์ จากนั้นพบว่าผิวหนังบริเวณก้นกบเริ่มแดง ไม่ซีดเมื่อกด สาเหตุหลักของปัญหานี้สัมพันธ์กับ Big’s I ข้อใดมากที่สุด",
    options: [
      { key: "ก", text: "Instability" },
      { key: "ข", text: "Incontinence" },
      { key: "ค", text: "Immobility" },
      { key: "ง", text: "Inanition" },
    ],
    answer: "ค",
  },
  {
    id: 4,
    text: "ผู้ป่วยสูงอายุลืมเวลา สถานที่ และบุคคลใกล้ชิด อาการดังกล่าวจัดอยู่ใน Big’s I ข้อใด ",
    options: [
      { key: "ก", text: "Insomnia" },
      { key: "ข", text: "Inanition" },
      { key: "ค", text: "Intellectual Impairment" },
      { key: "ง", text: "Instability" },
    ],
    answer: "ค",
  },
  {
    id: 5,
    text: "ผู้สูงอายุ เพศชาย 75 ปี ปัสสาวะเล็ดเมื่อไอหรือหัวเราะ ไม่มีอาการปวดแสบขัด อาการดังกล่าวเป็นภาวะกลั้นปัสสาวะไม่อยู่ชนิดใด ",
    options: [
      { key: "ก", text: "Urge incontinence" },
      { key: "ข", text: "Stress incontinence" },
      { key: "ค", text: "Overflow incontinence" },
      { key: "ง", text: "Functional incontinence" },
    ],
    answer: "ข",
  },
  {
    id: 6,
    text: "ผู้สูงอายุ เพศชาย 75 ปี ปัสสาวะเล็ดเมื่อไอหรือหัวเราะ ไม่มีอาการปวดแสบขัด การพยาบาลข้อใดเหมาะสมที่สุด ",
    options: [
      { key: "ก", text: "จำกัดการดื่มน้ำ" },
      { key: "ข", text: "ใส่สายสวนปัสสาวะถาวร" },
      { key: "ค", text: "ฝึกกล้ามเนื้ออุ้งเชิงกรานและการขับถ่ายเป็นเวลา" },
      { key: "ง", text: "ให้ยาขับปัสสาวะ" },
    ],
    answer: "ค",
  },
  {
    id: 7,
    text: "ผู้สูงอายุ เพศหญิง 82 ปี นอนโรงพยาบาล 10 วันจากปอดอักเสบ ปัจจุบันเดินเองไม่ได้ เบื่ออาหารนอนกลางวันบ่อย กลางคืนนอนไม่หลับ และเริ่มสับสนช่วงเย็น ภาวะใดควรได้รับการจัดการเป็นลำดับแรก",
    options: [
      { key: "ก", text: "Insomnia" },
      { key: "ข", text: "Intellectual impairment" },
      { key: "ค", text: "Immobility" },
      { key: "ง", text: "Inanition" },
    ],
    answer: "ก",
  },
  {
    id: 8,
    text: "ข้อใดเป็นลักษณะที่ช่วยแยก Delirium จาก Intellectual impairment อื่นๆได้ชัดเจนที่สุด",
    options: [
      { key: "ก", text: "การมีอารมณ์แปรปรวน" },
      { key: "ข", text: "การเกิดอาการอย่างเฉียบพลันและเปลี่ยนแปลงเร็ว" },
      { key: "ค", text: "การมีปัญหาความจำ" },
      { key: "ง", text: "การสื่อสารลำบาก" },
    ],
    answer: "ข",
  },
  {
    id: 9,
    text: "ผู้ป่วยสูงอายุเบื่ออาหาร น้ำหนักลด อ่อนเพลีย ภาวะนี้ส่งผลต่อการฟื้นฟูสุขภาพอย่างไร",
    options: [
      { key: "ก", text: "เพิ่มความสามารถในการเคลื่อนไหว" },
      { key: "ข", text: "เพิ่มการนอนหลับสึก" },
      { key: "ค", text: "เพิ่มความเสี่ยงต่อการติดเชื้อและการหกล้ม" },
      { key: "ง", text: "ลดความเสี่ยงต่อแผลกดทับ" },
    ],
    answer: "ค",
  },
  {
    id: 10,
    text: "ผู้ป่วยสูงอายุบ่นนอนไม่หลับ หลับ ๆ ตื่น ๆ กลางคืน ง่วงกลางวัน การพยาบาลใดเหมาะสมที่สุด",
    options: [
      { key: "ก", text: "ให้ยานอนหลับทันที" },
      { key: "ข", text: "แนะนำงีบกลางวันให้น้อยลง" },
      {
        key: "ค",
        text: "ส่งเสริม sleep hygiene เช่น งดกาแฟ จัดสิ่งแวดล้อมก่อนนอน",
      },
      { key: "ง", text: "จำกัดกิจกรรมระหว่างวัน" },
    ],
    answer: "ค",
  },
];

let currentQuestion = 0;
let userAnswers = {};
let timerInterval = null;
let timeRemaining = 180; // 3 minutes in seconds

const STORAGE_KEY_START = "posttest_start_ts";
const STORAGE_KEY_ANSWERS = "posttest_answers";
const STORAGE_KEY_QUESTION = "posttest_current_q";

// Check if user already attempted Post-test
async function checkIfAlreadyAttempted() {
  try {
    const { supabase } = await import("../supabaseClient.js");
    const { getSession } = await import("../auth.js");

    const session = await getSession();
    if (!session) return null;

    // Check for existing post-test attempt
    const { data: attempt, error } = await supabase
      .from("attempts")
      .select("*")
      .eq("user_id", session.user.id)
      .eq("test_type", "post")
      .order("submitted_at", { ascending: false })
      .limit(1)
      .maybeSingle();

    if (error) throw error;
    return attempt;
  } catch (e) {
    console.error("Error checking attempt:", e);
    return null;
  }
}

// Show already attempted UI
function showAlreadyAttempted(attempt) {
  const passed = attempt.passed;
  const percentage = attempt.score_percent;
  const correct = Math.round((percentage / 100) * questions.length);

  // Update results UI
  const scoreEl = document.getElementById("resultScore");
  scoreEl.textContent = percentage + "%";
  scoreEl.className = "result-score " + (passed ? "passed" : "failed");

  document.getElementById("correctCount").textContent = correct;
  document.getElementById("incorrectCount").textContent =
    questions.length - correct;

  const noticeEl = document.getElementById("certificateNotice");
  const certBtn = document.getElementById("certBtn");

  if (passed) {
    document.getElementById("resultIcon").textContent = "🎉";
    document.getElementById("resultText").textContent =
      "ยอดเยี่ยม! คุณผ่านเกณฑ์";
    noticeEl.textContent = "คุณสามารถรับเกียรติบัตรได้แล้ว!";
    noticeEl.className = "certificate-notice";
    certBtn.style.display = "inline-flex";
  } else {
    document.getElementById("resultIcon").textContent = "📝";
    document.getElementById("resultText").textContent =
      "คุณได้ทำแบบทดสอบไปแล้ว";
    noticeEl.innerHTML = `
      <div style="color: var(--warning);">
        คะแนนยังไม่ถึงเกณฑ์ (ต้องได้มากกว่า 60%)<br>
        <small>หากต้องการทำใหม่ กรุณาติดต่ออาจารย์เพื่อรีเซ็ตข้อมูล</small>
      </div>
    `;
    noticeEl.className = "certificate-notice failed";
    certBtn.style.display = "none";
  }

  // Hide intro and quiz, show results directly
  document.getElementById("quizIntro").style.display = "none";
  document.getElementById("quizContainer").classList.remove("active");
  document.getElementById("quizResults").classList.add("active");
}

// Initialize page - check for previous attempts
async function initPosttest() {
  const existingAttempt = await checkIfAlreadyAttempted();
  if (existingAttempt) {
    showAlreadyAttempted(existingAttempt);
  }
}

// Run init on page load
initPosttest();

// Resume quiz if user refreshed mid-test
(function resumeIfNeeded() {
  const startTs = sessionStorage.getItem(STORAGE_KEY_START);
  if (!startTs) return; // no quiz in progress

  // Restore saved answers
  const savedAnswers = sessionStorage.getItem(STORAGE_KEY_ANSWERS);
  if (savedAnswers) {
    try {
      userAnswers = JSON.parse(savedAnswers);
    } catch (_) {
      /* ignore */
    }
  }

  // Restore current question index
  const savedQ = sessionStorage.getItem(STORAGE_KEY_QUESTION);
  if (savedQ !== null) {
    currentQuestion = parseInt(savedQ, 10) || 0;
  }

  // Jump straight into the quiz (skip intro)
  startQuiz();
})();

function startQuiz() {
  document.getElementById("quizIntro").style.display = "none";
  document.getElementById("quizContainer").classList.add("active");

  // Save start timestamp so refresh won't reset the timer
  if (!sessionStorage.getItem(STORAGE_KEY_START)) {
    sessionStorage.setItem(STORAGE_KEY_START, Date.now().toString());
  }

  renderQuestion();
  startTimer();
}

function startTimer() {
  // Calculate remaining time from saved start timestamp
  const startTs = parseInt(sessionStorage.getItem(STORAGE_KEY_START), 10);
  if (startTs) {
    const elapsed = Math.floor((Date.now() - startTs) / 1000);
    timeRemaining = Math.max(180 - elapsed, 0);
  }

  // If already out of time, auto-submit immediately
  if (timeRemaining <= 0) {
    submitQuiz();
    return;
  }

  updateTimerDisplay();
  timerInterval = setInterval(() => {
    timeRemaining--;
    updateTimerDisplay();

    if (timeRemaining <= 0) {
      clearInterval(timerInterval);
      submitQuiz(); // Auto submit when time runs out
    }
  }, 1000);
}

function updateTimerDisplay() {
  const minutes = Math.floor(timeRemaining / 60);
  const seconds = timeRemaining % 60;
  const display = `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
  document.getElementById("timerDisplay").textContent = display;

  const timerEl = document.getElementById("timer");
  if (timeRemaining <= 30) {
    timerEl.className = "timer danger";
  } else if (timeRemaining <= 60) {
    timerEl.className = "timer warning";
  } else {
    timerEl.className = "timer";
  }
}

function renderQuestion() {
  const q = questions[currentQuestion];
  const savedAnswer = userAnswers[q.id] || "";

  let optionsHTML = "";
  q.options.forEach((opt) => {
    const checked = savedAnswer === opt.key ? "checked" : "";
    optionsHTML += `
      <div class="option-item">
        <input type="radio" 
               class="option-input" 
               name="q${q.id}" 
               id="q${q.id}_${opt.key}" 
               value="${opt.key}"
               ${checked}
               onchange="selectAnswer('${q.id}', '${opt.key}')">
        <label class="option-label" for="q${q.id}_${opt.key}">
          <span class="option-marker">${opt.key}</span>
          <span class="option-text">${opt.text}</span>
        </label>
      </div>
    `;
  });

  document.getElementById("questionContainer").innerHTML = `
    <div class="question-card">
      <span class="question-number success">ข้อที่ ${q.id}</span>
      <div class="question-text">${q.text}</div>
      <div class="options-list">
        ${optionsHTML}
      </div>
    </div>
  `;

  updateProgress();
  updateNavigation();
}

function updateProgress() {
  const progress = ((currentQuestion + 1) / questions.length) * 100;
  document.getElementById("progressFill").style.width = progress + "%";
  document.getElementById("progressText").textContent =
    `ข้อ ${currentQuestion + 1}/${questions.length}`;
}

function updateNavigation() {
  const q = questions[currentQuestion];
  const hasAnswer = userAnswers[q.id] !== undefined;

  document.getElementById("prevBtn").disabled = currentQuestion === 0;

  if (currentQuestion === questions.length - 1) {
    document.getElementById("nextBtn").style.display = "none";
    document.getElementById("submitBtn").style.display = "inline-flex";
    document.getElementById("submitBtn").disabled = !hasAnswer;
  } else {
    document.getElementById("nextBtn").style.display = "inline-flex";
    document.getElementById("nextBtn").disabled = !hasAnswer;
    document.getElementById("submitBtn").style.display = "none";
  }
}

function selectAnswer(questionId, answer) {
  userAnswers[questionId] = answer;
  // Persist answers so refresh won't lose them
  sessionStorage.setItem(STORAGE_KEY_ANSWERS, JSON.stringify(userAnswers));
  updateNavigation(); // Enable next button after selecting
}

function nextQuestion() {
  if (currentQuestion < questions.length - 1) {
    currentQuestion++;
    sessionStorage.setItem(STORAGE_KEY_QUESTION, currentQuestion.toString());
    renderQuestion();
  }
}

function prevQuestion() {
  if (currentQuestion > 0) {
    currentQuestion--;
    sessionStorage.setItem(STORAGE_KEY_QUESTION, currentQuestion.toString());
    renderQuestion();
  }
}

async function submitQuiz() {
  // Stop timer
  if (timerInterval) {
    clearInterval(timerInterval);
  }

  // Clear session data — quiz is done
  sessionStorage.removeItem(STORAGE_KEY_START);
  sessionStorage.removeItem(STORAGE_KEY_ANSWERS);
  sessionStorage.removeItem(STORAGE_KEY_QUESTION);

  // Calculate score
  let correct = 0;
  questions.forEach((q) => {
    if (userAnswers[q.id] === q.answer) {
      correct++;
    }
  });

  const percentage = Math.round((correct / questions.length) * 100);
  const passed = percentage >= 60;

  // Update results UI
  const scoreEl = document.getElementById("resultScore");
  scoreEl.textContent = percentage + "%";
  scoreEl.className = "result-score " + (passed ? "passed" : "failed");

  document.getElementById("correctCount").textContent = correct;
  document.getElementById("incorrectCount").textContent =
    questions.length - correct;

  const noticeEl = document.getElementById("certificateNotice");
  const certBtn = document.getElementById("certBtn");

  if (passed) {
    document.getElementById("resultIcon").textContent = "🎉";
    document.getElementById("resultText").textContent =
      "ยอดเยี่ยม! คุณผ่านเกณฑ์";
    noticeEl.textContent = "คุณสามารถรับเกียรติบัตรได้แล้ว!";
    noticeEl.className = "certificate-notice";
    certBtn.style.display = "inline-flex";
  } else {
    document.getElementById("resultIcon").textContent = "✌🏻";
    document.getElementById("resultText").textContent =
      "พยายามต่อไป! ลองอีกครั้งนะ";
    noticeEl.textContent =
      "คะแนนยังไม่ถึงเกณฑ์ ต้องได้มากกว่า 60% เพื่อรับเกียรติบัตร";
    noticeEl.className = "certificate-notice failed";
    certBtn.style.display = "none";
  }

  // Try to save score to database
  try {
    const { supabase } = await import("../supabaseClient.js");
    const { getSession } = await import("../auth.js");

    const session = await getSession();
    if (session) {
      await supabase.from("attempts").insert({
        user_id: session.user.id,
        module_id: "module-1",
        test_type: "post",
        score_percent: percentage,
        passed: passed,
        submitted_at: new Date().toISOString(),
      });
    }
  } catch (e) {
    console.log("Could not save score:", e);
  }

  // Show results
  document.getElementById("quizContainer").classList.remove("active");
  document.getElementById("quizResults").classList.add("active");
}
