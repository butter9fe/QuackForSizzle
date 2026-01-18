using UnityEngine;
using PrimeTween;
using QuackForSizzle.Player;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;

public class IntroPairingScreen : MonoBehaviour
{
    [SerializeField] [SerializedDictionary("Player", "Button")] private SerializedDictionary<PlayerNumber, ButtonLoader> connectButtons;
    [SerializeField] private ButtonLoader startButton;

    private Dictionary<PlayerNumber, bool> isConnected = new Dictionary<PlayerNumber, bool>
    {
        { PlayerNumber.Player1, false },
        { PlayerNumber.Player2, false }
    };

    private void OnEnable()
    {
        // Subscribe to events
        BluetoothManager.Instance.OnConnectedEvent.AddListener(OnBluetoothConnected);
        BluetoothManager.Instance.OnConnectionFailedEvent.AddListener(OnBluetoothFailed);

        startButton.gameObject.SetActive(false);
        Time.timeScale = 0f;

        InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.TogglePlayerControls, new QuackForSizzle.Player.EventArgs.ToggleControlsArgs(PlayerNumber.Player1, false));
        InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.TogglePlayerControls, new QuackForSizzle.Player.EventArgs.ToggleControlsArgs(PlayerNumber.Player2, false));
    }

    private void OnDisable()
    {
        // Unsubscribe to events
        BluetoothManager.Instance.OnConnectedEvent.RemoveListener(OnBluetoothConnected);
        BluetoothManager.Instance.OnConnectionFailedEvent.RemoveListener(OnBluetoothFailed);
    }

    private void OnBluetoothConnected(PlayerNumber playerNum)
    {
        connectButtons[playerNum].SetLoading(false);
        connectButtons[playerNum].SetInteractable(false);
        isConnected[playerNum] = true;

        if (isConnected.All(x => x.Value == true))
            startButton.gameObject.SetActive(true);
    }

    private void OnBluetoothFailed(PlayerNumber playerNum)
    {
        connectButtons[playerNum].SetLoading(false);
    }

    public void OnPressConnect(bool isPlayer1)
    {
        var playerNum = isPlayer1 ? PlayerNumber.Player1 : PlayerNumber.Player2;
        connectButtons[playerNum].SetLoading(true);
        BluetoothManager.Instance.ConnectPans(playerNum);
    }

    public void OnPressPlayGame()
    {
        InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.TogglePlayerControls, new QuackForSizzle.Player.EventArgs.ToggleControlsArgs(PlayerNumber.Player1, true));
        InputEventManager.Interface.TriggerEvent(QuackForSizzle.Player.Events.InputEvent.TogglePlayerControls, new QuackForSizzle.Player.EventArgs.ToggleControlsArgs(PlayerNumber.Player2, true));
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
