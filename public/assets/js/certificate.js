const statusEl = document.getElementById("status");
const btn = document.getElementById("download");

// ตอนนี้ยัง mock ด้วย localStorage ก่อน
const score = Number(localStorage.getItem("post_score_percent") || "0");

if (score >= 80) {
  statusEl.innerHTML = `✅ <b>ผ่าน</b> (Post-test ${score}%) คุณสามารถดาวน์โหลดได้`;
  btn.disabled = false;
} else {
  statusEl.innerHTML = `❌ <b>ไม่ผ่าน</b> (Post-test ${score}%) ต้องได้อย่างน้อย 80%`;
  btn.disabled = true;
}

// ปุ่มดาวน์โหลด: รอบแรกเราจะให้เรียก backend (Cloudflare Worker) ทีหลัง
btn.addEventListener("click", async () => {
  // ชั่วคราว: ดาวน์โหลดไฟล์ dummy
  const blob = new Blob([`CERTIFICATE\nPost-test score: ${score}%`], {
    type: "text/plain",
  });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = "certificate.txt";
  a.click();
  URL.revokeObjectURL(url);
});
