using UnityEngine;

namespace Code
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInputs _actions;

        public PlayerInputFrame Current { get; private set; }

        private void Awake() => _actions = new();
        private void OnEnable() => _actions.Enable();
        private void OnDisable() => _actions.Disable();
        private void OnDestroy() => _actions.Dispose();

        private void Update()
        {
            Current = new()
            {
                Move = _actions.Player.Move.ReadValue<Vector2>(),
                Look = _actions.Player.Look.ReadValue<Vector2>(),
                StanceDelta = _actions.Player.StanceChange.ReadValue<float>(),
                CrouchHeld = _actions.Player.Crouch.IsPressed()
            };
        }
    }

    public struct PlayerInputFrame
    {
        public Vector2 Move;
        public Vector2 Look;
        public float StanceDelta;
        public bool CrouchHeld;
    }
}