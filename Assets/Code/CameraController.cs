using UnityEngine;

namespace Code
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraHolder;
        [SerializeField] private float _maxPitch = 80f;
        [SerializeField] private float _mouseSensitivity = 1f;

        private PlayerInputHandler _input;
        private Vector2 _mouseInput;
        private float _pitch;
        private float _yaw;

        public float DesiredYaw => _yaw;

        private void Awake()
        {
            _input = GetComponent<PlayerInputHandler>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            _mouseInput = _input.LookInput * _mouseSensitivity;
            _pitch -= _mouseInput.y;
            _pitch = Mathf.Clamp(_pitch, -_maxPitch, _maxPitch);
            _yaw += _mouseInput.x;
        }

        private void LateUpdate()
        {
            _cameraHolder.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }
}