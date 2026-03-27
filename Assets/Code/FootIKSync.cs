using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Code
{
    public class FootIKSync : MonoBehaviour
    {
        [SerializeField] private TwoBoneIKConstraint _ik;

        [SerializeField] private Animator _animator;
        [SerializeField] private ExtractTransformConstraint _preIKFootCapture;
        
        [SerializeField] private LayerMask _groundMask = -1;

        [SerializeField] private float _raycastOffset = 0.05f;
        [SerializeField] private float _plantedThreshold = 0.03f;
        [SerializeField] private float _smoothTime = 0.02f;

        private Vector3 _storedOffset;
        private bool _wasPlanted;
        private float _currentWeight;
        private float _weightVelocity;

        private void LateUpdate()
        {
            Vector3 rawTipPos = _preIKFootCapture.data.position;
            float footBottomHeight = _animator.leftFeetBottomHeight;

            float legLength = Vector3.Distance(_ik.data.tip.position, _ik.data.root.position);
            float rayDistance = legLength + footBottomHeight + _raycastOffset;
            Vector3 rayOrigin = rawTipPos + Vector3.up * legLength;

            bool hit = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitInfo, rayDistance, _groundMask);
            Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, hit ? Color.green : Color.red);

            if (!hit)
            {
                _ik.weight = 0f;
                return;
            }

            Vector3 desiredGroundPos = hitInfo.point + Vector3.up * footBottomHeight;

            Vector3 footBottom = rawTipPos + Vector3.down * footBottomHeight;
            float distanceToGround = footBottom.y - hitInfo.point.y;
            bool isPlanted = distanceToGround <= _plantedThreshold;

            if (isPlanted)
            {
                _storedOffset = desiredGroundPos - rawTipPos;
            }

            _ik.data.target.position = rawTipPos + _storedOffset;

            float targetWeight = isPlanted
                ? 1f
                : 0f;

            _currentWeight = Mathf.SmoothDamp(_currentWeight, targetWeight, ref _weightVelocity, _smoothTime);
            _ik.weight = _currentWeight;
        }
    }
}