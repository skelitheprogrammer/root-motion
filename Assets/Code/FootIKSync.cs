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

        [SerializeField] private bool _alignRotation = true;
        [SerializeField] private float _rotationSmoothing = 5f;
        [SerializeField] private float _rotationBlend = 0.8f;
        [SerializeField] private float _maxRotationAngle = 45f;

        [SerializeField] private Vector3 _footUpAxis = Vector3.up;
        [SerializeField] private Vector3 _footForwardAxis = Vector3.forward;

        [Header("Multi‑Ray Settings")]
        [SerializeField] private float _toeOffset = 0.15f;

        [SerializeField] private float _sideOffset = 0.08f;

        private Vector3 _storedPosOffset;
        private Quaternion _storedRotOffset;
        private float _currentWeight;
        private float _weightVelocity;
        private Quaternion _currentRotOffset;

        private void LateUpdate()
        {
            Vector3 rawTipPos = _preIKFootCapture.data.position;
            Quaternion rawTipRot = _preIKFootCapture.data.rotation;
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

            Vector3 desiredPos = hitInfo.point + Vector3.up * footBottomHeight;
            Quaternion desiredRot = rawTipRot;

            if (_alignRotation)
            {
                desiredRot = ComputeFootRotation(rawTipPos, rawTipRot, footBottomHeight);
                desiredRot = Quaternion.Slerp(rawTipRot, desiredRot, _rotationBlend);
            }

            Vector3 footBottom = rawTipPos + Vector3.down * footBottomHeight;
            bool isPlanted = (footBottom.y - hitInfo.point.y) <= _plantedThreshold;

            if (isPlanted)
            {
                _storedPosOffset = desiredPos - rawTipPos;
                Quaternion targetRotOffset = Quaternion.Inverse(rawTipRot) * desiredRot;
                if (_rotationSmoothing > 0f)
                {
                    _currentRotOffset = Quaternion.Slerp(_currentRotOffset, targetRotOffset, Time.deltaTime * _rotationSmoothing);
                }
                else
                {
                    _currentRotOffset = targetRotOffset;
                }

                _storedRotOffset = _currentRotOffset;
            }
            else if (_rotationSmoothing > 0f)
            {
                _currentRotOffset = Quaternion.Slerp(_currentRotOffset, Quaternion.identity, Time.deltaTime * _rotationSmoothing);
                _storedRotOffset = _currentRotOffset;
            }

            _ik.data.target.position = rawTipPos + _storedPosOffset;
            if (_alignRotation)
            {
                _ik.data.target.rotation = rawTipRot * _storedRotOffset;
            }

            float targetWeight = isPlanted ? 1f : 0f;
            _currentWeight = Mathf.SmoothDamp(_currentWeight, targetWeight, ref _weightVelocity, _smoothTime);
            _ik.weight = _currentWeight;
        }

        private Quaternion ComputeFootRotation(Vector3 footPos, Quaternion footRot, float footBottomHeight)
        {
            Vector3 heelWorld = footPos;
            Vector3 toeWorld = footPos + footRot * (_footForwardAxis * _toeOffset);
            Vector3 leftWorld = footPos + footRot * (Vector3.Cross(_footForwardAxis, _footUpAxis).normalized * _sideOffset);
            Vector3 rightWorld = footPos - footRot * (Vector3.Cross(_footForwardAxis, _footUpAxis).normalized * _sideOffset);

            float rayLen = footBottomHeight + 0.2f;
            bool hitHeel = Physics.Raycast(heelWorld + Vector3.up * 0.1f, Vector3.down, out RaycastHit heelHit, rayLen, _groundMask);
            bool hitToe = Physics.Raycast(toeWorld + Vector3.up * 0.1f, Vector3.down, out RaycastHit toeHit, rayLen, _groundMask);
            bool hitLeft = Physics.Raycast(leftWorld + Vector3.up * 0.1f, Vector3.down, out RaycastHit leftHit, rayLen, _groundMask);
            bool hitRight = Physics.Raycast(rightWorld + Vector3.up * 0.1f, Vector3.down, out RaycastHit rightHit, rayLen, _groundMask);

            Vector3[] groundPoints = new Vector3[4];
            int count = 0;
            if (hitHeel)
            {
                groundPoints[count++] = heelHit.point;
            }

            if (hitToe)
            {
                groundPoints[count++] = toeHit.point;
            }

            if (hitLeft)
            {
                groundPoints[count++] = leftHit.point;
            }

            if (hitRight)
            {
                groundPoints[count++] = rightHit.point;
            }

            if (count < 2)
            {
                if (Physics.Raycast(footPos + Vector3.up * 0.1f, Vector3.down, out RaycastHit fallback, rayLen, _groundMask))
                {
                    Vector3 up = footRot * _footUpAxis;
                    Quaternion align = Quaternion.FromToRotation(up, fallback.normal);
                    return align * footRot;
                }

                return footRot;
            }

            Vector3 center = Vector3.zero;
            foreach (Vector3 p in groundPoints) center += p;
            center /= count;

            Vector3 normal = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                Vector3 p1 = groundPoints[i];
                Vector3 p2 = groundPoints[(i + 1) % count];
                Vector3 edge = p2 - p1;
                Vector3 toCenter = center - p1;
                normal += Vector3.Cross(edge, toCenter).normalized;
            }

            normal.Normalize();

            Vector3 originalForward = footRot * _footForwardAxis;
            Vector3 projectedForward = Vector3.ProjectOnPlane(originalForward, normal).normalized;
            if (projectedForward.sqrMagnitude < 0.01f)
            {
                projectedForward = Vector3.Cross(normal, Vector3.right).normalized;
            }

            Quaternion targetRot = Quaternion.LookRotation(projectedForward, normal);

            float angle = Quaternion.Angle(footRot, targetRot);
            if (angle > _maxRotationAngle)
            {
                targetRot = Quaternion.Slerp(footRot, targetRot, _maxRotationAngle / angle);
            }

            return targetRot;
        }

        private void OnDrawGizmos()
        {
            if (!_preIKFootCapture || !_preIKFootCapture.data.bone)
            {
                return;
            }

            Transform bone = _preIKFootCapture.data.bone;
            Vector3 worldUp = bone.TransformDirection(_footUpAxis);
            Vector3 worldForward = bone.TransformDirection(_footForwardAxis);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(bone.position, worldUp);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(bone.position, worldForward);
        }
    }
}