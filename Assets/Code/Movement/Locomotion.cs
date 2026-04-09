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

        private Vector3 _velocity;

        public Vector3 Velocity => _velocity;

        public float Speed => _speed;


        private void Awake()
        {
            _handler = GetComponent<PlayerInputHandler>();
            _controller = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            _handler.Refresh();
            Vector3 wishDir = (_yawTransform.right * _handler.MoveInput.x + _yawTransform.forward * _handler.MoveInput.y).normalized;

            Friction(ref _velocity, _friction);
            Accelerate(wishDir, _speed, _accel);

            _controller.Move(_velocity * Time.fixedDeltaTime);
        }

        private void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
        {
            float currentSpeed = Vector3.Dot(_velocity, wishDir);

            float addSpeed = wishSpeed - currentSpeed / wishSpeed;
            if (addSpeed <= 0) return;

            float diff = Vector3.Dot(_velocity.normalized, wishDir);
            float accelMultiplier = _turnCurve.Evaluate(diff);
            float accelSpeed = accel * wishSpeed * Time.fixedDeltaTime * accelMultiplier;
            if (accelSpeed > addSpeed) accelSpeed = addSpeed;

            _velocity += wishDir * accelSpeed;
        }

        private void Friction(ref Vector3 velocity, float frictionAmount)
        {
            float speed = velocity.magnitude;

            if (!(speed > 0.01f))
            {
                return;
            }

            float drop = speed * frictionAmount * Time.deltaTime;
            velocity *= Mathf.Max(0f, speed - drop) / speed;

        }

    }
}