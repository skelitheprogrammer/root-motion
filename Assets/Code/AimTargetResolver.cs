using UnityEngine;
namespace Code
{
    public class AimTargetResolver : MonoBehaviour
    {
        [SerializeField] private Camera _aimCamera;
        [SerializeField] private Transform _aimTargetTransform;
        
        [SerializeField] private LayerMask _obstacleLayers = -1;
        [SerializeField] private float _maxAimDistance = 50f;
        [SerializeField] private float _wallBuffer = 0.3f;
        [SerializeField] private float _smoothTime = 0.1f;
    
        private Vector3 _currentAimPos;
        private Vector3 _aimVelocity;
    
        public Transform AimTarget => _aimTargetTransform;
        public Vector3 WorldAimPosition => _aimTargetTransform.position;
        public Vector3 AimDirection => _aimTargetTransform.forward;
    
        private void LateUpdate()
        {
            if (_aimCamera is null || _aimTargetTransform is null) return;

            Ray ray = _aimCamera.ViewportPointToRay(Vector3.one * 0.5f);

            Vector3 targetPos = Physics.Raycast(ray, out RaycastHit hit, _maxAimDistance, _obstacleLayers)
                ? hit.point + hit.normal * _wallBuffer
                : ray.origin + ray.direction * _maxAimDistance;

            _currentAimPos = Vector3.SmoothDamp(_currentAimPos, targetPos, ref _aimVelocity, _smoothTime);
            _aimTargetTransform.position = _currentAimPos;
            _aimTargetTransform.rotation = _aimCamera.transform.rotation;
        }
    }
}