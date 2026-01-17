using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookOrBeCooked.Systems.EventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QuackForSizzle.Player
{
    [DefaultExecutionOrder(-2)] // Always run input handler first
    public class InputHandler : MonoBehaviour, Controls.IPlayerActions
    {
        private Controls _controls;
        private Controller _thisPlayer;
        private PlayerInput _playerInput;

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
            _thisPlayer = GetComponentInParent<Controller>();
            _playerInput = GetComponent<PlayerInput>();
            playerMoveArgs = new EventArgs.Move(_thisPlayer.PlayerNumber, Vector2.zero);

            _controls = new Controls();
            _controls.Player.SetCallbacks(this);

            _playerInput.ActivateInput();

        }

        private void OnEnable()
        {
            _controls.Player.Enable();
            isMoveInputHeld[_thisPlayer.PlayerNumber] = false;
            _playerInput.SwitchCurrentControlScheme(_thisPlayer.PlayerNumber == PlayerNumber.Player2 ? _controls.Player2Scheme.name : _controls.Player1Scheme.name, new InputDevice[] { Keyboard.current, Mouse.current });
            InputEventManager.Interface.ListenToEvent(Events.InputEvent.TogglePlayerControls, Listen_TogglePlayerControls);
        }
        private void OnDisable()
        {
            _controls.Player.Disable();
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.TogglePlayerControls, Listen_TogglePlayerControls);
        }

        private void OnDestroy()
        {
            _controls.Player.Disable();
            _controls.Dispose();
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
            if (!_isInputEnabled || !CheckInputSchemeMatches(context))
                return;

            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    {
                        InputEventManager.Interface.TriggerEvent(Events.InputEvent.OnActionPerformed, new EventArgs.InputArgsBase(_thisPlayer.PlayerNumber));
                    }
                    break;

                default:
                    break;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!_isInputEnabled || !CheckInputSchemeMatches(context))
                return;

            lastMoveContext = context;

            switch (context.phase)
            {

                case InputActionPhase.Performed:
                    {
                        // Also start firing events for held
                        isMoveInputHeld[_thisPlayer.PlayerNumber] = true;


                        //Debug.LogError($"Move Performed: {_thisPlayer.PlayerNumber}");
                        InputEventManager.Interface.TriggerEvent(Events.InputEvent.OnMovePerformed, new EventArgs.Move(_thisPlayer.PlayerNumber, _controls.Player.Move.ReadValue<Vector2>()));
                    }
                    break;

                case InputActionPhase.Canceled:
                    {
                        // Stop firing held events
                        isMoveInputHeld[_thisPlayer.PlayerNumber] = false;

                        //Debug.LogError($"Move Cancelled: {_thisPlayer.PlayerNumber}");
                        InputEventManager.Interface.TriggerEvent(Events.InputEvent.OnMoveCancelled, new EventArgs.Move(_thisPlayer.PlayerNumber, _controls.Player.Move.ReadValue<Vector2>()));
                    }
                    break;

                default:
                    break;
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (!_isInputEnabled || !CheckInputSchemeMatches(context))
                return;
        }
        #endregion Event Listeners

        #region Held Callbacks
        /* Movement */
        private InputAction.CallbackContext lastMoveContext;
        private EventArgs.Move playerMoveArgs;
        private Dictionary<PlayerNumber, bool> isMoveInputHeld = new();

        private void Update()
        {
            UpdateMoveEveryFrame();
        }

        private void UpdateMoveEveryFrame()
        {
            if (_isInputEnabled && isMoveInputHeld[_thisPlayer.PlayerNumber])   // if controls are enabled && is currently moving
            {
                if (lastMoveContext.ReadValue<Vector2>().sqrMagnitude > 0)
                {
                    playerMoveArgs.InputVector = lastMoveContext.ReadValue<Vector2>();
                    InputEventManager.Interface.TriggerEvent(Events.InputEvent.OnMoveHeld, playerMoveArgs);
                }
            }
        }
        #endregion

        private bool CheckInputSchemeMatches(InputAction.CallbackContext context)
        {
            var controlSchemes = context.action.GetBindingForControl(context.control).Value.groups.Split(';', StringSplitOptions.RemoveEmptyEntries);

            //Debug.Log($"ControlSchemes: {string.Join(", ", controlSchemes)} | Current Control Scheme: {_playerInput.currentControlScheme} | GO: {gameObject.name}");
            return controlSchemes.Contains(_playerInput.currentControlScheme);
        }
    }
}
