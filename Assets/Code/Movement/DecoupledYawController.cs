using Code;
using UnityEngine;
using UnityEngine.Serialization;

public class DecoupledYawController : MonoBehaviour
{
    [SerializeField] private Transform _cameraYawPivot;
    [SerializeField] private PlayerInputHandler _inputs;

    [SerializeField] private float _enterThreshold = 90f;
    [SerializeField] private float _rotationSpeed = 600f;
    [SerializeField] private float _completionDeadzone = 1.5f;
    [SerializeField] private float _movementAlignSpeed = 250f;
    [SerializeField] private float _movementInputDeadzone = 0.01f;

    private float _cameraWorldYaw;
    private float _bodyWorldYaw;
    private bool _isInterpolating;
    private bool _isMoving;

    private void Start()
    {
        _bodyWorldYaw = transform.eulerAngles.y;
        _cameraWorldYaw = _bodyWorldYaw;
    }

    private void Update()
    {
        ProcessYawInput(_inputs.LookInput.x, Time.deltaTime, _inputs.MoveInput.sqrMagnitude);
    }

    private void ProcessYawInput(float inputDelta, float deltaTime, float movementInputMagnitude = 0f)
    {
        _cameraWorldYaw += inputDelta;

        bool isMoving = movementInputMagnitude > _movementInputDeadzone;

        if (!isMoving && movementInputMagnitude > _movementInputDeadzone)
        {
            _isMoving = true;
        }
        float delta = Mathf.DeltaAngle(_bodyWorldYaw, _cameraWorldYaw);

        if (_isMoving)
        {
            _bodyWorldYaw = _cameraWorldYaw;
            _isInterpolating = false;
        }
        else
        {
            if (!_isInterpolating && Mathf.Abs(delta) > _enterThreshold)
            {
                _isInterpolating = true;
            }

            if (_isInterpolating)
            {
                _bodyWorldYaw = Mathf.MoveTowardsAngle(_bodyWorldYaw, _cameraWorldYaw, _rotationSpeed * deltaTime);


                if (Mathf.Abs(Mathf.DeltaAngle(_bodyWorldYaw, _cameraWorldYaw)) < _completionDeadzone)
                {
                    _bodyWorldYaw = _cameraWorldYaw;
                    _isInterpolating = false;
                }
            }
        }

        transform.rotation = Quaternion.Euler(0f, _bodyWorldYaw % 360f, 0f);

        float localYawOffset = _cameraWorldYaw - _bodyWorldYaw;
        _cameraYawPivot.localRotation = Quaternion.Euler(0f, localYawOffset, 0f);

        if (Mathf.Abs(_cameraWorldYaw) > 10000f)
        {
            _cameraWorldYaw %= 360f;
            _bodyWorldYaw %= 360f;
        }
    }

    public float CameraWorldYaw => _cameraWorldYaw;

    public float AngleDelta => Mathf.DeltaAngle(_bodyWorldYaw, _cameraWorldYaw);
}