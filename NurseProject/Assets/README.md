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

---

## Unity Version
Unity 6.3 LTS (6000.3.0f1)

---

## Status
Main Menu implementation complete.  
Development continues in the **Instability** scene.