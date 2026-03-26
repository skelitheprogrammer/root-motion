using UnityEngine;
using UnityEngine.InputSystem;
namespace Code
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private InputAction _moveAction;
        [SerializeField] private Transform _camera;
        [SerializeField] private float _maxYaw;
        [SerializeField] private float _maxPitch;

        private Vector2 _mouseInput;
        private float _yaw;
        private float _pitch;

        private void Awake()
        {
            InputSystem.EnableDevice(Mouse.current);
            _moveAction.Enable();
            _moveAction.performed += MoveActionPerformed;
            _moveAction.canceled += MoveActionPerformed;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDestroy()
        {
            _moveAction.performed -= MoveActionPerformed;
            _moveAction.canceled -= MoveActionPerformed;
            _moveAction.Disable();
        }

        private void MoveActionPerformed(InputAction.CallbackContext ctx)
        {
            _mouseInput = ctx.ReadValue<Vector2>();
        }

        private void Update()
        {
            UpdateInput();
        }

        private void LateUpdate()
        {
            _camera.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        private void UpdateInput()
        {
            _pitch -= _mouseInput.y;
            _pitch = Mathf.Clamp(_pitch, -_maxPitch, _maxPitch);

            _yaw += _mouseInput.x;
            _yaw = Mathf.Clamp(_yaw, -_maxYaw, _maxYaw);
        }
    }
}