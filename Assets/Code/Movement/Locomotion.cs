using UnityEngine;
namespace Code.Movement
{
    public class Locomotion : MonoBehaviour
    {
        [SerializeField] private Transform _yawTransform;
        [SerializeField] private float _speed;
        [SerializeField] private float _accel;
        [SerializeField] private float _friction;
        [SerializeField] private AnimationCurve _turnCurve = AnimationCurve.EaseInOut(-1f, 0.1f, 1f, 1f);

        private PlayerInputHandler _handler;
        private CharacterController _controller;

        public float NormalizedTopSpeed { get; private set; }

        private float _measuredTopSpeed;

        private Vector3 _velocity;

        public Vector3 Velocity => _velocity;

        private void Awake()
        {
            _handler = GetComponent<PlayerInputHandler>();
            _controller = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            Vector3 wishDir = (_yawTransform.right * _handler.Current.Move.x +
                               _yawTransform.forward * _handler.Current.Move.y).normalized;

            Friction(ref _velocity, wishDir, _friction);
            Accelerate(wishDir, _speed, _accel);

            _controller.Move(_velocity * Time.fixedDeltaTime);

            if (_velocity.sqrMagnitude <= 0.001f)
                _velocity = Vector3.zero;

            if (_velocity.magnitude > _measuredTopSpeed)
                _measuredTopSpeed = _velocity.magnitude;

            NormalizedTopSpeed = Mathf.Max(_measuredTopSpeed, _speed * 0.85f);
        }

        private void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
        {
            if (wishDir.sqrMagnitude < 0.001f) return;

            float currentSpeed = Vector3.Dot(_velocity, wishDir);
            float addSpeed = wishSpeed - currentSpeed;
            if (addSpeed <= 0) return;

            float alignment = Vector3.Dot(_velocity.normalized, wishDir);
            float accelMultiplier = _turnCurve.Evaluate(Mathf.Clamp01(alignment));

            float accelDelta = accel * wishSpeed * Time.fixedDeltaTime * accelMultiplier;
            if (accelDelta > addSpeed) accelDelta = addSpeed;

            _velocity += wishDir * accelDelta;
        }

        private void Friction(ref Vector3 velocity, Vector3 wishDir, float frictionAmount)
        {
            float speed = velocity.magnitude;
            if (speed < 0.01f) return;

            bool hasInput = wishDir.sqrMagnitude > 0.001f;
            float alignment = hasInput ? Vector3.Dot(velocity.normalized, wishDir) : -1f;
            bool movingWithInput = alignment > 0.7f;

            float frictionMult = (hasInput && movingWithInput) ? 0.15f : 1.0f;
            float drop = speed * frictionAmount * Time.fixedDeltaTime * frictionMult;

            velocity *= Mathf.Max(0f, speed - drop) / speed;
        }

    }
}