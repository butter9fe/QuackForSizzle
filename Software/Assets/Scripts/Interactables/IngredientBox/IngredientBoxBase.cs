using UnityEngine;
using CookOrBeCooked.Systems.InteractionSystem;
using CookOrBeCooked.Utility.ObjectPool;

namespace QuackForSizzle.Interactables
{
    public class IngredientBoxBase : InteractableObjectBase, IObjectPoolUser
    {
        [Foldout("Ingredient Box")]
        [SerializeField] private GameObject ingredientPrefab;

        public GameObject[] ObjectsToPool => new[] { ingredientPrefab }; 

        public int[] PoolQuantitiesRequired => new[] { 5 };

        public Transform PoolParent => transform;

        public bool IsPoolExpandable => true;

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
            if (!_currSelectedPlayer.HasValue || Player.PlayersManager.Instance.Controllers[_currSelectedPlayer.Value].InventoryHandler.HeldItem != null)
                return;

            // Create ingredient
            var ingredient = ObjectPoolManager.Instance.GetGameObjectFromPool(this, ingredientPrefab, transform, true);

            // Add ingredient to inventory
            Player.EventManager.Interface.TriggerEvent(Player.Events.PlayerEvent.SetInventory, new Player.EventArgs.Inventory(playerNumber, ingredient));
        }
    }
}
