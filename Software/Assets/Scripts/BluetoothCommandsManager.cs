using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuackForSizzle.Player;
using UnityEditor.PackageManager;
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
            MovePerform(playerNum, Vector2.up);
        else if (message == "DOWN")
            MovePerform(playerNum, Vector2.down);
        else if (message == "LEFT")
            MovePerform(playerNum, Vector2.left);
        else if (message == "RIGHT")
            MovePerform(playerNum, Vector2.right);
        else if (message == "NEUTRAL")
        {
            InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnMoveCancelled, new QuackForSizzle.Player.EventArgs.Move(playerNum, Vector2.zero));
            isMoveInputHeld[playerNum] = false;
        }

        if (message == "FLIP")
            InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnActionPerformed, new QuackForSizzle.Player.EventArgs.InputArgsBase(playerNum));
    }

    #region Held Callbacks
    /* Movement */
    private QuackForSizzle.Player.EventArgs.Move playerMoveArgs;
    private Dictionary<PlayerNumber, bool> isMoveInputHeld = new();
    
    private void MovePerform(PlayerNumber playerNum, Vector2 dir)
    {
        playerMoveArgs = new QuackForSizzle.Player.EventArgs.Move(playerNum, dir);
        isMoveInputHeld[playerNum] = true;

        InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnMovePerformed, playerMoveArgs);
    }

    private void Update()
    {
        UpdateMoveEveryFrame();
    }

    private void UpdateMoveEveryFrame()
    {
        if (playerMoveArgs != null && isMoveInputHeld[playerMoveArgs.Player])   // if controls are enabled && is currently moving
        {
            InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.OnMoveHeld, playerMoveArgs);
        }
    }
    #endregion
}
