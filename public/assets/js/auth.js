import { supabase } from "./supabaseClient.js";

// ตรวจสอบว่าเป็น KKU email หรือไม่ (รองรับทั้ง @kkumail.com และ @kku.ac.th)
function isKKUEmail(email) {
  const lowerEmail = email.toLowerCase();
  return (
    lowerEmail.endsWith("@kkumail.com") || lowerEmail.endsWith("@kku.ac.th")
  );
}

export async function signUp(email, password, fullName, studentId) {
  // ตรวจสอบว่าเป็น KKU email
  if (!isKKUEmail(email)) {
    throw new Error(
      "กรุณาใช้อีเมลมหาวิทยาลัย (@kkumail.com หรือ @kku.ac.th) เท่านั้น",
    );
  }

  // 1) สร้าง user ใน auth
  const { data, error } = await supabase.auth.signUp({
    email,
    password,
    options: {
      data: {
        full_name: fullName,
        student_id: studentId,
        role: "student",
      },
    },
  });

  if (error) throw error;

  // ถ้าเปิด confirm email ไว้ data.user อาจ null จนกว่าจะยืนยันอีเมล
  const user = data.user;
  if (!user) return { needsEmailConfirm: true };

  // 2) สร้าง profile (กันซ้ำด้วย upsert)
  // NOTE: student_id unique ถ้าซ้ำจะ error -> โยนออกไปให้ UI แสดง
  const { error: pErr } = await supabase.from("profiles").upsert(
    {
      user_id: user.id,
      full_name: fullName,
      student_id: studentId,
      role: "student",
    },
    { onConflict: "user_id" },
  );

  if (pErr) throw pErr;

  return { needsEmailConfirm: false };
}

export async function signIn(email, password) {
  // ตรวจสอบว่าเป็น KKU email
  if (!isKKUEmail(email)) {
    throw new Error(
      "กรุณาใช้อีเมลมหาวิทยาลัย (@kkumail.com หรือ @kku.ac.th) เท่านั้น",
    );
  }

  const { data, error } = await supabase.auth.signInWithPassword({
    email,
    password,
  });
  if (error) throw error;
  return data;
}

export async function signOut() {
  const { error } = await supabase.auth.signOut();
  if (error) throw error;
}

export async function getSession() {
  const { data, error } = await supabase.auth.getSession();
  if (error) throw error;
  return data.session;
}

export async function getMyProfile() {
  const session = await getSession();
  if (!session) return null;

  // ✅ เปลี่ยน single() -> maybeSingle() กัน 406
  const { data, error } = await supabase
    .from("profiles")
    .select("full_name, student_id, role")
    .eq("user_id", session.user.id)
    .maybeSingle();

  // maybeSingle: ถ้าไม่เจอ data จะเป็น null แต่ไม่ error
  if (error) throw error;

  // ถ้าไม่เจอโปรไฟล์จริง ๆ (เช่น insert ไม่ทัน/พลาด) ก็คืน null ให้หน้าไป handle
  return data;
}

// ส่ง OTP ไปยังอีเมล
export async function sendOtp(email) {
  const { error } = await supabase.auth.signInWithOtp({
    email,
    options: {
      shouldCreateUser: false, // ไม่สร้าง user ใหม่ถ้าไม่มีในระบบ
    },
  });
  if (error) throw error;
}

// ยืนยัน OTP
export async function verifyOtp(email, token) {
  const { data, error } = await supabase.auth.verifyOtp({
    email,
    token,
    type: "email",
  });
  if (error) throw error;
  return data;
}
