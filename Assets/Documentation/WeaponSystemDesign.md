# Weapon System Design Document

## Overview
This document outlines the design for a modular weapon system that supports multiple weapon types and slot-based equipment management.

## Weapon Slots
The player has 4 dedicated weapon slots:
1. Primary Weapon (Machine Guns, Rifles)
2. Secondary Weapon (Pistols, Small Arms)
3. Melee Weapon (Knives, Batons)
4. Throwable Items (Grenades, Consumables)

## Weapon Categories

### 1. Machine Guns
- High rate of fire
- Large magazine capacity
- Higher recoil
- Lower accuracy at range
- Examples: M249, MP5, UMP45

### 2. Rifles
- Balanced rate of fire
- Medium magazine capacity
- Controlled recoil
- High accuracy
- Examples: M4A1, AK-47, SCAR-H

### 3. Shotguns
- Low rate of fire
- Spread pattern projectiles
- High close-range damage
- Limited effective range
- Examples: M870, SPAS-12

### 4. Throwables
- Single-use items
- Various effects (damage, utility)
- Arc trajectory system
- Examples: Frag Grenades, Flashbangs

### 5. Melee Weapons
- Close-range combat
- Quick deployment
- No ammunition requirement
- Examples: Combat Knife, Tactical Baton

## Weapon Properties

### Common Properties
- Name
- Description
- Weight
- Rarity
- Icon/Model Reference
- Slot Type (Primary/Secondary/Melee/Throwable)

### Firearm-Specific Properties
- Damage
- Fire Rate
- Magazine Capacity
- Reserve Ammo
- Reload Time
- Accuracy
- Recoil Pattern
- Range
- Bullet Velocity
- Penetration

### Melee-Specific Properties
- Damage
- Attack Speed
- Attack Range
- Combo System

### Throwable-Specific Properties
- Effect Radius
- Effect Duration
- Throw Force
- Cooking Time (if applicable)

## Animation System

### Required Animation Clips
1. Attack/Fire Animations
   - Primary Attack
   - Secondary Attack (if applicable)
   - Charge Attack (for melee)

2. Reload Animations
   - Tactical Reload (magazine not empty)
   - Full Reload (magazine empty)
   - Reload Cancel

3. Movement Animations
   - Idle
   - Walk
   - Run
   - Jump
   - Crouch

4. Inspection Animations
   - Inspect Start (detailed weapon examination)
   - Inspect Idle (looping inspection state)
   - Inspect End (return to normal state)

5. State Change Animations
   - Switch In (equip)
   - Switch Out (unequip)
   - Quick Switch (emergency weapon swap)

### Animation States
```
WeaponAnimationState
├── Idle
├── Moving
│   ├── Walking
│   ├── Running
│   └── Crouching
├── Acting
│   ├── Firing
│   ├── Reloading
│   └── Meleeing
├── Inspecting
│   ├── InspectStart
│   ├── InspectIdle
│   └── InspectEnd
└── Transitioning
    ├── SwitchingIn
    └── SwitchingOut
```

## Weapon State Machine
```
WeaponState
├── Idle
├── Ready
├── Firing
├── Reloading
├── Switching
└── Disabled
```

## Events System

### Weapon Events
- OnWeaponEquipped
- OnWeaponUnequipped
- OnFireStart
- OnFireEnd
- OnReloadStart
- OnReloadEnd
- OnAmmoChanged
- OnWeaponJammed
- OnStateChanged
- OnAttachmentMounted
- OnAttachmentRemoved
- OnAttachmentEffectApplied
- OnWeaponStatsUpdated

### Inventory Events
- OnSlotChanged
- OnWeaponAdded
- OnWeaponRemoved
- OnAmmoReplenished

## Audio System Integration

### Sound Categories
1. Weapon Actions
   - Fire Sounds
   - Reload Sounds
   - Equip/Unequip Sounds
   - Empty Magazine Clicks

2. Impact Sounds
   - Surface Hit Effects
   - Flesh Impact
   - Metal Impact
   - Wood Impact

3. Movement Sounds
   - Weapon Sway
   - Running with Weapon
   - Weapon Collisions

## Effects System

### Visual Effects
1. Muzzle Effects
   - Muzzle Flash
   - Smoke
   - Shell Ejection

2. Impact Effects
   - Bullet Holes
   - Surface Particles
   - Blood Effects

3. Environmental Effects
   - Brass Casings
   - Magazine Drops
   - Weapon Trails

## Interaction System

### Player Input Handling
1. Primary Actions
   - Fire (Hold/Tap)
   - Aim Down Sights
   - Reload
   - Weapon Switch

2. Secondary Actions
   - Alternative Fire Mode
   - Weapon Inspect
   - Drop Weapon
   - Quick Melee

## Implementation Considerations

### Performance Optimization
- Object Pooling for projectiles and effects
- LOD system for weapon models
- Animation culling when not visible
- Sound optimization for multiple weapons

### Networking Considerations
- Weapon state synchronization
- Hit registration and validation
- Latency compensation
- Prediction for weapon switching

## Attachment System

### Attachment Slots
1. Optics
   - Red Dot Sights
   - Holographic Sights
   - Telescopic Scopes (2x-8x)
   - Thermal Scopes
   - Iron Sights (default)

2. Muzzle
   - Suppressors
   - Flash Hiders
   - Compensators
   - Muzzle Brakes

### Attachment Properties
1. Base Properties
   - Weight
   - Installation Time
   - Compatibility List

2. Optics Properties
   - Zoom Level
   - ADS Speed Modifier
   - Field of View Modifier
   - Sight Picture Type

3. Muzzle Properties
   - Sound Reduction Level
   - Recoil Impact
   - Muzzle Flash Reduction
   - Velocity Modifier

4. Visual Properties
   - Model Reference
   - Attachment Points
   - Special Effects
   - Custom Animations

### Attachment Effects

#### Optics
- Zoom Level Modification
- Aim Down Sight Speed
- Peripheral Vision
- Special Features (Night Vision, Thermal)

#### Suppressors
- Sound Reduction
- Velocity Modification
- Range Impact
- Heat Management
- Flash Concealment

### Implementation Details

#### Attachment Management
1. Mounting System
   - Rail System Types
   - Compatibility Checks
   - Quick-Release Options
   - Multi-Mount Support

2. State Handling
   - Attachment Installation
   - Removal Process
   - State Persistence
   - Configuration Save/Load

3. Visual Updates
   - Real-time Model Changes
   - Effect Integration
   - Animation Adjustments
   - UI Representation

### Future Expandability
- Advanced Optics Integration
- Enhanced Suppressor Types
- Hybrid Attachments
- Attachment Presets System

## Aiming and Shooting System

### Aiming System
1. Weapon States
   - Hip Position
     * Default weapon position and rotation
     * Wider crosshair spread
     * Full movement speed
   
   - ADS Position
     * LeanTween transition to aimed position
     * Precise aiming point
     * Reduced movement speed
     * Optics integration

2. Position Control
   - Transform References
     * Weapon hold position
     * ADS position target
     * Camera alignment point
   
   - Transition Control
     * LeanTween easing functions
     * Customizable transition speed
     * Interrupt handling
     * State restoration

### Shooting System
1. Fire Control
   - Fire Modes
     * Single Shot
     * Burst Fire (2-3 rounds)
     * Full Auto
   
   - Raycast Implementation
     * Camera-based aim point
     * Layer-based collision
     * Range limits
     * Hit detection

2. Accuracy Control
   - Spread Factors
     * Base weapon spread
     * Movement penalty
     * ADS bonus
     * Stance modifiers
   
   - Recoil System
     * Visual kick
     * Screen shake
     * Recovery speed
     * Pattern definition

### Shooting Implementation
1. Raycast System
   - Instant hit detection
   - Layer-based targeting
   - Hit point calculation
   - Surface detection
   - Damage application

2. Weapon Positioning
   - LeanTween position transitions
     * Normal to ADS position
     * Smooth interpolation
     * Configurable duration
   - LeanTween rotation handling
     * Weapon alignment
     * Scope alignment
     * Recoil rotation

3. Movement Integration
   - Position updates while moving
   - Stance-based positioning
   - Movement speed effects
   - Direction handling

### Visual Feedback
1. Weapon Positioning
   - Normal stance view
   - ADS view position
   - Transition animations
   - Recoil visualization

2. Combat Effects
   - Hit markers
   - Impact effects
   - Muzzle flash
   - Shell ejection

3. UI Elements
   - Crosshair types
     * Hip fire
     * ADS
     * Different weapon types
   - Hit indicators
   - Damage feedback
   - Ammo counter

## Data Management

### Scriptable Objects
1. WeaponData
   - Base weapon properties
   - Animation references
   - Sound references
   - Effect references
   - Available attachment slots
   - Default attachments
   - Attachment mounting points
   - Stat modification handlers

2. AttachmentData
   - Attachment type and category
   - Stat modifiers
   - Visual assets
   - Sound effects
   - Compatible weapons list
   - Required mounting points
   - Special effect handlers
   - Custom animation overrides

2. AmmoData
   - Ammo types
   - Damage profiles
   - Effect properties

### Save System Integration
- Weapon loadout persistence
- Ammo state saving
- Weapon modifications
- Player preferences