using UnityEngine;
namespace Code
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInputs _inputActions;

        public Vector2 MoveInput { get; private set; }

        public Vector2 LookInput { get; private set; }
        
        public bool CrouchHeld { get; private set; }

        public float StanceDelta { get; private set; }

        private void Awake()
        {
            _inputActions = new();
            _inputActions.Enable();
        }

        public void Refresh()
        {
            MoveInput = _inputActions.Player.Move.ReadValue<Vector2>();
            LookInput = _inputActions.Player.Look.ReadValue<Vector2>();
            StanceDelta = _inputActions.Player.StanceChange.ReadValue<float>();
            CrouchHeld = _inputActions.Player.Crouch.IsPressed();
        }
    }
}