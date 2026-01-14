import { supabase } from "./supabaseClient.js";

export async function signUp(email, password, fullName, studentId) {
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
    { onConflict: "user_id" }
  );

  if (pErr) throw pErr;

  return { needsEmailConfirm: false };
}

export async function signIn(email, password) {
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
