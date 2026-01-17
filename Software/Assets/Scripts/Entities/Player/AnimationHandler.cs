using UnityEngine;
using CookOrBeCooked.Systems.EventSystem;

namespace QuackForSizzle.Player
{
    /// <summary>
    /// Triggers animation events for player
    /// </summary>
    public class AnimationHandler : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            // Listen to events
            InputEventManager.Interface.ListenToEvent(Events.InputEvent.OnMovePerformed, Listen_StartMove);
            InputEventManager.Interface.ListenToEvent(Events.InputEvent.OnMoveCancelled, Listen_StopMove);
            InputEventManager.Interface.ListenToEvent(Events.InputEvent.OnActionPerformed, Listen_Interact);
        }

        private void OnDisable()
        {
            // Stop listening to events
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.OnMoveHeld, Listen_StartMove);
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.OnMoveCancelled, Listen_StopMove);
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.OnActionPerformed, Listen_Interact);
        }

        private void Listen_StartMove(ArgsBase e)
        {
            EventArgs.Move args = e as EventArgs.Move;
            if (args == null)
                return;

            _animator.SetBool("IsMoving", true);
        }

        private void Listen_StopMove(ArgsBase e)
        {
            _animator.SetBool("IsMoving", false);
        }

        private void Listen_Interact(ArgsBase e)
        {
            _animator.SetTrigger("Interact");
        }
    }
}
