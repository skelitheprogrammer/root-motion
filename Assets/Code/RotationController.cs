using UnityEngine;
namespace Code
{
    public class RotationController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraRig;
        [SerializeField] private float _maxPitch;

        private PlayerInputHandler _input;
        private float _yaw;
        private float _pitch;

        private void Awake()
        {
            _input = GetComponent<PlayerInputHandler>();
        }

        private void Start()
        {
            _yaw = transform.eulerAngles.y;
            _pitch = _cameraRig.localEulerAngles.x;

            if (_pitch > 180f) _pitch -= 360f;
        }

        private void Update()
        {
            _yaw += _input.LookInput.x;
            _yaw %= 360;
            _pitch += _input.LookInput.y;
            _pitch = Mathf.Clamp(_pitch, -_maxPitch, _maxPitch);

            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            _cameraRig.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

    }
}