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
(Future scenes will be added after Instability is completed.)

---

## Scripts

### MainMenuController
- Attached to Main Menu Canvas
- Responsible only for menu navigation
- Handles scene loading via UI Button `OnClick()`
- must assign the scene in (file -> build profiles -> open scene list -> add open scenes)

---

## Build Notes
- All scenes must be added to **Build Settings**
- Scene names must match exactly when used in button callbacks

---

## Development Notes
- Main Menu is intentionally simple
- No settings, save system, or audio manager implemented at this stage
- Focus is on completing gameplay scenes

---

## Unity Version
Unity 6.3 LTS (6000.3.0f1)

---

## Status
Main Menu implementation complete.  
Development continues in the **Instability** scene.