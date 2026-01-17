using UnityEngine;
using CookOrBeCooked.Systems.EventSystem;

namespace QuackForSizzle.Player
{
    namespace Events
    {
        public enum PlayerEvent
        {
            /// <summary>
            /// Player Inventory - Set inventory to a new item
            /// [Args type: EventArgs.Inventory]
            /// </summary>
            SetInventory,

            /// <summary>
            /// Player Inventory - Throw away held item in inventory
            /// [Args type: null]
            /// </summary>
            DiscardInventory
        }
    }

    namespace EventArgs
    {
        public class PlayerArgsBase : ArgsBase
        {
            public PlayerNumber PlayerNumber;
            public PlayerArgsBase(PlayerNumber playerNumber) : base()
            {
                this.PlayerNumber = playerNumber;
            }
        }
        public class Inventory : PlayerArgsBase
        {
            public GameObject Item;

            /// <summary>
            /// Player - Set inventory to a new item
            /// </summary>
            /// <param name="item">item to hold</param>
            public Inventory(PlayerNumber playerNumber, GameObject item) : base(playerNumber)
            {
                this.Item = item;
            }
        }
    }
}