using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class LocomotionSystem : MonoBehaviour
{
    [SerializeField] private MovementDataSO data;

    private CharacterController _controller;
    private PlayerInputHandler _input;
    private Motor _motor;
    private float _currentHeight;
    private float _currentStamina;
    private float _staminaRegenCooldown;
    private float _currentTurnVelocity;
    private float _currentCarryWeight;
    private float _weightSpeedMod;
    private float _weightAccelMod;

    public event System.Action<MovementState> OnMovementStateChanged;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();
        _motor = new Motor(data);
        _currentStamina = data.maxStamina;
        _currentHeight = data.standHeight;
        _controller.height = _currentHeight;
        Vector3 center = _controller.center;
        center.y = _currentHeight / 2f;
        _controller.center = center;
        UpdateWeightModifiers();
    }

    private void Update()
    {
        _input.Refresh();

        bool isCrouching = _input.CrouchHeld;
        float targetHeight = isCrouching ? data.crouchHeight : data.standHeight;
        _currentHeight = Mathf.Lerp(_currentHeight, targetHeight, data.crouchTransitionSpeed * Time.deltaTime);
        _controller.height = _currentHeight;
        Vector3 center = _controller.center;
        center.y = _currentHeight / 2f;
        _controller.center = center;

        Vector3 inputDir = new Vector3(_input.MoveInput.x, 0f, _input.MoveInput.y).normalized;
        Vector3 desiredDirection = (transform.forward * inputDir.z + transform.right * inputDir.x);
        desiredDirection *= inputDir.magnitude;

        bool isGrounded = PerformGroundCheck(out Vector3 groundNormal);

        bool sprintRequested = _input.SprintHeld && !isCrouching;
        bool canSprint = _currentStamina > 0.05f;
        bool isSprinting = sprintRequested && canSprint;

        float staminaPercent = _currentStamina / data.maxStamina;
        MovementState state = _motor.Tick(Time.deltaTime, desiredDirection, _input.JumpPressedThisFrame, isSprinting,
                                          isCrouching, isGrounded, groundNormal, staminaPercent, out bool sprintAvailable);

        if (isSprinting && state.HorizontalVelocity.magnitude > 0.1f && isGrounded)
        {
            _currentStamina -= data.sprintDrainRate * Time.deltaTime;
            _staminaRegenCooldown = data.staminaRegenDelay;
            if (_currentStamina <= 0) isSprinting = false;
        }
        else if (_staminaRegenCooldown <= 0f && isGrounded)
        {
            _currentStamina += data.staminaRegenRate * Time.deltaTime;
        }
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, data.maxStamina);

        Vector3 movement = new Vector3(state.HorizontalVelocity.x, state.VerticalVelocity, state.HorizontalVelocity.z) * Time.deltaTime;
        _controller.Move(movement);

        float mouseX = _input.LookInput.x;
        float targetYaw = transform.eulerAngles.y + mouseX;
        float smoothYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref _currentTurnVelocity, data.turnSmoothing);
        transform.rotation = Quaternion.Euler(0f, smoothYaw, 0f);

        OnMovementStateChanged?.Invoke(state);
        _input.ConsumeJump();
    }

    private bool PerformGroundCheck(out Vector3 normal)
    {
        normal = Vector3.up;
        Vector3 footPos = transform.position + Vector3.down * (_controller.height * 0.5f);
        if (Physics.SphereCast(footPos + Vector3.up * data.groundCheckDistance, data.groundCheckRadius, Vector3.down, out RaycastHit hit, data.groundCheckDistance * 2f, data.groundLayers))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle <= data.slopeLimit)
            {
                normal = hit.normal;
                return true;
            }
        }
        return false;
    }

    public void SetCarryWeight(float newWeight)
    {
        _currentCarryWeight = newWeight;
        UpdateWeightModifiers();
    }

    private void UpdateWeightModifiers()
    {
        _weightSpeedMod = data.weightSpeedCurve.Evaluate(_currentCarryWeight);
        _weightAccelMod = data.weightAccelCurve.Evaluate(_currentCarryWeight);
        _motor.CurrentWeightModifier = _weightSpeedMod;
        _motor.CurrentAccelModifier = _weightAccelMod;
    }
}