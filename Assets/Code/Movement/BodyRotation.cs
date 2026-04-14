using UnityEngine;
namespace Code.Movement
{
    public class BodyRotation : MonoBehaviour
    {
        [SerializeField] private PlayerInputHandler _inputHandler;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _bodyTransform;

        [Range(0.08f, 0.25f)]
        [SerializeField] private float _smoothTime = 0.15f;

        [SerializeField] private float _movingTurnRate = 140f;
        [SerializeField] private float _idleTurnRate = 60f;

        [Range(5f, 30f)]
        [SerializeField] private float _movingEnterThreshold = 12f;

        [Range(2f, 15f)]
        [SerializeField] private float _movingExitThreshold = 6f;

        [Range(15f, 90f)]
        [SerializeField] private float _idleEnterThreshold = 25f;

        [Range(5f, 20f)]
        [SerializeField] private float _idleExitThreshold = 12f;

        private float _currentBodyYaw;
        private float _rotationVelocity;
        private bool _isRotating;

        public float AngleDelta { get; private set; }

        public float AngleVelocity => _rotationVelocity;

        public float Threshold => _idleEnterThreshold;

        public bool IsRotating { get; private set; }

        public bool IsMoving { get; private set; }

        private void Awake()
        {
            _currentBodyYaw = _bodyTransform.eulerAngles.y;
        }

        private void Update()
        {
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            Vector2 moveInput = _inputHandler.Current.Move;
            IsMoving = moveInput.sqrMagnitude > 0.01f;

            float targetYaw = _currentBodyYaw;
            float enterThreshold, exitThreshold, turnRate;

            if (IsMoving)
            {
                Vector3 camF = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up);
                Vector3 camR = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up);
                Vector3 moveDir = camF * moveInput.y + camR * moveInput.x;

                if (moveDir.sqrMagnitude > 0.001f)
                {
                    targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
                }

                enterThreshold = _movingEnterThreshold;
                exitThreshold = _movingExitThreshold;
                turnRate = _movingTurnRate;
            }
            else
            {
                Vector3 camF = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up);
                if (camF.sqrMagnitude > 0.001f)
                {
                    targetYaw = Mathf.Atan2(camF.x, camF.z) * Mathf.Rad2Deg;
                }

                enterThreshold = _idleEnterThreshold;
                exitThreshold = _idleExitThreshold;
                turnRate = _idleTurnRate;
            }

            AngleDelta = Mathf.DeltaAngle(_currentBodyYaw, targetYaw);

            bool shouldRotate = Mathf.Abs(AngleDelta) > enterThreshold || _isRotating && Mathf.Abs(AngleDelta) > exitThreshold;

            _isRotating = shouldRotate;
            IsRotating = _isRotating;

            if (_isRotating)
            {
                _currentBodyYaw = Mathf.SmoothDampAngle(
                    _currentBodyYaw, targetYaw, ref _rotationVelocity,
                    _smoothTime, turnRate, Time.deltaTime
                );
            }
            else
            {
                _rotationVelocity = 0f;
            }

            Vector3 euler = _bodyTransform.eulerAngles;
            _bodyTransform.rotation = Quaternion.Euler(euler.x, _currentBodyYaw, euler.z);
        }
    }
}