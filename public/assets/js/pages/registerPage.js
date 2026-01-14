import { supabase } from "../supabaseClient.js";

const form = document.getElementById("form");
const msg = document.getElementById("msg");

function setMsg(text, color = "") {
  const style = color ? `style="color:${color};"` : "";
  msg.innerHTML = `<small ${style}>${text}</small>`;
}

form.addEventListener("submit", async (e) => {
  e.preventDefault();

  const fullName = document.getElementById("fullName").value.trim();
  const studentId = document.getElementById("studentId").value.trim();
  const email = document.getElementById("email").value.trim();
  const password = document.getElementById("password").value;

  // กันคนกรอกมั่ว (ช่วย UX)
  if (!fullName || !studentId || !email || !password) {
    setMsg("กรุณากรอกข้อมูลให้ครบ", "#fca5a5");
    return;
  }

  // (Optional) เช็ครหัสนักศึกษาแบบ KKU คร่าว ๆ เช่น 663380xxx-x
  const sidOk = /^[0-9]{9}-[0-9]$/.test(studentId);
  if (!sidOk) {
    setMsg("รูปแบบรหัสนักศึกษาไม่ถูกต้อง (ตัวอย่าง: 663380213-6)", "#fca5a5");
    return;
  }

  setMsg("กำลังสมัคร…");

  // ปิดปุ่มกันกดซ้ำ
  const submitBtn = form.querySelector('button[type="submit"]');
  submitBtn.disabled = true;

  try {
    const { data, error } = await supabase.auth.signUp({
      email,
      password,
      options: {
        // ถ้าคุณใช้ GitHub Pages / path ย่อย ให้แก้ url นี้เป็น URL จริงตอน deploy
        emailRedirectTo: `${location.origin}${location.pathname.replace(
          /\/[^/]*$/,
          "/"
        )}login.html`,
        data: {
          full_name: fullName,
          student_id: studentId,
          role: "student",
        },
      },
    });

    if (error) throw error;

    // Supabase ถ้าเปิด Email confirmation:
    // data.session = null แต่ user ถูกสร้างแล้ว และต้องไปกดยืนยันอีเมล
    const needsConfirm = !data?.session;

    if (needsConfirm) {
      setMsg(
        "สมัครสำเร็จ ✅ กรุณาเช็คอีเมลเพื่อยืนยันบัญชี แล้วค่อยกลับมาล็อกอิน",
        "#86efac"
      );
      // ไม่ redirect ทันที ให้ผู้ใช้ได้อ่านข้อความ
      return;
    }

    // ถ้าไม่ได้เปิด confirm email: จะได้ session ทันที
    setMsg("สมัครสำเร็จ ✅ กำลังพาไปหน้าเข้าสู่ระบบ…", "#86efac");
    setTimeout(() => (window.location.href = "login.html"), 700);
  } catch (err) {
    setMsg(`สมัครไม่สำเร็จ: ${err.message}`, "#fca5a5");
  } finally {
    submitBtn.disabled = false;
  }
});
