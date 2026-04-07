using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace Code
{
    public class ExtractTransformConstraintJobBinder : AnimationJobBinder<ExtractTransformConstraintJob, ExtractTransformConstraintData>
    {
        public override ExtractTransformConstraintJob Create(Animator animator, ref ExtractTransformConstraintData data, Component component)
        {
            return new()
            {
                bone = ReadWriteTransformHandle.Bind(animator, data.bone),
                position = Vector3Property.Bind(animator, component, "m_Data." + nameof(data.position)),
                rotation = Vector4Property.Bind(animator, component, "m_Data." + nameof(data.rotation)),
            };
        }

        public override void Destroy(ExtractTransformConstraintJob job) { }
    }
}