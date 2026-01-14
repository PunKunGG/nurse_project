import { getSession, getMyProfile, signOut } from "../auth.js";

const me = document.getElementById("me");
const logoutBtn = document.getElementById("btn_logout");
const teacherTools = document.getElementById("teacher_tools");

async function init() {
  const session = await getSession();

  if (!session) {
    window.location.href = "login.html";
    return;
  }

  let profile = null;

  try {
    profile = await getMyProfile();

    if (!profile) {
      me.innerHTML = `<small style="color:#fbbf24;">⚠️ ยังไม่พบโปรไฟล์ใน profiles</small>`;
    } else {
      me.innerHTML = `👤 <b>${profile.full_name}</b> (${profile.student_id}) <small>role: ${profile.role}</small>`;
    }
  } catch (e) {
    me.innerHTML = `<small style="color:#fca5a5;">โหลดโปรไฟล์ไม่สำเร็จ: ${e.message}</small>`;
  }

  // โชว์ Teacher tools เฉพาะ role ที่กำหนด
  if (
    teacherTools &&
    profile &&
    (profile.role === "teacher" || profile.role === "admin")
  ) {
    teacherTools.style.display = "block";
  }

  logoutBtn?.addEventListener("click", async () => {
    await signOut();
    window.location.href = "index.html";
  });
}

init();
