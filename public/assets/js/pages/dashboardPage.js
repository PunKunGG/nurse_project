import { getSession, getMyProfile, signOut } from "../auth.js";

const me = document.getElementById("me");
const meSub = document.getElementById("me_sub");
const logoutBtn = document.getElementById("btn_logout");
const teacherTools = document.getElementById("teacher_tools");
const adminTools = document.getElementById("admin_tools");

// slider elements
const slidesEl = document.getElementById("slides");
const dotsEl = document.getElementById("slider_dots");
const prevBtn = document.getElementById("prevBtn");
const nextBtn = document.getElementById("nextBtn");
const captionEl = document.getElementById("slider_caption");

let profile = null;

// ---------- Auth + Profile ----------
async function initProfile() {
  const session = await getSession();
  if (!session) {
    window.location.href = "login.html";
    return false;
  }

  try {
    profile = await getMyProfile();
    if (!profile) {
      me.textContent = "⚠️ ยังไม่พบโปรไฟล์ใน profiles";
      meSub.textContent = "";
      return true;
    }

    me.innerHTML = `<b>${profile.full_name}</b>`;
    meSub.textContent = `${profile.student_id}  |  role: ${profile.role}`;

    if (profile.role === "teacher" || profile.role === "admin") {
      teacherTools.style.display = "block";
    }

    if (profile.role === "admin") {
      adminTools.style.display = "block";
    }
  } catch (e) {
    me.innerHTML = `โหลดโปรไฟล์ไม่สำเร็จ`;
    meSub.textContent = e.message;
  }

  logoutBtn?.addEventListener("click", async () => {
    await signOut();
    window.location.href = "index.html";
  });

  return true;
}

// ---------- Slider ----------
function setupDots(count) {
  dotsEl.innerHTML = "";
  for (let i = 0; i < count; i++) {
    const d = document.createElement("div");
    d.className = "dot" + (i === 0 ? " active" : "");
    d.addEventListener("click", () => goTo(i));
    dotsEl.appendChild(d);
  }
}

function setActiveDot(i) {
  const dots = [...dotsEl.querySelectorAll(".dot")];
  dots.forEach((d, idx) => d.classList.toggle("active", idx === i));
}

let index = 0;
let timer = null;

function goTo(i) {
  const slides = slidesEl?.querySelectorAll(".slide") || [];
  if (!slides.length) return;

  index = (i + slides.length) % slides.length;
  slidesEl.style.transform = `translateX(${-index * 100}%)`;
  setActiveDot(index);

  if (captionEl)
    captionEl.textContent = `ภาพตัวอย่าง ${index + 1} / ${slides.length}`;
}

function next() {
  goTo(index + 1);
}
function prev() {
  goTo(index - 1);
}

function startAuto() {
  stopAuto();
  timer = setInterval(next, 3500);
}

function stopAuto() {
  if (timer) clearInterval(timer);
  timer = null;
}

function initSlider() {
  if (!slidesEl) return;

  const slides = slidesEl.querySelectorAll(".slide");
  if (!slides.length) return;

  setupDots(slides.length);
  goTo(0);
  startAuto();

  nextBtn?.addEventListener("click", () => {
    next();
    startAuto();
  });
  prevBtn?.addEventListener("click", () => {
    prev();
    startAuto();
  });

  // hover แล้วหยุดเลื่อน
  const slider = document.getElementById("slider");
  slider?.addEventListener("mouseenter", stopAuto);
  slider?.addEventListener("mouseleave", startAuto);
}

// ---------- Boot ----------
(async function boot() {
  const ok = await initProfile();
  if (!ok) return;
  initSlider();
})();
