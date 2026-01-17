using UnityEngine;
using CookOrBeCooked.Systems.InteractionSystem;
using CookOrBeCooked.Utility.ObjectPool;
using PrimeTween;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;

namespace QuackForSizzle.Interactables
{
    public class KitchenwareBase : InteractableObjectBase
    {
        [Foldout("Kitchenware")]
        [SerializeField] private CanvasGroup hoverCanvas;
        [SerializeField] private FoodIcon startFoodIcon, endFoodIcon;
        [SerializeField] private GameObject invalidHoverParent, validHoverParent;   
        [SerializeField] [SerializedDictionary("Food", "Cooked Amt")] private SerializedDictionary<FoodType, CookedAmt> _acceptedInputs;

        protected virtual CookedAmt cookedAmtAfterKW => CookedAmt.Cooked;

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
            
            var currHeldFood = !_currSelectedPlayer.HasValue ? null : Player.PlayersManager.Instance.Controllers[_currSelectedPlayer.Value].InventoryHandler.GetInventoryItemOfType<FoodItemBase>();
            bool isValid = currHeldFood != null && _acceptedInputs.ContainsKey(currHeldFood.FoodType) && _acceptedInputs[currHeldFood.FoodType] == currHeldFood.CookedAmt;
            if (currHeldFood == null || !isValid)
            {
                invalidHoverParent.gameObject.SetActive(true);
                validHoverParent.gameObject.SetActive(false);   
            }
            else
            {
                // Set images
                startFoodIcon.SetIcon(currHeldFood.FoodType, currHeldFood.CookedAmt);
                endFoodIcon.SetIcon(currHeldFood.FoodType, cookedAmtAfterKW);

                invalidHoverParent.gameObject.SetActive(false);
                validHoverParent.gameObject.SetActive(true);
            }
            hoverCanvas.alpha = 1;
        }

        private void HideFoodIcon(InteractableObjectBase obj)
        {
            iconFadeTween.Stop();
            iconFadeTween = Tween.Alpha(hoverCanvas, 0f, 0.25f)
                .OnComplete(() =>
                {
                    invalidHoverParent.gameObject.SetActive(true);
                    validHoverParent.gameObject.SetActive(false);
                });
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
            /*if (!_currSelectedPlayer.HasValue || Player.PlayersManager.Instance.Controllers[_currSelectedPlayer.Value].InventoryHandler.HeldItem != null)
                return;

            // Create ingredient
            var ingredient = ObjectPoolManager.Instance.GetGameObjectFromPool(this, ingredientPrefab, transform, true);

            // Add ingredient to inventory
            Player.EventManager.Interface.TriggerEvent(Player.Events.PlayerEvent.SetInventory, new Player.EventArgs.Inventory(playerNumber, ingredient));*/
        }
    }
}
