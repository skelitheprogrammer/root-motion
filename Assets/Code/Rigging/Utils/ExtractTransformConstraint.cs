using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace Code.Rigging.Utils
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Animation Rigging/Extract Transform Constraint")]
    public class ExtractTransformConstraint : RigConstraint<ExtractTransformConstraintJob, ExtractTransformConstraintData, ExtractTransformConstraintJobBinder>
    {
    }
}