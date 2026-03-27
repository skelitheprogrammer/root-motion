using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace Code
{
    public class FootIKSync : MonoBehaviour
    {
        [SerializeField] private TwoBoneIKConstraint _ik;
        [SerializeField] private MultiParentConstraint _parent;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _offset;
        public float weight;

        private void LateUpdate()
        {
            float legLength;
            float distance;
            Vector3 correctedPos;
            RaycastHit info;
            bool hit;

            legLength = Vector3.Distance(_ik.data.tip.position, _ik.data.root.position);
            distance = legLength + _animator.leftFeetBottomHeight + _offset;
            Vector3 direction = Vector3.down;
            correctedPos = _ik.data.tip.position + Vector3.up * legLength;
            hit = Physics.Raycast(correctedPos, direction, out info, distance);
            Debug.DrawRay(correctedPos, direction * distance, Color.magenta);


            if (!hit)
            {
                _ik.data.target.position = _ik.data.tip.position;
                _ik.weight = 0;
                return;
            }

            _ik.data.target.position = info.point + Vector3.up * _animator.leftFeetBottomHeight;
            float actualDistance = Vector3.Distance(_ik.data.tip.position + Vector3.down * _animator.leftFeetBottomHeight, info.point);
            Debug.DrawRay(_ik.data.tip.position, Vector3.down * _animator.leftFeetBottomHeight, Color.green);
            float ratio = 1 - (actualDistance / _animator.leftFeetBottomHeight);
            Debug.Log($"{actualDistance} {ratio}");

            _ik.weight = ratio;

        }
    }
}