using UnityEngine;
using CookOrBeCooked.Systems.InteractionSystem;
using CookOrBeCooked.Utility.ObjectPool;
using PrimeTween;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using QuackForSizzle.Player;
using CookOrBeCooked.Systems.EventSystem;

namespace QuackForSizzle.Interactables
{
    public class KitchenwareBase : InteractableObjectBase
    {
        [Foldout("Kitchenware Icon Reference")]
        [SerializeField] private CanvasGroup hoverCanvas;
        [SerializeField] private FoodIcon startFoodIcon, endFoodIcon;
        [SerializeField] private GameObject invalidHoverParent, validHoverParent;

        [Foldout("Kitchenware Cooking Reference")]
        [SerializeField] private GameObject cookingCanvas;
        [SerializeField] private Slider cookingProgressSlider;

        [Foldout("Kitchenware Finish Reference")]
        [SerializeField] private CanvasGroup finishCanvas;
        [SerializeField] private FoodIcon finishFoodIcon;
        [SerializeField] private TMPro.TMP_Text finalDishText;

        [Foldout("Kitchenware Configs")]
        [SerializeField] [SerializedDictionary("Food", "Cooked Amt")] private SerializedDictionary<FoodType, CookedAmt> _acceptedInputs;
        [SerializeField] [SerializedDictionary("Food", "Title")] private SerializedDictionary<FoodType, string> _cookedTitles;

        // Separate variable as isCooking resets _currSelectedPlayer
        protected PlayerNumber? _currCookingPlayer = null;

        protected virtual CookedAmt cookedAmtAfterKW => CookedAmt.Cooked;

        private Tween iconFadeTween;

        public override bool CanInteract => base.CanInteract && !isCooking;

        protected bool isCooking = false;
        protected FoodItemBase playerHeldFood = null;
        private float currCookingProg = 0f;
        protected float CurrCookingProg { get => currCookingProg; set
            {
                currCookingProg = value;
                cookingProgressSlider.value = currCookingProg;
                if (currCookingProg >= 1f)
                    OnFinishCooking();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            OnSelectedEvent += ShowFoodIcon;
            OnUnselectedEvent += HideFoodIcon;

            _currCookingPlayer = null;
            currCookingProg = 0;
            hoverCanvas.alpha = 0;
            cookingCanvas.gameObject.SetActive(false);

            finishCanvas.alpha = 0f;
            finishCanvas.gameObject.SetActive(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnSelectedEvent -= ShowFoodIcon;
            OnUnselectedEvent -= HideFoodIcon;
            playerHeldFood = null;
            _currCookingPlayer = null;
            currCookingProg = 0;

            finishCanvas.alpha = 0f;
            finishCanvas.gameObject.SetActive(false);
        }

        private void ShowFoodIcon(InteractableObjectBase obj)
        {
            if (isCooking)
                return;

            iconFadeTween.Stop();
            bool isValid = GetIsValidInput(out FoodItemBase currHeldFood);
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
            if (isCooking)
                return;

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
            bool isValid = GetIsValidInput(out FoodItemBase currHeldFood);
            if (!isValid)
                return;

            // Start cooking!
            _currCookingPlayer = _currSelectedPlayer.Value;
            isCooking = true;
            PlayersManager.Instance.Controllers[_currSelectedPlayer.Value].PlayerState = PlayerState.Cooking;
            playerHeldFood = currHeldFood;

            // Show UI
            CurrCookingProg = 0;
            cookingCanvas.SetActive(true);
            iconFadeTween.Stop();
            hoverCanvas.alpha = 1;
        }

        protected void OnFinishCooking()
        {
            // Show Finished UI
            finishFoodIcon.SetIcon(playerHeldFood.FoodType, cookedAmtAfterKW);
            finalDishText.text = _cookedTitles[playerHeldFood.FoodType];
            finishCanvas.alpha = 1f;
            finishCanvas.gameObject.SetActive(true);

            // Update type
            playerHeldFood.CookedAmt = cookedAmtAfterKW;

            // Hide UI
            cookingCanvas.SetActive(false);
            invalidHoverParent.gameObject.SetActive(false);
            validHoverParent.gameObject.SetActive(false);
            hoverCanvas.alpha = 0;

            // Give control back to player
            PlayersManager.Instance.Controllers[_currCookingPlayer.Value].PlayerState = PlayerState.Normal;

            // Start fading out before reenabling counter
            Sequence.Create()
                .ChainDelay(1f)
                .Chain(Tween.Alpha(finishCanvas, 0f, 0.25f))
                .ChainCallback(() =>
                {
                    isCooking = false;
                    playerHeldFood = null;
                    CurrCookingProg = 0;
                    _currCookingPlayer = null;
                });
        }

        protected bool GetIsValidInput(out FoodItemBase playerHeldFood)
        {
            playerHeldFood = !_currSelectedPlayer.HasValue ? null : Player.PlayersManager.Instance.Controllers[_currSelectedPlayer.Value].InventoryHandler.GetInventoryItemOfType<FoodItemBase>();
            return playerHeldFood != null && _acceptedInputs.ContainsKey(playerHeldFood.FoodType) && _acceptedInputs[playerHeldFood.FoodType] == playerHeldFood.CookedAmt;
        }
    }
}
