using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/InputReader", order = 1)]
public class InputReader : ScriptableObject, PlayerInputAction.IGameplayActions
{
    private PlayerInputAction _gameInput;

    void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new PlayerInputAction();

            _gameInput.Gameplay.SetCallbacks(this);
        }

        EnableAllInput();
    }

    void OnDisable()
    {
        DisableAllInput();
    }

    private void DisableAllInput()
    {
        _gameInput.Gameplay.Disable();
    }

    public void EnableAllInput()
    {
        _gameInput.Gameplay.Enable();
    }

    public event Action<Vector2> onMove;
    public event Action onJump;

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            this.onJump?.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        this.onMove?.Invoke(context.ReadValue<Vector2>());
    }
}
