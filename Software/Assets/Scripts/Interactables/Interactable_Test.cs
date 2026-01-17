using UnityEngine;
using CookOrBeCooked.Systems.InteractionSystem;

namespace QuackForSizzle.Interactables
{
    /// <summary>
    /// 
    /// </summary>
    public class Interactable_Test : InteractableObjectBase
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
            Debug.Log("Interacted: " + gameObject.name);
        }
    }
}
