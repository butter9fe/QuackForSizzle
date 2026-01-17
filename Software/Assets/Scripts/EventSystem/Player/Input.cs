using System.Collections.Generic;
using UnityEngine;
using CookOrBeCooked.Systems.EventSystem;

namespace QuackForSizzle.Player
{
    namespace Events
    {
        public enum Input
        {
            /// <summary>
            /// Player - Movement Start. 
            /// [Args type: EventArgs.Player.Move]
            /// </summary>
            OnMovePerformed,

            /// <summary>
            /// Player - Movement Update (Fires Continuously). 
            /// [Args type: EventArgs.Player.Move]
            /// </summary>
            OnMoveHeld,

            /// <summary>
            /// Player - Movement Stop. 
            /// [Args type: EventArgs.Player.Move]
            /// </summary>
            OnMoveCancelled,

            /// <summary>
            /// Interaction event when Player has an input to send to an Interactable. 
            /// [Args type: null]
            /// </summary>
            OnActionPerformed,

            /// <summary>
            /// Player - Action Update (Fires Continuously). 
            /// [Args type: null]
            /// </summary>
            OnActionHeld,

            /// <summary>
            /// Player - Action Stop. 
            /// [Args type: null]
            /// </summary>
            OnActionCancelled,

            /// <summary>
            /// Player - Disable/Enable player 3rd-person controls. This also enables/disables Cinemachine Camera! 
            /// [Args type: EventArgs.BoolEventArgs]
            /// </summary>
            TogglePlayerControls = 999,
        }
    }

    namespace EventArgs
    {
        public class InputArgsBase : ArgsBase
        {
            public PlayerNumber Player;
            public InputArgsBase(PlayerNumber player) {
                this.Player = player;
            }
        }

        public class Move : InputArgsBase
        {
            public Vector2 InputVector;

            /// <summary>
            /// Player - Movement Start/Update/Stop.
            /// </summary>
            /// <param name="inputVector">Move vector of input</param>
            public Move(PlayerNumber player, Vector2 inputVector) : base(player)
            {
                this.InputVector = inputVector;
            }
        }
    }

    public enum PlayerNumber
    {
        Player1,
        Player2
    }
}
