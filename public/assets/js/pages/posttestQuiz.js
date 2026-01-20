/**
 * Post-test Quiz JavaScript
 * Handles quiz logic with timer for the post-test page
 */

// Sample Questions Data - Post-test
const questions = [
  {
    id: 1,
    text: "ชายอายุ 72 ปี มีประวัติเป็นเบาหวานและความดันโลหิตสูง รับประทานยาลดความดัน ก่อนลุกจากเตียงมีอาการหน้ามืด เดินเซ และล้มในห้องน้ำ พยาบาลควรประเมินสาเหตุสำคัญของการหกล้มเป็นอันดับแรกจากข้อใด",
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
    text: "ชายอายุ 72 ปี มีประวัติเป็นเบาหวานและความดันโลหิตสูง รับประทานยาลดความดัน ก่อนลุกจากเตียงมีอาการหน้ามืด เดินเซ และล้มในห้องน้ำ หากต้องการป้องกันการหกล้มในระยะสั้น การพยาบาลใดเหมาะสมที่สุด",
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
    text: "หญิงอายุ 80 ปี นอนติดเตียงมา 2 สัปดาห์จากภาวะหลอดเลือดสมอง พบว่าผิวหนังบริเวณก้นกบเริ่มแดง ไม่ซีดเมื่อกด สาเหตุหลักของปัญหานี้สัมพันธ์กับข้อใดมากที่สุด",
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
    text: "ผู้ป่วยสูงอายุลืมเวลา สถานที่ และบุคคลใกล้ชิด จัดอยู่ในภาวะใด",
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
    text: "ชายอายุ 75 ปี ปัสสาวะเล็ดเมื่อไอหรือหัวเราะ ไม่มีอาการปวดแสบขัด ข้อใดเป็นชนิดของภาวะกลั้นปัสสาวะไม่อยู่ที่เหมาะสมที่สุด",
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
    text: "ชายอายุ 75 ปี ปัสสาวะเล็ดเมื่อไอหรือหัวเราะ ไม่มีอาการปวดแสบขัด การพยาบาลใดเหมาะสมที่สุดสำหรับผู้ป่วยรายนี้",
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
    text: "หญิงอายุ 82 ปี นอนโรงพยาบาล 10 วันจากปอดอักเสบ ปัจจุบันเดินเองไม่ได้ เบื่ออาหารนอนกลางวันบ่อย กลางคืนนอนไม่หลับ และเริ่มสับสนช่วงเย็น ภาวะใดควรได้รับการจัดการเป็นลำดับแรก",
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
    text: "ข้อใดเป็นลักษณะที่ช่วยแยก Delirium ออกจาก Intellectual impairment ได้ชัดเจนที่สุด",
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
    text: "ผู้ป่วยสูงอายุเบื่ออาหาร น้ำหนักลด อ่อนเพลีย ผมหายฝ้า ภาวะนี้ส่งผลต่อการฟื้นฟูสุขภาพอย่างไร",
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
      { key: "ข", text: "แนะนำจิบกลางวันให้น้อยขึ้น" },
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
let timeRemaining = 90; // 3 minutes in seconds

function startQuiz() {
  document.getElementById("quizIntro").style.display = "none";
  document.getElementById("quizContainer").classList.add("active");
  renderQuestion();
  startTimer();
}

function startTimer() {
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
  updateNavigation(); // Enable next button after selecting
}

function nextQuestion() {
  if (currentQuestion < questions.length - 1) {
    currentQuestion++;
    renderQuestion();
  }
}

function prevQuestion() {
  if (currentQuestion > 0) {
    currentQuestion--;
    renderQuestion();
  }
}

async function submitQuiz() {
  // Stop timer
  if (timerInterval) {
    clearInterval(timerInterval);
  }

  // Calculate score
  let correct = 0;
  questions.forEach((q) => {
    if (userAnswers[q.id] === q.answer) {
      correct++;
    }
  });

  const percentage = Math.round((correct / questions.length) * 100);
  const passed = percentage >= 80;

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
    noticeEl.textContent = "🎓 คุณสามารถรับเกียรติบัตรได้แล้ว!";
    noticeEl.className = "certificate-notice";
    certBtn.style.display = "inline-flex";
  } else {
    document.getElementById("resultIcon").textContent = "💪";
    document.getElementById("resultText").textContent =
      "พยายามต่อไป! ลองอีกครั้งนะ";
    noticeEl.textContent =
      "❌ คะแนนยังไม่ถึงเกณฑ์ ต้องได้ ≥ 80% เพื่อรับเกียรติบัตร";
    noticeEl.className = "certificate-notice failed";
    certBtn.style.display = "none";
  }

  // Try to save score to database
  try {
    const { supabase } = await import("./supabaseClient.js");
    const { getSession } = await import("./auth.js");

    const session = await getSession();
    if (session) {
      await supabase.from("attempts").insert({
        user_id: session.user.id,
        module_id: "module-1",
        test_type: "post",
        score_percent: percentage,
        passed: passed,
      });
    }
  } catch (e) {
    console.log("Could not save score:", e);
  }

  // Show results
  document.getElementById("quizContainer").classList.remove("active");
  document.getElementById("quizResults").classList.add("active");
}
