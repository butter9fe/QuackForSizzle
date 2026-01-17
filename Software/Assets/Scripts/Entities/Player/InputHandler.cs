using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CookOrBeCooked.Systems.EventSystem;

namespace QuackForSizzle.Player
{
    [DefaultExecutionOrder(-2)] // Always run input handler first
    [RequireComponent(typeof(PlayerInput))]
    public class InputHandler : MonoBehaviour, Controls.IPlayerActions
    {
        private static Controls _controls;

        private bool _isInputEnabled = true;
        public bool IsInputEnabled
        {
            get => _isInputEnabled;
            private set
            {
                _isInputEnabled = value;
                if (_isInputEnabled)
                    _controls.Player.Enable();
                else
                    _controls.Player.Disable();
            }
        }

        private void Awake()
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }

        private void OnEnable()
        {
            _controls.Player.Enable();
            InputEventManager.Interface.ListenToEvent(Events.InputEvent.TogglePlayerControls, Listen_TogglePlayerControls);
        }
        private void OnDisable()
        {
            _controls.Player.Disable();
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.TogglePlayerControls, Listen_TogglePlayerControls);
        }

        #region Event Listeners
        private void Listen_TogglePlayerControls(ArgsBase e)
        {
            BoolArgs args = e as BoolArgs;
            if (args == null)
                return;

            IsInputEnabled = args.value;
        }

        // Default Input System requires overloaded functions w/o context as well, but we won't be doing anything with them
        public void OnMove() { }
        public void OnAction() { }
        public void OnPause() { }

        public void OnAction(InputAction.CallbackContext context)
        {
            if (!_isInputEnabled)
                return;

            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    {
                        InputEventManager.Interface.TriggerEvent(Events.InputEvent.OnActionPerformed, null);
                    }
                    break;

                default:
                    break;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!_isInputEnabled)
                return;

            switch (context.phase)
            {

                case InputActionPhase.Performed:
                    {
                        InputEventManager.Interface.TriggerEvent(Events.InputEvent.OnMovePerformed, new EventArgs.Move(_controls.Player.Move.ReadValue<Vector2>()));
                    }
                    break;

                case InputActionPhase.Canceled:
                    {
                        InputEventManager.Interface.TriggerEvent(Events.InputEvent.OnMoveCancelled, new EventArgs.Move(_controls.Player.Move.ReadValue<Vector2>()));
                    }
                    break;

                default:
                    break;
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (!_isInputEnabled)
                return;
        }
        #endregion Event Listeners

        #region Held Callbacks
        private void Update()
        {
            if (_controls.Player.Move.IsInProgress())
                InputEventManager.Interface.TriggerEvent(Events.InputEvent.OnMoveHeld, new EventArgs.Move(_controls.Player.Move.ReadValue<Vector2>()));
        }
        #endregion
    }
}
