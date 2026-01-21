/**
 * Creators Page JavaScript
 * Handles modal functionality and creator data
 */

// Creator Data
const creatorsData = {
  // === อาจารย์ที่ปรึกษา ===
  advisor1: {
    name: "ผศ.ดร.ลดาวัลย์ พันธุ์พาณิชย์",
    role: "อาจารย์ที่ปรึกษา",
    avatar: "assets/img/aj1.png",
    style: "object-position: top",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%238b5cf6%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👩‍🏫</text></svg>",
    email: "-",
    phone: "-",
    major: "คณะพยาบาลศาสตร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "อาจารย์ที่ปรึกษาโครงการ",
  },
  advisor2: {
    name: "อาจารย์พิมชญา วิเศษสิทธิกุล",
    role: "อาจารย์ที่ปรึกษา",
    avatar: "assets/img/aj2.jpg",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%238b5cf6%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨‍🏫</text></svg>",
    email: "-",
    phone: "-",
    major: "คณะพยาบาลศาสตร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "อาจารย์ที่ปรึกษาร่วม",
  },

  // === นักศึกษาผู้พัฒนา ===
  creator1: {
    name: "นางสาวรินลดา ทองตา",
    role: "663060107-8",
    avatar: "assets/img/rinlada.png",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%233b82f6%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👩‍💻</text></svg>",
    email: "rinlada.t@kkumail.com",
    phone: "-",
    major: "คณะพยาบาลศาสตร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้ทดสอบระบบ",
  },
  creator2: {
    name: "นางสาวยุภาดา พิมพ์แมน",
    role: "663060105-2",
    avatar: "assets/img/yuphada.jpg",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨‍💻</text></svg>",
    email: "yupada.p@kkumail.com",
    phone: "-",
    major: "คณะพยาบาลศาสตร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้ทดสอบระบบ",
  },
  creator3: {
    name: "นางสาวมณีชนก นิทะรัมย์",
    role: "663060100-2",
    avatar: "assets/img/maneechanok.jpg",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨‍💻</text></svg>",
    email: "maneechanok.n@kkumail.com",
    phone: "-",
    major: "คณะพยาบาลศาสตร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้ทดสอบระบบ",
  },
  creator4: {
    name: "นางสาวศวิตา สังแคนพรม",
    role: "663060110-9",
    avatar: "assets/img/sawita.jpg",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨‍💻</text></svg>",
    email: "sawita.s@kkumail.com",
    phone: "-",
    major: "คณะพยาบาลศาสตร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้ทดสอบระบบ",
  },
  creator5: {
    name: "นางสาวเมทิณี ภูนิโรจน์",
    role: "663060125-6",
    avatar: "assets/img/methinee.jpg",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨💻</text></svg>",
    email: "methinee.p@kkumail.com",
    phone: "-",
    major: "คณะพยาบาลศาสตร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้ทดสอบระบบ",
  },
  creator6: {
    name: "นางสาวอภิสรา สุสำนาจ",
    role: "663060119-1",
    avatar: "assets/img/apisara.jpg",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨💻</text></svg>",
    email: "apisara.s@kkumail.com",
    phone: "-",
    major: "คณะพยาบาลศาสตร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้ทดสอบระบบ",
  },
  creator7: {
    name: "นางสาวศิริรัตน์ อุตระทอง",
    role: "663060113-3",
    avatar: "assets/img/sirirat.jpg",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨💻</text></svg>",
    email: "sirirat.o@kkumail.com",
    phone: "-",
    major: "พยาบาลศาคณะพยาบาลศาสตร์ มหาวิทยาลัยขอนแก่นสตร์",
    responsibility: "ผู้ทดสอบระบบ",
  },
  developer1: {
    name: "นายศิวภาส ภูศรีอ่อน",
    role: "663380026-5",
    avatar: "assets/img/pun.png",
    style: "object-position: top",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨💻</text></svg>",
    email: "siwapad.p@kkumail.com",
    phone: "-",
    major: "วิทยาลัยการคอมพิวเตอร์ สาขาวิทยาการคอมพิวเตอร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้พัฒนาระบบทั้งหมด ในส่วนของหน้าบ้านและหลังบ้าน",
  },
  developer2: {
    name: "นายภควรรธ บุญเรือง",
    role: "663380227-5",
    avatar: "assets/img/tar.png",
    style: "object-position: top",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨💻</text></svg>",
    email: "phakhawat.bo@kkumail.com",
    phone: "-",
    major: "วิทยาลัยการคอมพิวเตอร์ สาขาวิทยาการคอมพิวเตอร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้พัฒนาระบบเกม และทดสอบเกม",
  },
  developer3: {
    name: "นายรัชภูมิ ทองแดง",
    role: "663380231-4",
    avatar: "assets/img/boat.png",
    style: "object-position: top",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨💻</text></svg>",
    email: "ratchaphoom.th@kkumail.com",
    phone: "-",
    major: "วิทยาลัยการคอมพิวเตอร์ สาขาวิทยาการคอมพิวเตอร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้พัฒนาระบบเกม วาดภาพประกอบฉาก และทดสอบเกม",
  },
  developer4: {
    name: "นายธิติวุฒิ ศรีอมรรัตน์",
    role: "663380213-6",
    avatar: "assets/img/gust.png",
    style: "object-position: top",
    avatarFallback:
      "data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><rect fill=%22%2310b981%22 width=%22100%22 height=%22100%22/><text x=%2250%22 y=%2260%22 font-size=%2250%22 text-anchor=%22middle%22 fill=%22white%22>👨💻</text></svg>",
    email: "thitiwut.sri@kkumail.com",
    phone: "-",
    major: "วิทยาลัยการคอมพิวเตอร์ สาขาวิทยาการคอมพิวเตอร์ มหาวิทยาลัยขอนแก่น",
    responsibility: "ผู้พัฒนาระบบเกม วาดภาพประกอบฉาก และทดสอบเกม",
  },
};

/**
 * Open modal with creator details
 * @param {string} creatorId - The ID of the creator to display
 */
function openModal(creatorId) {
  const creator = creatorsData[creatorId];
  if (!creator) return;

  const modal = document.getElementById("modal");
  const avatar = document.getElementById("modalAvatar");

  // Set avatar with fallback
  avatar.src = creator.avatar;
  avatar.style = creator.style || ""; // Apply custom style if exists
  avatar.onerror = function () {
    this.src = creator.avatarFallback;
  };

  // Populate modal content
  document.getElementById("modalName").textContent = creator.name;
  document.getElementById("modalRole").textContent = creator.role;
  document.getElementById("modalEmail").textContent = creator.email;
  document.getElementById("modalPhone").textContent = creator.phone;
  document.getElementById("modalMajor").textContent = creator.major;
  document.getElementById("modalResponsibility").textContent =
    creator.responsibility;

  // Show modal
  modal.classList.add("active");
  document.body.style.overflow = "hidden";
}

/**
 * Close the modal
 * @param {Event} event - Optional click event
 */
function closeModal(event) {
  // If clicked on overlay (not content), close
  if (event && event.target !== event.currentTarget) return;

  const modal = document.getElementById("modal");
  modal.classList.remove("active");
  document.body.style.overflow = "";
}

// Close modal with Escape key
document.addEventListener("keydown", function (e) {
  if (e.key === "Escape") {
    closeModal();
  }
});

// Export functions for use in HTML
window.openModal = openModal;
window.closeModal = closeModal;
