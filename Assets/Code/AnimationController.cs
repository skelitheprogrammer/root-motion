using Code.Movement;
using UnityEngine;
namespace Code
{
    public class AnimationController : MonoBehaviour
    {
        private static readonly int SPEED = Animator.StringToHash("Speed");
        private static readonly int DIRECTION = Animator.StringToHash("Direction");
        private Animator _animator;
        private Locomotion _locomotion;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _locomotion = GetComponent<Locomotion>();
        }

        private void LateUpdate()
        {
            Vector3 localVel = transform.InverseTransformDirection(_locomotion.Velocity.normalized);
            _animator.SetFloat(SPEED, localVel.z);
            _animator.SetFloat(DIRECTION, localVel.x);
        }
    }
}