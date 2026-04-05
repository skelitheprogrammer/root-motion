using UnityEngine;

public readonly struct MovementState
{
    public readonly Vector3 HorizontalVelocity;
    public readonly float VerticalVelocity;
    public readonly bool IsGrounded;
    public readonly float SpeedNormalized;
    public readonly bool JumpTriggered;
    public readonly bool IsJumping;
    public readonly float CurrentMaxSpeed;
    public readonly bool IsCrouching;
    public readonly bool IsSprinting;
    public readonly float MaxWalkSpeed;
    public readonly float MaxSprintSpeed;
    public readonly float MaxCrouchSpeed;

    public MovementState(Vector3 horizontalVel, float verticalVel, bool grounded,
        float speedNormalized, bool jumpTriggered, bool isJumping,
        float currentMaxSpeed, bool isCrouching, bool isSprinting,
        float maxWalkSpeed, float maxSprintSpeed, float maxCrouchSpeed)
    {
        HorizontalVelocity = horizontalVel;
        VerticalVelocity = verticalVel;
        IsGrounded = grounded;
        SpeedNormalized = speedNormalized;
        JumpTriggered = jumpTriggered;
        IsJumping = isJumping;
        CurrentMaxSpeed = currentMaxSpeed;
        IsCrouching = isCrouching;
        IsSprinting = isSprinting;
        MaxWalkSpeed = maxWalkSpeed;
        MaxSprintSpeed = maxSprintSpeed;
        MaxCrouchSpeed = maxCrouchSpeed;
    }
}