using UnityEngine;

[CreateAssetMenu(menuName = "Movement/Data", fileName = "MovementData")]
public class MovementDataSO : ScriptableObject
{
    public float maxWalkSpeed = 5f;
    public float maxSprintSpeed = 8f;
    public float maxCrouchSpeed = 2.5f;
    public float acceleration = 12f;
    public float deceleration = 25f;
    public float stoppingDecelerationMultiplier = 2f;
    public float counterStrafeMultiplier = 5f;
    public float lateralDeceleration = 30f;
    public float airControl = 0.2f;
    public float turnSmoothing = 0.05f;
    public float jumpHeight = 2.5f;
    public float gravity = -9.81f;
    public float groundCheckRadius = 0.2f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayers = ~0;
    public float slopeLimit = 45f;
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchTransitionSpeed = 10f;
    public float baseCarryWeight = 0f;
    public float maxCarryWeight = 80f;
    public AnimationCurve weightSpeedCurve = AnimationCurve.Linear(0f, 1f, 80f, 0.3f);
    public AnimationCurve weightAccelCurve = AnimationCurve.Linear(0f, 1f, 80f, 0.2f);
    public float maxStamina = 100f;
    public float sprintDrainRate = 20f;
    public float staminaRegenRate = 15f;
    public float staminaRegenDelay = 0.5f;
}