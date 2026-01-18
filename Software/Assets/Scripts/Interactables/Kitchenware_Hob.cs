using CookOrBeCooked.Systems.EventSystem;
using UnityEngine.UI;
using QuackForSizzle.Player;
using QuackForSizzle.Interactables;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace QuackForSizzle.Interactables
{
    public class Kitchenware_Hob : KitchenwareBase
    {
        [Foldout("Hob Configs")]
        [SerializeField]
        [SerializedDictionary("FoodType", "NumFlips")]
        private SerializedDictionary<FoodType, int> _numFlipsNeededPerFood = new();

        private float progressPerFlip = 1f;
        protected override void OnEnable()
        {
            base.OnEnable();

            // Listen to player input events
            InputEventManager.Interface.ListenToEvent(Player.Events.InputEvent.OnActionPerformed, Listen_OnActionPerformed);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // Listen to player input events
            InputEventManager.Interface.StopListeningToEvent(Player.Events.InputEvent.OnActionPerformed, Listen_OnActionPerformed);
        }

        public override void OnActionPerformed(PlayerNumber playerNumber)
        {
            base.OnActionPerformed(playerNumber);

            bool isValid = GetIsValidInput(out FoodItemBase currHeldFood);
            if (!isValid)
                return;

            progressPerFlip = 1f / (float)_numFlipsNeededPerFood[currHeldFood.FoodType];
        }

        private void Listen_OnActionPerformed(ArgsBase e)
        {
            if (!_currCookingPlayer.HasValue || PlayersManager.Instance.Controllers[_currCookingPlayer.Value].PlayerState != PlayerState.Cooking)
                return;

            CurrCookingProg += progressPerFlip;
        }
    }
}
