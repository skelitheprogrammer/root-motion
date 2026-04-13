using Code.Movement;
using UnityEngine;
namespace Code
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _bodyTransform;
        private Locomotion _locomotion;
        private PlayerInputHandler _inputs;
        private BodyRotation _bodyRotation;
        private float _currentStance;
        private float _scrollBuffer;

        private static readonly int SPEED_HASH = Animator.StringToHash("Speed");
        private static readonly int STANCE_HASH = Animator.StringToHash("Stance");
        private static readonly int FORWARD_HASH = Animator.StringToHash("Forward");
        private static readonly int STRAFE_HASH = Animator.StringToHash("Strafe");
        private static readonly int TURN_ANGLE_HASH = Animator.StringToHash("TurnAngle");
        private static readonly int TURN_LEFT_HASH = Animator.StringToHash("TurnLeft");
        private static readonly int TURN_RIGHT_HASH = Animator.StringToHash("TurnRight");

        private void Awake()
        {
            _locomotion = GetComponent<Locomotion>();
            _inputs = GetComponent<PlayerInputHandler>();
            _bodyRotation = GetComponent<BodyRotation>();
        }

        private void Update()
        {
            _animator.ResetTrigger(TURN_LEFT_HASH);
            _animator.ResetTrigger(TURN_RIGHT_HASH);
            UpdateStanceParameter(_inputs.StanceDelta, _inputs.CrouchHeld);
            UpdateMovementParameters(_locomotion.Velocity, _locomotion.NormalizedTopSpeed);
            UpdateTurnParameter(_bodyRotation.AngleVelocity);
        }

        private void UpdateMovementParameters(Vector3 worldVelocity, float maxSpeed)
        {
            const float DEADZONE = 0.01f;
    
            if (worldVelocity.sqrMagnitude < DEADZONE)
            {
                _animator.SetFloat(FORWARD_HASH, 0f, 0.1f, Time.deltaTime);
                _animator.SetFloat(STRAFE_HASH, 0f, 0.1f, Time.deltaTime);
                _animator.SetFloat(SPEED_HASH, 0f, 0.1f, Time.deltaTime);
                return;
            }

            Transform facingRef = _bodyTransform ?? transform;
            Vector3 localVel = facingRef.InverseTransformDirection(worldVelocity);

            Vector2 dir2D = new Vector2(localVel.x, localVel.z).normalized;
    
            _animator.SetFloat(FORWARD_HASH, dir2D.y, 0.08f, Time.deltaTime);
            _animator.SetFloat(STRAFE_HASH, dir2D.x, 0.08f, Time.deltaTime);
    
            float speedRatio = Mathf.Clamp01(worldVelocity.magnitude / Mathf.Max(maxSpeed, 0.01f));
            _animator.SetFloat(SPEED_HASH, speedRatio, 0.1f, Time.deltaTime);
        }

        private void UpdateTurnParameter(float angularVelocity, float maxAngularVelocity = 180f)
        {
            float normalized = Mathf.Clamp(angularVelocity / maxAngularVelocity, -1f, 1f);
            _animator.SetFloat(TURN_ANGLE_HASH, normalized, 0.05f, Time.deltaTime);

            if (Mathf.Abs(normalized) > 0.8f)
            {
                _animator.SetTrigger(normalized > 0 ? TURN_RIGHT_HASH : TURN_LEFT_HASH);
            }
        }

        private void UpdateStanceParameter(float rawScrollDelta, bool wantsCrouch)
        {
            float delta = rawScrollDelta;
            delta = Mathf.Clamp(delta, -0.05f, 0.05f);

            _currentStance += delta;
            _currentStance = Mathf.Clamp01(_currentStance);
            _animator.SetFloat(STANCE_HASH, _currentStance);
        }
    }
}