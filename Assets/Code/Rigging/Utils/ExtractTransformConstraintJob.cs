using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
namespace Code
{
    public struct ExtractTransformConstraintJob : IWeightedAnimationJob
    {
        public ReadWriteTransformHandle bone;
        public FloatProperty jobWeight { get; set; }

        public Vector3Property position;
        public Vector4Property rotation;

        public void ProcessRootMotion(AnimationStream stream) { }

        public void ProcessAnimation(AnimationStream stream)
        {
            AnimationRuntimeUtils.PassThrough(stream, bone);
            
            Vector3 pos = bone.GetPosition(stream);
            Quaternion rot = bone.GetRotation(stream);

            position.Set(stream, pos);
            rotation.Set(stream, new(rot.x, rot.y, rot.z, rot.w));
        }
    }
}