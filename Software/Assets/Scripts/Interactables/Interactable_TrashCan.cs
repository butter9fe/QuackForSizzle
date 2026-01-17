using UnityEngine;
using CookOrBeCooked.Systems.InteractionSystem;

namespace QuackForSizzle.Interactables
{
    /// <summary>
    /// 
    /// </summary>
    public class Interactable_TrashCan : InteractableObjectBase
    {
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
    }
}
