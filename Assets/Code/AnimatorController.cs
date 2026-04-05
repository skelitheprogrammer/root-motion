using UnityEngine;
namespace Code
{
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

            Vector3 localVelocity = _characterTransform.InverseTransformDirection(state.HorizontalVelocity);

            float normalizedSpeed = 0f;
            float normalizedDirection = 0f;
            if (state.CurrentMaxSpeed > 0f)
            {
                normalizedSpeed = Mathf.Clamp(localVelocity.z / state.CurrentMaxSpeed, -1f, 1f);
                normalizedDirection = Mathf.Clamp(localVelocity.x / state.CurrentMaxSpeed, -1f, 1f);
            }

            _animator.SetFloat(SpeedHash, normalizedSpeed);
            _animator.SetFloat(DirectionHash, normalizedDirection);
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
}