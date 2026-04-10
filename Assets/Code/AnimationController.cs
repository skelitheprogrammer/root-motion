using Code.Movement;
using UnityEngine;
namespace Code
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private Locomotion _locomotion;
        private PlayerInputHandler _inputs;
        private BodyRotation _bodyRotation;
        private float _currentStance;

        private static readonly int SPEED_HASH = Animator.StringToHash("Speed");
        private static readonly int STANCE_HASH = Animator.StringToHash("Stance");
        private static readonly int FORWARD_HASH = Animator.StringToHash("Forward");
        private static readonly int STRAFE_HASH = Animator.StringToHash("Strafe");
        private static readonly int TURN_ANGLE_HASH = Animator.StringToHash("TurnAngle");

        private void Awake()
        {
            _locomotion = GetComponent<Locomotion>();
            _inputs = GetComponent<PlayerInputHandler>();
            _bodyRotation = GetComponent<BodyRotation>();
        }

        private void Update()
        {
            UpdateStanceParameter(_inputs.StanceDelta, _inputs.CrouchHeld);
            UpdateMovementParameters(_locomotion.Velocity, _locomotion.Speed);
            UpdateTurnParameter(_bodyRotation.AngleDelta, _bodyRotation.Threshold);
        }

        private void UpdateMovementParameters(Vector3 velocity, float speed)
        {
            Vector3 direction = transform.InverseTransformDirection(velocity.normalized);
            _animator.SetFloat(FORWARD_HASH, direction.z);
            _animator.SetFloat(STRAFE_HASH, direction.x);

            float speedRatio = velocity.magnitude / speed;

            _animator.SetFloat(SPEED_HASH, speedRatio);
        }

        private void UpdateTurnParameter(float turnAngleDelta, float maxTurnDeltaAngle)
        {
            float delta = Mathf.Clamp(turnAngleDelta / maxTurnDeltaAngle, -1, 1);
            _animator.SetFloat(TURN_ANGLE_HASH, delta);
        }

        private void UpdateStanceParameter(float stanceDelta, bool wantsCrouch)
        {

            // float targetStance = wantsCrouch ? 1f : 0f;
            // _currentStance = Mathf.MoveTowards(_currentStance, targetStance, _stanceDamping * Time.deltaTime);
            // _animator.SetFloat(STANCE_HASH, _currentStance, _stanceDamping, Time.deltaTime);

            if (Mathf.Abs(stanceDelta) < 0.001f) return;
            const float stepSize = 0.5f;
            _currentStance = Mathf.Round(_currentStance / stepSize) * stepSize;
            _currentStance = Mathf.Clamp01(_currentStance);
            _animator.SetFloat(STANCE_HASH, _currentStance);
        }
    }
}