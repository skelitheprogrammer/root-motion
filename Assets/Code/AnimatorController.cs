using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour
{
    [SerializeField] private LocomotionSystem locomotion;

    private Animator _animator;
    private Transform _characterTransform;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int DirectionHash = Animator.StringToHash("Direction");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int JumpingHash = Animator.StringToHash("Jumping");
    private static readonly int CrouchHash = Animator.StringToHash("Crouch");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterTransform = transform;
        if (locomotion is not null)
            locomotion.OnMovementStateChanged += OnMovementStateChanged;
    }

    private void OnMovementStateChanged(MovementState state)
    {
        if (_animator is null) return;

        Vector3 localVel = _characterTransform.InverseTransformDirection(state.HorizontalVelocity);
        float forwardSpeed = localVel.z;
        float rightSpeed = localVel.x;
        float absoluteSpeed = Mathf.Abs(forwardSpeed);

        float speedParam = 0f;

        if (state.IsSprinting)
        {
            if (absoluteSpeed <= state.MaxWalkSpeed)
                speedParam = 0.5f * (absoluteSpeed / state.MaxWalkSpeed);
            else
                speedParam = 0.5f + 0.5f * ((absoluteSpeed - state.MaxWalkSpeed) / (state.MaxSprintSpeed - state.MaxWalkSpeed));
        }
        else if (state.IsCrouching)
        {
            speedParam = 0.3f * Mathf.Clamp01(absoluteSpeed / state.MaxCrouchSpeed);
        }
        else
        {
            speedParam = 0.5f * Mathf.Clamp01(absoluteSpeed / state.MaxWalkSpeed);
        }

        float maxDir = state.IsSprinting ? state.MaxSprintSpeed : (state.IsCrouching ? state.MaxCrouchSpeed : state.MaxWalkSpeed);
        float directionParam = Mathf.Clamp(rightSpeed / maxDir, -1f, 1f);

        _animator.SetFloat(SpeedHash, speedParam);
        _animator.SetFloat(DirectionHash, directionParam);
        _animator.SetBool(GroundedHash, state.IsGrounded);
        if (state.JumpTriggered)
            _animator.SetTrigger(JumpHash);
        _animator.SetBool(JumpingHash, state.IsJumping);
        _animator.SetBool(CrouchHash, state.IsCrouching);
    }

    private void OnDestroy()
    {
        if (locomotion is not null)
            locomotion.OnMovementStateChanged -= OnMovementStateChanged;
    }
}