# Test1 — Unity Gameplay Systems Project

A third-person Unity gameplay project focused on **modular gameplay architecture, AI behavior, player systems, and maintainable state-driven design**.

The project was built to explore production-oriented approaches to gameplay programming rather than concentrating solely on content or visual presentation.
<br>
## What This Project Demonstrates

**Unity gameplay engineering**
- decomposing gameplay features into maintainable systems
- designing finite-state-machine architectures
- coordinating AI navigation and gameplay behavior
- managing dependencies between gameplay components
- building systems that can be extended without rewriting core controllers
  <br>
## Highlights

- **Player state machine** for movement and gameplay behavior
- **AI finite-state machine**
- **NavMesh-based AI navigation**
- Component-oriented architecture designed to reduce tightly coupled `MonoBehaviour` logic
- Player movement using `CharacterController`
- Unity **Input System**
- Combat, health, knockback, and weapon interactions
- Animation and audio integration
- Centralized game-state management
- Event-driven UI and gameplay communication
  <br>
## Architecture

Gameplay systems are separated by responsibility rather than implemented inside large controller classes.
<br>
```text
Player
├── Input
├── Movement
├── Combat
└── State Machine

AI
├── State Machine
│   ├── Patrol
│   ├── Chase
│   ├── Attack
│   ├── Stun
│   └── Death
├── Navigation
├── Combat
├── Animation
└── Status / Health

Game
├── Game State
├── UI
└── Gameplay Events
```
<br>
AI states interact with capabilities through focused interfaces such as movement, animation, sound, and status components.<br>
This keeps individual states small and allows behavior to evolve without concentrating all AI logic into a single class.
<br>
## Engineering Approach

The project emphasizes several principles I use when building gameplay systems:

- **Composition over inheritance**
- Clear separation of responsibilities
- Explicit state transitions
- Event-driven communication where appropriate
- Minimal unnecessary `Update()` polling
- Small, replaceable gameplay components
- Separation between navigation, state logic, presentation, and combat behavior

The goal is to keep systems understandable as gameplay complexity increases.
<br>
## Technology

- **Unity 6**
- **C#**
- Universal Render Pipeline (URP)
- AI NavMesh
  
  <br>
## Project Status

This is a gameplay engineering / architecture project rather than a commercially released game. 
Its primary purpose is demonstrating implementation patterns and gameplay-system design.
