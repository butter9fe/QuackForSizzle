using System;
using AYellowpaper.SerializedCollections;
using CookOrBeCooked.Systems.EventSystem;
using QuackForSizzle.Interactables;
using QuackForSizzle.Player;
using UnityEngine;
using UnityEngine.UI;

namespace QuackForSizzle.Interactables
{
    public class Kitchenware_Counter : KitchenwareBase
    {
        [Foldout("Counter Configs")]
        [SerializeField]
        [SerializedDictionary("FoodType", "NumScrables")]
        private SerializedDictionary<FoodType, int> _numScrambles = new();

        private Direction? _prevDir = null;
        protected override CookedAmt cookedAmtAfterKW => CookedAmt.Intermediate;

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private float progressPerFlip = 1f;
        protected override void OnEnable()
        {
            base.OnEnable();
            _prevDir = null;

            // Listen to player input events
            InputEventManager.Interface.ListenToEvent(Player.Events.InputEvent.OnMovePerformed, Listen_MovePerformed);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // Listen to player input events
            InputEventManager.Interface.StopListeningToEvent(Player.Events.InputEvent.OnMovePerformed, Listen_MovePerformed);
        }

        public override void OnActionPerformed(PlayerNumber playerNumber)
        {
            base.OnActionPerformed(playerNumber);

            bool isValid = GetIsValidInput(out FoodItemBase currHeldFood);
            if (!isValid)
                return;

            progressPerFlip = 1f / (float)_numScrambles[currHeldFood.FoodType];
            _prevDir = null;
        }

        private void Listen_MovePerformed(ArgsBase e)
        {
            if (!_currCookingPlayer.HasValue || PlayersManager.Instance.Controllers[_currCookingPlayer.Value].PlayerState != PlayerState.Cooking)
                return;

            var currPlayer = PlayersManager.Instance.Controllers[_currCookingPlayer.Value];
            Player.EventArgs.Move args = e as Player.EventArgs.Move;
            if (args == null || args.Player != currPlayer.PlayerNumber)
                return;

            Direction newDir;
            if (args.InputVector.x > 0f)
                newDir = Direction.Right;
            else if (args.InputVector.x < 0f)
                newDir = Direction.Left;
            else if (args.InputVector.y > 0f)
                newDir = Direction.Up;
            else if (args.InputVector.y < 0f)
                newDir = Direction.Down;
            else
                return;

            if (newDir != _prevDir)
            {
                _prevDir = newDir;
                CurrCookingProg += progressPerFlip;
            }
        }
    }
}
