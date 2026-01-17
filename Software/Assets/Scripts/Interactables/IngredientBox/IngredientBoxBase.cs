using UnityEngine;
using CookOrBeCooked.Systems.InteractionSystem;
using CookOrBeCooked.Utility.ObjectPool;
using PrimeTween;

namespace QuackForSizzle.Interactables
{
    public class IngredientBoxBase : InteractableObjectBase, IObjectPoolUser
    {
        [Foldout("Ingredient Box")]
        [SerializeField] private GameObject ingredientPrefab;
        [SerializeField] private CanvasGroup foodIcon;

        public GameObject[] ObjectsToPool => new[] { ingredientPrefab }; 

        public int[] PoolQuantitiesRequired => new[] { 5 };

        public Transform PoolParent => transform;

        public bool IsPoolExpandable => true;

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
