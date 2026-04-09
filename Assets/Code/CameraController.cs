using UnityEngine;
namespace Code
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform yawTransform;
        [SerializeField] private Transform pitchTransform;
        [SerializeField] private Transform aimTarget;

        [SerializeField] private float _maxPitch;
        [SerializeField] private float aimDistance = 2.5f;
        [SerializeField] private float lookSmoothing = 0.1f;

        private PlayerInputHandler _inputs;

        private Vector2 _smoothedLook;
        private Vector2 _lookVelocity;
        private float _pitchAngle;
        
        private void Awake()
        {
            _inputs = GetComponentInParent<PlayerInputHandler>();
        }
        
        private void Update()
        {
            _smoothedLook = Vector2.SmoothDamp(_smoothedLook, _inputs.LookInput, ref _lookVelocity, lookSmoothing);

            yawTransform.Rotate(Vector3.up * _smoothedLook.x, Space.World);

            _pitchAngle += -_smoothedLook.y;
            _pitchAngle = Mathf.Clamp(_pitchAngle, -_maxPitch, _maxPitch);
            pitchTransform.localEulerAngles = new(_pitchAngle, 0f, 0f);
        }

        private void LateUpdate()
        {
            if (aimTarget is null)
            {
                return;
            }

            aimTarget.position = pitchTransform.position + pitchTransform.forward * aimDistance;
            aimTarget.rotation = pitchTransform.rotation;
        }
    }
}