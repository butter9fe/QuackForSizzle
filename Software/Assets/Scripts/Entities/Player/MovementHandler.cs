using CookOrBeCooked.Systems.EventSystem;
using QuackForSizzle.Player.EventArgs;
using UnityEngine;

namespace QuackForSizzle.Player
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class MovementHandler : MonoBehaviour
    {
        #region Properties
        [SerializeField] private Config _playerConfig;

        private Controller _thisPlayer;
        private CharacterController _controller;
        private Vector3 _lateralVelocity = Vector3.zero;
        private Vector3 _verticalVelocity = Vector3.zero;
        #endregion Properties

        #region LifeCycle Methods
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _thisPlayer = GetComponent<Controller>();
        }

        private void OnEnable()
        {
            // Listen to events
            InputEventManager.Interface.ListenToEvent(Events.InputEvent.TogglePlayerControls, Listen_TogglePlayerControls);

            InputEventManager.Interface.ListenToEvent(Events.InputEvent.OnMoveHeld, Listen_UpdateMove);
            InputEventManager.Interface.ListenToEvent(Events.InputEvent.OnMoveCancelled, Listen_StopMove);
        }

        private void OnDisable()
        {
            // Stop listening to events
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.TogglePlayerControls, Listen_TogglePlayerControls);

            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.OnMoveHeld, Listen_UpdateMove);
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.OnMoveCancelled, Listen_StopMove);
        }

        private void Update()
        {
            if (!_controller.enabled)
                return;

            HandleVerticalMovement();

            // Move character
            // NOTE: Recommended to only call this once per tick!
            _controller.Move((_lateralVelocity + _verticalVelocity) * Time.deltaTime);
        }
        #endregion LifeCycle Methods

        #region  Methods
        private void Listen_TogglePlayerControls(ArgsBase e)
        {
            ToggleControlsArgs args = e as ToggleControlsArgs;
            if (args == null || args.Player != _thisPlayer.PlayerNumber)
                return;

            if (_thisPlayer.PlayerState != PlayerState.Normal)
                return;

            _controller.enabled = args.Enabled;
        }

        /// <summary>
        /// Handles lateral movement
        /// </summary>
        private void Listen_UpdateMove(ArgsBase e)
        {
            EventArgs.Move args = e as EventArgs.Move;
            if (args == null || !_controller.enabled || args.Player != _thisPlayer.PlayerNumber)
                return;

            if (_thisPlayer.PlayerState != PlayerState.Normal)
                return;

            // Calculate movement direction based on camera direction
            var cameraForwardXZ = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
            var cameraRightXZ = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;
            var moveVector = cameraRightXZ * args.InputVector.x + cameraForwardXZ * args.InputVector.y;

            // Set player heading
            if (moveVector != Vector3.zero)
                transform.forward = moveVector;

            // Calculate new velocity
            _lateralVelocity = _controller.velocity + (moveVector * _playerConfig.MovementAcceleration * Time.deltaTime);

            // Add drag to player to prevent sliding
            var drag = _lateralVelocity.normalized * _playerConfig.Drag * Time.deltaTime;
            _lateralVelocity = (_lateralVelocity.magnitude > _playerConfig.Drag * Time.deltaTime) ? _lateralVelocity - drag : Vector3.zero; // Prevent drag from going backwards if velocity is too low
            _lateralVelocity = Vector3.ClampMagnitude(_lateralVelocity, _playerConfig.MaxMovementSpeed);
        }

        private void Listen_StopMove(ArgsBase e)
        {
            EventArgs.InputArgsBase args = e as EventArgs.InputArgsBase;
            if (args == null || args.Player != _thisPlayer.PlayerNumber)
                return;

            if (_thisPlayer.PlayerState != PlayerState.Normal)
                return;

            _lateralVelocity = Vector3.zero;
        }

        private void HandleVerticalMovement()
        {
            if (_controller.isGrounded || _verticalVelocity.magnitude < 0)
                _verticalVelocity = Vector3.zero; // Cap to 0 minimum

            _verticalVelocity += Physics.gravity * Time.deltaTime;
        }
        #endregion  Methods
    }
}
