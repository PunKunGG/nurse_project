import { signIn } from "../auth.js";

const mli = document.getElementById("msg_login");
const btnLogin = document.getElementById("btn_login");

if (!btnLogin || !mli) {
  console.error("Login elements not found");
} else {
  btnLogin.addEventListener("click", async () => {
    mli.innerHTML = "<small>กำลังเข้าสู่ระบบ…</small>";
    try {
      const email = document.getElementById("li_email").value.trim();
      const pass = document.getElementById("li_pass").value;

      await signIn(email, pass);
      window.location.href = "dashboard.html";
    } catch (e) {
      mli.innerHTML = `<small style="color:#fca5a5;">ล็อกอินไม่สำเร็จ: ${e.message}</small>`;
    }
  });
}
