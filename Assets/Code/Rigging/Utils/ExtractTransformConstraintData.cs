using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace Code
{
    [System.Serializable]
    public struct ExtractTransformConstraintData : IAnimationJobData
    {
        [SyncSceneToStream] public Transform bone;
        public Vector3 position;
        public Quaternion rotation;

        public bool IsValid() => bone is not null;
        public void SetDefaultValues()
        {
            bone = null;
            position = Vector3.zero;
            rotation = Quaternion.identity;
        }
    }
}