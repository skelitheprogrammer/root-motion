using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputs _inputActions;
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressedThisFrame { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool CrouchHeld { get; private set; }

    private void Awake()
    {
        _inputActions = new PlayerInputs();
        _inputActions.Enable();
    }

    public void Refresh()
    {
        MoveInput = _inputActions.Player.Move.ReadValue<Vector2>();
        LookInput = _inputActions.Player.Look.ReadValue<Vector2>();
        JumpPressedThisFrame = _inputActions.Player.Jump.WasPressedThisFrame();
        SprintHeld = _inputActions.Player.Sprint.IsPressed();
        CrouchHeld = _inputActions.Player.Crouch.IsPressed();
    }

    public void ConsumeJump()
    {
        JumpPressedThisFrame = false;
    }
}