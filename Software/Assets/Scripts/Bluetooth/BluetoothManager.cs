using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Unity.VisualScripting;
using ArduinoBluetoothAPI;
using QuackForSizzle.Player;

public class BluetoothManager : CookOrBeCooked.Utility.Singleton<BluetoothManager>
{
    const string deviceName1 = "Pan_1";
    const string deviceName2 = "Pan_2";
    string received_message;
    BluetoothHelper bluetoothHelper1;
    BluetoothHelper bluetoothHelper2;

    public UnityEvent<PlayerNumber> OnConnectedEvent { get; private set; } = new();
    public UnityEvent<PlayerNumber> OnConnectionFailedEvent { get; private set; } = new();
    public UnityEvent<PlayerNumber, string> OnMessageReceivedEvent { get; private set; } = new();

    public bool IsConnected => BluetoothHelperV2.Instance?.IsConnected ?? false;

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WSA
        BluetoothHelperV2.Instance.OnConnectionSucceeded += OnBluetoothConnected;
        BluetoothHelperV2.Instance.OnConnectionFailed += OnBluetoothConnectionFailed;
        BluetoothHelperV2.Instance.OnDisconnected += OnBluetoothDisconnected;
        BluetoothHelperV2.Instance.OnMessageReceived += OnBluetoothMessageReceived;
#else
        BluetoothHelper.BLE = false;

        try
        {
            bluetoothHelper1 = BluetoothHelper.GetInstance(deviceName1);
            bluetoothHelper1.OnConnected += OnConnectedP1;
            bluetoothHelper1.OnConnectionFailed += OnConnectionFailedP1;
            bluetoothHelper1.OnDataReceived += OnMessageReceivedP1; //read the data

            //bluetoothHelper.setFixedLengthBasedStream(3); //receiving every 3 characters together
            bluetoothHelper1.setTerminatorBasedStream("\n"); //delimits received messages based on \n char
            //if we received "Hi\nHow are you?"
            //then they are 2 messages : "Hi" and "How are you?"

            bluetoothHelper2 = BluetoothHelper.GetInstance(deviceName2);
            bluetoothHelper2.OnConnected += OnConnectedP2;
            bluetoothHelper2.OnConnectionFailed += OnConnectionFailedP2;
            bluetoothHelper2.OnDataReceived += OnMessageReceivedP2; //read the data

            //bluetoothHelper.setFixedLengthBasedStream(3); //receiving every 3 characters together
            bluetoothHelper2.setTerminatorBasedStream("\n"); //delimits received messages based on \n char
            //if we received "Hi\nHow are you?"
            //then they are 2 messages : "Hi" and "How are you?"
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
#endif
    }

    public void ConnectPans(PlayerNumber playerNum)
    {
#if !UNITY_EDITOR && UNITY_WSA
        BluetoothHelperV2.Instance.SetDeviceName(deviceName);
        BluetoothHelperV2.Instance.Connect();
#else
        var helper = playerNum == PlayerNumber.Player1 ? bluetoothHelper1 : bluetoothHelper2;
        var devName = playerNum == PlayerNumber.Player1 ? deviceName1 : deviceName2;
        LinkedList<BluetoothDevice> ds = helper.getPairedDevicesList();

        foreach (BluetoothDevice d in ds)
        {
            // Note: Might need a delay, doesn't seem to connect on built
            Debug.Log($"{d.DeviceName} {d.DeviceAddress}");
            if (d.DeviceName == devName)
            {
                Debug.Log($"Paired! {devName}");
                helper.Connect();
                break;
            }
        }
#endif
    }

    public void SendBTMessage(string message)
    {
#if !UNITY_EDITOR && UNITY_WSA
        if (!BluetoothHelperV2.Instance.IsConnected)
            return;
            
        BluetoothHelperV2.Instance.SendMessage(message);
#else
        if (!IsConnected)
            return;

        bluetoothHelper1.SendData(message);
        bluetoothHelper2.SendData(message);
#endif
    }


    #region Editor Bluetooth Callbacks
    void OnMessageReceivedP1(BluetoothHelper helper)
    {
        OnMessageReceived(helper, PlayerNumber.Player1);
    }

    void OnMessageReceivedP2(BluetoothHelper helper)
    {
        OnMessageReceived(helper, PlayerNumber.Player2);
    }

    void OnMessageReceived(BluetoothHelper helper, PlayerNumber playerNum)
    {
        received_message = helper.Read();
        received_message = received_message.Replace("\r", "").Replace("\n", "");
        Debug.Log(received_message);

        OnMessageReceivedEvent?.Invoke(playerNum, received_message);
    }

    void OnConnectedP1(BluetoothHelper helper)
    {
        OnConnected(helper, PlayerNumber.Player1);
    }

    void OnConnectedP2(BluetoothHelper helper)
    {
        OnConnected(helper, PlayerNumber.Player2);
    }

    void OnConnected(BluetoothHelper helper, PlayerNumber playerNum)
    {
        Debug.Log("Connected");
        OnConnectedEvent?.Invoke(playerNum);

        try
        {
            Debug.Log("Starting listener!");
            helper.StartListening();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

    }

    void OnConnectionFailedP1(BluetoothHelper helper)
    {
        OnConnectionFailed(helper, PlayerNumber.Player1);
    }

    void OnConnectionFailedP2(BluetoothHelper helper)
    {
        OnConnectionFailed(helper, PlayerNumber.Player2);
    }

    void OnConnectionFailed(BluetoothHelper helper, PlayerNumber playerNum)
    {
        Debug.Log("Connection Failed");
        OnConnectionFailedEvent?.Invoke(playerNum);
    }
    #endregion
    protected override void OnDestroy()
    {
        if (bluetoothHelper1 != null)
            bluetoothHelper1.Disconnect();

        if (bluetoothHelper2 != null)
            bluetoothHelper2.Disconnect();
        base.OnDestroy();
    }
}
