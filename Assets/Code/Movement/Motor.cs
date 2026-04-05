using UnityEngine;

public class Motor
{
    private readonly MovementDataSO _data;
    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;

    public float CurrentWeightModifier { get; set; } = 1f;

    public float CurrentAccelModifier { get; set; } = 1f;

    public Motor(MovementDataSO data)
    {
        _data = data;
    }

    public MovementState Tick(float deltaTime, Vector3 desiredDirection, bool jumpRequested, bool isSprinting,
        bool isCrouching, bool isGrounded, Vector3 groundNormal,
        float staminaPercent, out bool canSprint)
    {
        float baseMaxSpeed = isCrouching ? _data.maxCrouchSpeed : (isSprinting ? _data.maxSprintSpeed : _data.maxWalkSpeed);
        float weightSpeedMod = _data.weightSpeedCurve.Evaluate(_data.baseCarryWeight);
        float maxSpeed = baseMaxSpeed * weightSpeedMod * CurrentWeightModifier;

        float accelRate = _data.acceleration * CurrentAccelModifier;
        float decelRate = _data.deceleration * CurrentAccelModifier;

        Vector3 newHorizontal = UpdateHorizontal(_horizontalVelocity, desiredDirection, maxSpeed, accelRate, decelRate, isGrounded, deltaTime);
        bool jumpValid = jumpRequested && isGrounded && !isCrouching;
        float newVertical = UpdateVertical(_verticalVelocity, jumpValid, isGrounded, deltaTime);

        canSprint = staminaPercent > 0.05f && !isCrouching;

        float speedNorm = maxSpeed > 0f ? newHorizontal.magnitude / maxSpeed : 0f;
        bool isJumping = newVertical > 0f && !isGrounded;

        _horizontalVelocity = newHorizontal;
        _verticalVelocity = newVertical;

        return new MovementState(newHorizontal, newVertical, isGrounded, groundNormal, speedNorm, jumpRequested, isJumping, maxSpeed, isCrouching);
    }

    private Vector3 UpdateHorizontal(Vector3 current, Vector3 desired, float maxSpeed, float accelRate, float decelRate, bool grounded, float dt)
    {
        if (desired.magnitude < 0.05f)
        {
            if (current.magnitude < 0.01f) return Vector3.zero;
            float speedFactor = Mathf.Clamp01(current.magnitude / maxSpeed);
            float effectiveDecel = decelRate * _data.stoppingDecelerationMultiplier * (1f + speedFactor);
            float newSpeed = Mathf.MoveTowards(current.magnitude, 0f, effectiveDecel * dt);
            return current.normalized * newSpeed;
        }

        Vector3 wishDir = desired.normalized;

        float forwardSpeed = Vector3.Dot(current, wishDir);
        Vector3 lateralVel = current - wishDir * forwardSpeed;

        float lateralDecel = _data.lateralDeceleration * (grounded ? 1f : _data.airControl);
        lateralVel = Vector3.MoveTowards(lateralVel, Vector3.zero, lateralDecel * dt);

        float targetForwardSpeed = maxSpeed * desired.magnitude;
        float newForwardSpeed = Mathf.MoveTowards(forwardSpeed, targetForwardSpeed, accelRate * dt);

        Vector3 newVel = wishDir * newForwardSpeed + lateralVel;

        if (forwardSpeed < -0.5f && desired.magnitude > 0.5f)
        {
            float speedFactor = Mathf.Clamp01(current.magnitude / maxSpeed);
            float extraBrake = decelRate * _data.counterStrafeMultiplier * (1f + speedFactor);
            newForwardSpeed = Mathf.MoveTowards(forwardSpeed, targetForwardSpeed, extraBrake * dt);
            newVel = wishDir * newForwardSpeed + lateralVel;
        }

        if (newVel.magnitude > maxSpeed)
            newVel = newVel.normalized * maxSpeed;

        return newVel;
    }

    private float UpdateVertical(float current, bool jumpRequested, bool grounded, float dt)
    {
        float newY = current;
        if (grounded && newY < 0f) newY = -2f;
        if (jumpRequested && grounded) newY = Mathf.Sqrt(2f * _data.jumpHeight * -_data.gravity);
        newY += _data.gravity * dt;
        return newY;
    }
}