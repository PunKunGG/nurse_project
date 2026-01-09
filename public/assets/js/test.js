// NOTE: ตอนนี้จำลองก่อน โดยเก็บคะแนน post-test ไว้ใน localStorage
// ของจริงจะส่งเข้า backend แล้วเก็บใน DB (Supabase)
document.getElementById("save").addEventListener("click", () => {
  const score = Number(document.getElementById("score").value || 0);
  localStorage.setItem("post_score_percent", String(score));
  document.getElementById(
    "msg"
  ).innerHTML = `<small>บันทึกแล้ว: ${score}%</small>`;
});
