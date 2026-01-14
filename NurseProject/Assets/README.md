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

---

## Unity Version
Unity 6.3 LTS (6000.3.0f1)

---

## Status
Main Menu implementation complete.  
Development continues in the **Instability** scene.