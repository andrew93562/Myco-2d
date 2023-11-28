using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject
{
    [Header("LAYERS")]
    [Tooltip("Set this to the layer your player is on")]
    public LayerMask PlayerLayer;

    [Header("INPUT")]
    [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    public bool SnapInput = true;

    [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.3f;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;

    [Header("MOVEMENT")]
    [Tooltip("The top horizontal movement speed")]
    public float MaxSpeed = 14;

    [Tooltip("The player's capacity to gain horizontal speed")]
    public float Acceleration = 120;

    [Tooltip("The pace at which the player comes to a stop")]
    public float GroundDeceleration = 60;

    [Tooltip("Deceleration in air only after stopping input mid-air")]
    public float AirDeceleration = 30;

    [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
    public float GroundingForce = -1.5f;

    [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
    public float GrounderDistance = 0.05f;

    [Header("JUMP")]
    [Tooltip("The immediate velocity applied when jumping")]
    public float JumpPower = 36;

    [Tooltip("The maximum vertical movement speed")]
    public float MaxFallSpeed = 40;

    [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
    public float FallAcceleration = 110;

    [Tooltip("The gravity multiplier added when jump is released early")]
    public float JumpEndEarlyGravityModifier = 3;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public float CoyoteTime = .15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    public float JumpBuffer = .2f;

    [Tooltip("The amount of time the player needs to hold the button to count as a full hop")]
    public float HoldTime = 0.2f;

    [Tooltip("The amount of time before the increased gravity modifier takes effect on a short hop")]
    public float AirTime = 0.2f;

    [Tooltip("Affects how slowly the player falls while charging the projectile")]
    public float MouseHeldGravityModifier = 0.5f;

    [Tooltip("The amount of time it takes to charge the projectile one level")]
    public float ProjectileHoldTime = 0.2f;

    [Tooltip("The immediate velocity applied when firing a level 1 projectile")]
    public float ProjectileVelocity = 5f;

    [Tooltip("How long after firing a projectile before another may be fired")]
    public float ProjectileCooldown = 0.5f;

    [Tooltip("The amount of time jump must be held to count as a full hop")]
    public float JumpSquat = 0.5f;

    [Tooltip("Immediate upward velocity applied on a short hop")]
    public float ShortHopPower = 15;

    [Tooltip("Immediate upward velocity applied on a full hop")]
    public float FullHopPower = 30;


}