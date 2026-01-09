import { supabase } from "./supabaseClient.js";

export async function signUp(email, password, fullName, studentId) {
  const { data, error } = await supabase.auth.signUp({ email, password });
  if (error) throw error;

  // ถ้าเปิด confirm email ไว้ data.user อาจ null จนกว่าจะยืนยันอีเมล
  const user = data.user;
  if (!user) return { needsEmailConfirm: true };

  // สร้าง profile
  const { error: pErr } = await supabase.from("profiles").insert({
    user_id: user.id,
    full_name: fullName,
    student_id: studentId,
    role: "student",
  });
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
  const { data } = await supabase.auth.getSession();
  return data.session;
}

export async function getMyProfile() {
  const session = await getSession();
  if (!session) return null;

  const { data, error } = await supabase
    .from("profiles")
    .select("full_name, student_id, role")
    .eq("user_id", session.user.id)
    .single();

  if (error) throw error;
  return data;
}
