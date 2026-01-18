using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuackForSizzle.Player;
using UnityEngine;
public class BluetoothCommandsManager : CookOrBeCooked.Utility.Singleton<BluetoothCommandsManager>
{
    private void Start()
    {
        BluetoothManager.Instance.OnMessageReceivedEvent.AddListener(OnMessageReceived);
    }

    private void OnMessageReceived(PlayerNumber playerNum, string message)
    {
        if (message == "UP")
            InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnMovePerformed, new QuackForSizzle.Player.EventArgs.Move(playerNum, Vector2.up));
        else if (message == "DOWN")
            InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnMovePerformed, new QuackForSizzle.Player.EventArgs.Move(playerNum, Vector2.down));
        else if (message == "LEFT")
            InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnMovePerformed, new QuackForSizzle.Player.EventArgs.Move(playerNum, Vector2.left));
        else if (message == "RIGHT")
            InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnMovePerformed, new QuackForSizzle.Player.EventArgs.Move(playerNum, Vector2.right));
        else if (message == "NEUTRAL")
            InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnMoveCancelled, new QuackForSizzle.Player.EventArgs.Move(playerNum, Vector2.zero));

        if (message == "FLIP")
            InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnActionPerformed, new QuackForSizzle.Player.EventArgs.InputArgsBase(playerNum));
    }
}
