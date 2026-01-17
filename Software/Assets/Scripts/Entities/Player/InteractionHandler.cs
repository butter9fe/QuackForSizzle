using UnityEngine;
using CookOrBeCooked.Systems.InteractionSystem;
using CookOrBeCooked.Systems.EventSystem;

namespace QuackForSizzle.Player
{
    public class InteractionHandler : InteractionHandlerBase
    {
        #region Properties
        protected override Transform _entityTransform => transform;
        #endregion Properties

        #region LifeCycle Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            InputEventManager.Interface.ListenToEvent(Events.InputEvent.OnActionPerformed, Listen_OnActionPerformed);
            InputEventManager.Interface.ListenToEvent(Events.InputEvent.OnActionHeld, Listen_OnActionHeld);
            InputEventManager.Interface.ListenToEvent(Events.InputEvent.OnActionCancelled, Listen_OnActionCancelled);
        }

        private void OnDisable()
        {
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.OnActionPerformed, Listen_OnActionPerformed);
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.OnActionHeld, Listen_OnActionHeld);
            InputEventManager.Interface.StopListeningToEvent(Events.InputEvent.OnActionCancelled, Listen_OnActionCancelled);
        }
        #endregion LifeCycle Methods

        #region Listeners
        private void Listen_OnActionPerformed(ArgsBase e)
        {
			EventArgs.InputArgsBase args = e as EventArgs.InputArgsBase;
			if (args == null || args.Player != _thisPlayer.PlayerNumber)
				return;

			if (_currInteractable == null)
                return;

            _currInteractable.OnActionPerformed(_thisPlayer.PlayerNumber);
        }

        private void Listen_OnActionHeld(ArgsBase e)
        {
			EventArgs.InputArgsBase args = e as EventArgs.InputArgsBase;
			if (args == null || args.Player != _thisPlayer.PlayerNumber)
				return;

			if (_currInteractable == null)
                return;

            _currInteractable.OnActionHeld(_thisPlayer.PlayerNumber);
        }

        private void Listen_OnActionCancelled(ArgsBase e)
        {
			EventArgs.InputArgsBase args = e as EventArgs.InputArgsBase;
			if (args == null || args.Player != _thisPlayer.PlayerNumber)
				return;

			if (_currInteractable == null)
                return;

            _currInteractable.OnActionCancelled(_thisPlayer.PlayerNumber);
        }
        #endregion Listeners

        protected override void UnselectCurrentInteractable()
        {
            if (_currInteractable == null)
                return;

            base.UnselectCurrentInteractable();

            // Release Press and Hold if unselected, regardless of player action
            _currInteractable.OnActionCancelled(_thisPlayer.PlayerNumber);
        }
    }
}
