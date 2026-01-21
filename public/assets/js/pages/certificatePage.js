import { supabase } from "../supabaseClient.js";
import { getSession } from "../auth.js";

// DOM Elements
const statusIcon = document.getElementById("statusIcon");
const statusTitle = document.getElementById("statusTitle");
const statusSubtitle = document.getElementById("statusSubtitle");
const scoreDisplay = document.getElementById("scoreDisplay");
const scoreCircle = document.getElementById("scoreCircle");
const scoreDetail = document.getElementById("scoreDetail");
const downloadBtn = document.getElementById("download");
const certLocked = document.getElementById("certLocked");
const certPreview = document.getElementById("certPreview");
const previewName = document.getElementById("previewName");
const previewStudentId = document.getElementById("previewStudentId");

// Timeline elements
const step2Icon = document.getElementById("step2Icon");
const step2Text = document.getElementById("step2Text");
const step3Icon = document.getElementById("step3Icon");
const step3Text = document.getElementById("step3Text");
const step4Icon = document.getElementById("step4Icon");
const step4Text = document.getElementById("step4Text");
const step5Icon = document.getElementById("step5Icon");
const step5Text = document.getElementById("step5Text");

const session = await getSession();
if (!session) {
  window.location.href = "login.html";
  throw new Error("not logged in");
}

const moduleId = "module-1";

// Update preview with user info
if (previewName) {
  previewName.textContent =
    session.user.user_metadata?.full_name || session.user.email.split("@")[0];
}
if (previewStudentId) {
  previewStudentId.textContent = `รหัสนักศึกษา: ${session.user.user_metadata?.student_id || "-"}`;
}

// Check attempts for pre-test
const { data: pretestAttempts } = await supabase
  .from("attempts")
  .select("id")
  .eq("user_id", session.user.id)
  .eq("module_id", moduleId)
  .eq("test_type", "pre")
  .limit(1);

const hasDonePretest = pretestAttempts && pretestAttempts.length > 0;

// Update Pre-test timeline (step 2)
if (hasDonePretest && step2Icon && step2Text) {
  step2Icon.className = "timeline-icon done";
  step2Icon.textContent = "✓";
  step2Text.className = "timeline-text done";
}

// TODO: Check game progress (step 3) - รอเพื่อนทำระบบเกมก่อน
// เมื่อมี table game_progress แล้ว uncomment โค้ดด้านล่างนี้
// let hasDoneGame = false;
// try {
//   const { data: gameProgress, error: gameErr } = await supabase
//     .from("game_progress")
//     .select("id")
//     .eq("user_id", session.user.id)
//     .limit(1);
//   hasDoneGame = !gameErr && gameProgress && gameProgress.length > 0;
//   if (hasDoneGame && step3Icon && step3Text) {
//     step3Icon.className = "timeline-icon done";
//     step3Icon.textContent = "✓";
//     step3Text.className = "timeline-text done";
//   }
// } catch (e) {
//   console.log("Could not check game progress:", e);
// }

// Check attempts for post-test
const { data: attempts, error: aErr } = await supabase
  .from("attempts")
  .select("score_percent, submitted_at")
  .eq("user_id", session.user.id)
  .eq("module_id", moduleId)
  .eq("test_type", "post")
  .order("submitted_at", { ascending: false })
  .limit(1);

let canDownload = false;
let latestScore = null;

if (aErr || !attempts || attempts.length === 0) {
  // No post-test attempt yet
  statusIcon.className = "status-icon pending";
  statusIcon.textContent = "📝";
  statusTitle.textContent = "ยังไม่ได้ทำ Post-test";
  statusSubtitle.textContent = "กรุณาทำแบบทดสอบหลังเรียนก่อน";
  downloadBtn.disabled = true;
  scoreDisplay.style.display = "none";
} else {
  latestScore = Number(attempts[0].score_percent);
  canDownload = latestScore >= 60;

  // Show score display
  scoreDisplay.style.display = "flex";
  scoreCircle.textContent = `${latestScore}%`;
  scoreCircle.className = `score-circle ${canDownload ? "passed" : "failed"}`;

  if (canDownload) {
    // Passed!
    statusIcon.className = "status-icon success";
    statusIcon.textContent = "🎉";
    statusTitle.textContent = "ยินดีด้วย! คุณผ่านเกณฑ์แล้ว";
    statusSubtitle.textContent = "สามารถดาวน์โหลดเกียรติบัตรได้เลย";
    scoreDetail.textContent = `ผ่านเกณฑ์มากกว่า 60%`;
    downloadBtn.disabled = false;

    // Show preview
    certLocked.style.display = "none";
    certPreview.style.display = "block";

    // Update timeline
    step4Icon.className = "timeline-icon done";
    step4Icon.textContent = "✓";
    step4Text.className = "timeline-text done";
    step5Icon.className = "timeline-icon done";
    step5Icon.textContent = "✓";
    step5Text.className = "timeline-text done";
  } else {
    // Not passed
    statusIcon.className = "status-icon failed";
    statusIcon.textContent = "❌";
    statusTitle.textContent = "ยังไม่ผ่านเกณฑ์";
    statusSubtitle.textContent = `ต้องได้ มากกว่า 60% แต่คุณได้ ${latestScore}%`;
    scoreDetail.textContent = `ต้องได้อีก ${60 - latestScore}% จึงจะผ่าน`;
    downloadBtn.disabled = true;

    // Update timeline - step 4 in progress
    step4Icon.className = "timeline-icon pending";
    step4Icon.textContent = "○";
    step4Text.textContent = `ทำ Post-test (ได้ ${latestScore}%)`;
  }
}

// Download button click handler
downloadBtn.addEventListener("click", async () => {
  if (downloadBtn.disabled) return;

  // Show loading state
  const originalText = downloadBtn.innerHTML;
  downloadBtn.innerHTML = "⏳ กำลังสร้างเกียรติบัตร...";
  downloadBtn.disabled = true;

  try {
    // Issue certificate from database
    const { data: issued, error: iErr } = await supabase.rpc(
      "issue_certificate",
      {
        p_module_id: moduleId,
      },
    );

    if (iErr) {
      alert("ออกใบไม่ได้: " + iErr.message);
      downloadBtn.innerHTML = originalText;
      downloadBtn.disabled = false;
      return;
    }

    const cert = issued?.[0];
    if (!cert) {
      alert("ออกใบไม่ได้: ไม่พบข้อมูล cert ที่คืนกลับมา");
      downloadBtn.innerHTML = originalText;
      downloadBtn.disabled = false;
      return;
    }

    // Create Canvas
    await document.fonts.ready;

    const canvas = document.createElement("canvas");
    canvas.width = 1920;
    canvas.height = 1080;
    const ctx = canvas.getContext("2d");

    // Fallback background
    ctx.fillStyle = "#0b1220";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Background image
    const bg = new Image();
    bg.src = "assets/img/Nurse_Certificate.png";
    await new Promise((resolve) => {
      bg.onload = resolve;
      bg.onerror = resolve;
    });
    if (bg.width) ctx.drawImage(bg, 0, 0, canvas.width, canvas.height);

    // Text on certificate
    ctx.fillStyle = "#000000";
    ctx.textAlign = "center";

    ctx.font = "bold 60px Charm";
    ctx.fillText("ประกาศนียบัตร", canvas.width / 2, 240);

    ctx.font = "35px Charm";
    ctx.fillText("มอบให้เพื่อแสดงว่า", canvas.width / 2, 320);

    ctx.font = "bold 65px Charm";
    ctx.fillText(cert.full_name, canvas.width / 2, 470);

    ctx.font = "30px Charm";
    ctx.fillText(`รหัสนักศึกษา: ${cert.student_id}`, canvas.width / 2, 540);

    ctx.font = "bold 35px Charm";
    ctx.fillText("ได้ผ่านการทำแบบทดสอบหลังเรียนเรื่อง:", canvas.width / 2, 650);
    ctx.fillText("ภารกิจพิชิต I", canvas.width / 2, 700);

    // Use current date for certificate
    const issuedAtText = new Date().toLocaleDateString("th-TH", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
    ctx.font = "bold 30px Charm";
    ctx.fillText(`ให้ไว้ ณ วันที่ : ${issuedAtText}`, canvas.width / 2, 820);

    // Certificate Code
    ctx.font = "25px Charm";
    ctx.fillText(`Certificate Code: ${cert.cert_code}`, canvas.width / 2, 880);

    // QR Code for verification
    const basePath = location.pathname.replace(/\/[^/]*$/, "/");
    const verifyUrl = `${location.origin}${basePath}verify.html?code=${encodeURIComponent(cert.cert_code)}`;

    if (
      typeof QRCode !== "undefined" &&
      typeof QRCode.toDataURL === "function"
    ) {
      const qrDataUrl = await new Promise((resolve, reject) => {
        QRCode.toDataURL(verifyUrl, { margin: 1, width: 220 }, (err, url) =>
          err ? reject(err) : resolve(url),
        );
      });

      const qrImg = new Image();
      qrImg.src = qrDataUrl;
      await new Promise((resolve) => {
        qrImg.onload = resolve;
        qrImg.onerror = resolve;
      });

      ctx.drawImage(qrImg, 1300, 840, 220, 220);
    }

    // Download as PNG
    const url = canvas.toDataURL("image/png");
    const a = document.createElement("a");
    a.href = url;
    a.download = `certificate_${moduleId}_${cert.student_id}_${cert.cert_code}.png`;
    a.click();

    // Reset button
    downloadBtn.innerHTML = "✅ ดาวน์โหลดสำเร็จ!";
    setTimeout(() => {
      downloadBtn.innerHTML = originalText;
      downloadBtn.disabled = false;
    }, 2000);
  } catch (error) {
    console.error("Error generating certificate:", error);
    alert("เกิดข้อผิดพลาด: " + error.message);
    downloadBtn.innerHTML = originalText;
    downloadBtn.disabled = false;
  }
});
