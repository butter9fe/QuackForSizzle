using UnityEngine;
using CookOrBeCooked.Systems.InteractionSystem;
using PrimeTween;

namespace QuackForSizzle.Interactables
{
    /// <summary>
    /// 
    /// </summary>
    public class Interactable_TrashCan : InteractableObjectBase
    {
        [SerializeField] private CanvasGroup foodIcon;
        private Tween iconFadeTween;

        protected override void OnEnable()
        {
            base.OnEnable();

            OnSelectedEvent += ShowFoodIcon;
            OnUnselectedEvent += HideFoodIcon;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnSelectedEvent -= ShowFoodIcon;
            OnUnselectedEvent -= HideFoodIcon;
        }

        public override void OnActionCancelled(Player.PlayerNumber playerNumber)
        {
            // Do nothing
        }

        public override void OnActionHeld(Player.PlayerNumber playerNumber)
        {
            // Do nothing
        }

        public override void OnActionPerformed(Player.PlayerNumber playerNumber)
        {
            if (!_currSelectedPlayer.HasValue || Player.PlayersManager.Instance.Controllers[_currSelectedPlayer.Value].InventoryHandler.GetInventoryItemOfType<IDiscardableItem>() == null)
                return;

            Player.EventManager.Interface.TriggerEvent(Player.Events.PlayerEvent.DiscardInventory, new Player.EventArgs.PlayerArgsBase(playerNumber));
        }

        private void ShowFoodIcon(InteractableObjectBase obj)
        {
            iconFadeTween.Stop();
            foodIcon.alpha = 1;
        }

        private void HideFoodIcon(InteractableObjectBase obj)
        {
            iconFadeTween.Stop();
            iconFadeTween = Tween.Alpha(foodIcon, 0f, 0.25f);
        }
    }
}
