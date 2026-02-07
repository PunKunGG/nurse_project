## Project Type
Educational serious game for nursing students  
Name : Temp => A Nursing Simulation Game

---

## Current Implementation Status

### Main Menu (First Commit)
The Main Menu scene is implemented and functional.

**Contents**
- Game title text: A Nursing Simulation Game
- Start button
- Nurse character animation
- UI Canvas with menu controller

**Behavior**
- Clicking the **Start** button loads the `Instability` scene
- Scene transition is handled via `MainMenuController`

---

## Scene Flow (Current)
MainMenu
└── Instability
└── Immobility


---

## Scripts

### MainMenuController
- Attached to Main Menu Canvas
- Responsible only for menu navigation
- Handles scene loading via UI Button `OnClick()`

### InstabilityManager
- Attached to InstabilityManager
- Responsible for obstacles clearing and UI

### Obstacle2D
- Attached to Obstable Object 
- Make Interactable with Object using mouse possible with collider2D
- Swap Sprite between Fixed and Obstacle Visual

---

## Build Notes
- All scenes must be added to **Build Settings**
- Scene names must match exactly when used in button callbacks

---

## Development Notes 1
- Main Menu is intentionally simple
- No settings, save system, or audio manager implemented at this stage
- Focus is on completing gameplay scenes
## Development Notes 2
- Instability need a lot polishment and knowledge pop up is suck please fix it
- Temp picture is only joke we have no bad intention please forgive us if it make you upset
## Development Notes 3 
- Immobility Stage (In Progress)

- The Immobility stage is currently under active development and is being implemented according to the clinical scenario defined in the design document (bed-bound elderly patient with pressure injury risk).

- This stage focuses on nursing assessment and intervention, not player movement or action mechanics.

- Core mechanics under implementation include:

- Mouse/touch interaction to initiate assessment

- Multiple-choice question for pressure injury grading

- Drag-and-drop interaction for correct patient positioning using a pillow

- Immediate feedback for correct and incorrect actions

- Temporary placeholder visuals (simple shapes/sprites) are intentionally used to prioritize interaction logic, correctness, and learning flow over visual fidelity at this stage.

- Touch and mouse input are both supported to ensure compatibility with desktop testing and future mobile deployment.

- Further polishing (visual refinement, instructional text clarity, and UI feedback improvements) will be applied after the core interaction flow is validated.

## Change Log
update0.2 (Immobility Update)

-เพิ่ม Debugger รวมถึง cheat code เพื่อทดลองแต่ละเต็ปของเกม

-เพิ่ม Extras script เพื่อทำให้การ interact เสถียรกว่าเดิม (PatientClickTrigger,WoundClickTrigger)

-ปรับ script Immobility Stage Manager

    CheckList

    -ImmobilityStageManager GameObject:
    -ลาก PatientDummy (ที่มี SpriteRenderer) ใส่ช่อง Patient Renderer ✅
    -ลาก WoundClickArea ใส่ช่อง Wound Click Area ✅
    -ใส่รูป นอนตะแคง ในช่อง Lateral Sprite // รอดู sprite

    -PatientDummy GameObject:
    -แปะ Script PatientClickTrigger ✅
    -ใส่ BoxCollider2D (ให้ครอบคลุมตัวคน) ✅

    -WoundClickArea GameObject (ลูกของ Patient):
    -แปะ Script WoundClickTrigger ✅ 
    -ใส่ BoxCollider2D (ให้ครอบคลุมแผล) ✅

    -Pillow GameObject:
    -ปรับค่า Snap Distance ตามความเหมาะสม (แนะนำ 1.5 - 2) // รอดู sprite

update0.3 (Intellectual Impairment Update)

-ระบบ cutscene to quiz

-ระบบ interact กับ object เปลี่ยน sprite จากผิดเป็นถูก

-ระบบ Debugger

-เพิ่มปุ่ม replay สำหรับเล่น cutscene ใหม่

-Display UI ว่า object ในการเล่นหลังจากตอบคำถามคืออะไร

    CheckList

    Cutscene to Quiz
    -ลาก Panel_Cutscene, VideoPlayer, Panel_Quiz ใส่ให้ครบ
    -ที่ Panel_Quiz ลากปุ่มถูกใส่ช่อง Correct Button ลากปุ่มผิดใส่ Array Wrong Buttons ทีไม่แน่ใจอันไหนถูกผิด
    -หา font ที่ไม่มีลิขสิทธิ์มาใช้กับคำถามภาษาไทยด้วยเด้อ

    Object Interaction
    -วางของในฉาก (เช่น Lamp_Object) เป็น SpriteRenderer
    -คลิกที่ Object ที่ต้องการ -> ใส่ Script HazardObject -> ใส่ BoxCollider2D (เพื่อให้คลิกได้)
    -ลากรูป ที่แก้ไขหลังจากกด object มาใส่ในช่อง Fixed Sprite
    -ลากฉากใส่ Room_Background sprite แล้วก็ลาก sprite patient ใส่ patient
    -ปรับค่า size ใน main camera ด้วยเด้อ(อยู่ตรง Inpector -> Camera ) แบบลากฉากให้ตรงกับ camera แล้วก้ใส่ size ดูว่าฉากมันกว้างยาวแค่ได้ไหน
    -(optional) สามารถปรับสีพื้นหลังในกรณีที่หน้าจอเกมไม่ fit กับขอบพื้นหลังได้ที่ main camera -> environment -> Background เด้อ ไม่งั้นก้อาจจะเปลี่ยนจาก solid color เป็น skybox แล้วแต่ๆ 

update0.4 (Insomnia)

-ระบบตอบคำถาม + สุ่มคำตอบ

-ระบบ Next Stage for Insomnia (เหลือของ  Intellectual Impairment, Immobility , Instability)

    CheckList

    Sprite For UI
    -เช็ค sprite ง่ายๆผ่านไม่ผ่านใน Universal_Result
    -ขนาดหน้าจอ ปรับได้ตามเหมาะสม

update0.45 (Inanition)

-หน้า result สำหรับ Intellectual , Immobility

-หน้าต่างคำถามท้ายก่อน result ของ immobility

-หน้าต่าง Inanition ระบบภายในเสร็จแล้ว หัวใจ / จับผิด / ไปหน้าถัดไป / เริ่มใหม่

    CheckList

    Sprite For UI
    -เอา Sprite ใส่ให้หมดในหน้านี้เด้อตามชื่อใน hierarchy เลย


update0.5 (Incontinence)

-ระบบหลักเสร็จแล้วนะคิดว่า ถ้าอยากปรับอะไรปรับเลย

วิธีการกรอก Case ของผู้ป่วยนะ

กรอกข้อมูลทีละ Element (0 - 4):

Case Name: ตั้งชื่อเล่นให้จำง่าย (เช่น "ยายแมว Stress UI", "ลุงดำ Urge UI")

Patient Info: Copy ข้อมูลผู้ป่วยทั้งหมด (อายุ เพศ อาการ...) มาวางในช่องนี้

Correct Diagnosis Index: ใส่ตัวเลขลำดับปุ่มคำตอบที่ถูก (0-6)

0=Stress, 1=Urge, 2=Overflow, 3=Functional, 4=Mix(S+U), 5=Mix(U+F), 6=Mix(S+F)

Correct Intervention Indices:

นี่คือเฉลยการพยาบาล

กด Size ใส่จำนวนข้อที่ถูก (เช่น เคสนี้มีข้อถูก 3 ข้อ ก็ใส่ 3)

ใส่ Index ของปุ่ม ที่ถูก (0-9) ลงไป

(Tip: คุณต้องจดไว้ก่อนว่า ปุ่มที่ 1-10 ใน Scene ของคุณคือปุ่มอะไรบ้าง)

ตัวอย่างการกรอกข้อมูล (สมมติ)
Element 0 (เคส Stress UI):

Patient Info: "หญิง 60 ปี ไอจามแล้วปัสสาวะเล็ด..."

Correct Diagnosis Index: 0 (เพราะปุ่ม 0 คือ Stress UI)

Correct Intervention Indices:

Size: 2

Element 0: 0 (สมมติปุ่ม 0 คือ บริหารอุ้งเชิงกราน)

Element 1: 2 (สมมติปุ่ม 2 คือ ลดน้ำหนัก)

Element 1 (เคส Urge UI):

Patient Info: "ชาย 70 ปี ปวดฉี่แล้วราดเลย ไปไม่ทัน..."

Correct Diagnosis Index: 1 (เพราะปุ่ม 1 คือ Urge UI)

Correct Intervention Indices:

Size: 3

Element 0: 0 (บริหารอุ้งเชิงกราน)

Element 1: 1 (ฝึกกระเพาะปัสสาวะ)

Element 2: 5 (หลีกเลี่ยงคาเฟอีน)

---

## Unity Version
Unity 6.3 LTS (6000.3.0f1)

---

## Status
Main Menu implementation complete.  
Development continues in the **Instability** scene.