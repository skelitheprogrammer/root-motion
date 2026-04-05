using UnityEngine;

public readonly struct MovementState
{
    public readonly Vector3 HorizontalVelocity;
    public readonly float VerticalVelocity;
    public readonly bool IsGrounded;
    public readonly Vector3 GroundNormal;
    public readonly float SpeedNormalized;
    public readonly bool JumpTriggered;
    public readonly bool IsJumping;
    public readonly float CurrentMaxSpeed;
    public readonly bool IsCrouching;

    public MovementState(Vector3 horizontalVel, float verticalVel, bool grounded, Vector3 groundNormal,
        float speedNormalized, bool jumpTriggered, bool isJumping, float currentMaxSpeed, bool isCrouching)
    {
        HorizontalVelocity = horizontalVel;
        VerticalVelocity = verticalVel;
        IsGrounded = grounded;
        GroundNormal = groundNormal;
        SpeedNormalized = speedNormalized;
        JumpTriggered = jumpTriggered;
        IsJumping = isJumping;
        CurrentMaxSpeed = currentMaxSpeed;
        IsCrouching = isCrouching;
    }
}