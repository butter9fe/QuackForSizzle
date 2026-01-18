using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_WSA && !UNITY_EDITOR
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

public class BluetoothHelperV2 : CookOrBeCooked.Utility.Singleton<BluetoothHelperV2>
{
    // Events
    public event Action<string> OnMessageReceived;
    public event Action OnConnectionSucceeded;
    public event Action<string> OnConnectionFailed;
    public event Action OnDisconnected;

    // Status properties
    public bool IsConnected { get; private set; }
    public string ConnectedDeviceName { get; private set; }

    // Private variables
    private bool _isListening = false;
    private byte[] _readBuffer = new byte[1024];

#if UNITY_WSA && !UNITY_EDITOR
    private BluetoothDevice _bluetoothDevice = null;
    private RfcommDeviceService _rfcommService = null;
    private StreamSocket _socket = null;
    private DataWriter _dataWriter = null;
    private DataReader _dataReader = null;
#endif

    private void OnDestroy()
    {
        Disconnect();
    }

    /// <summary>
    /// Initiates the connection to the specified Bluetooth device
    /// </summary>
    public async void Connect()
    {
#if UNITY_WSA && !UNITY_EDITOR
        try
        {
            Debug.Log($"Searching for Bluetooth device: {deviceName}");
        
            string selector = BluetoothDevice.GetDeviceSelectorFromPairingState(true);
            DeviceInformationCollection pairedDevices = await DeviceInformation.FindAllAsync(selector);

            foreach (DeviceInformation device in pairedDevices) 
            {
                Debug.Log($"Paired device: {device.Name}");
            }

            // Connect to specific device by name
            DeviceInformation targetDevice = pairedDevices.FirstOrDefault(d => d.Name == deviceName);
        
            // Connect to the device
            _bluetoothDevice = await BluetoothDevice.FromIdAsync(targetDevice.Id);
            if (_bluetoothDevice == null)
            {
                OnConnectionFailed?.Invoke("Failed to connect to device. Make sure Bluetooth capabilities are enabled.");
                return;
            }
            
            // Get the RFCOMM service for serial communication
            RfcommDeviceServicesResult serviceResult = await _bluetoothDevice.GetRfcommServicesAsync();
            if (serviceResult.Services.Count == 0)
            {
                OnConnectionFailed?.Invoke("No RFCOMM services found on device");
                return;
            }
            
            // Try to find the serial port service
            _rfcommService = serviceResult.Services.FirstOrDefault(s => s.ServiceId.Uuid.ToString() == serviceUuid) ?? 
                serviceResult.Services[0]; // Fallback to first service if specific one not found
            
            // Create socket connection
            _socket = new StreamSocket();
            await _socket.ConnectAsync(
                _rfcommService.ConnectionHostName,
                _rfcommService.ConnectionServiceName,
                SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
            
            // Setup writer and reader
            _dataWriter = new DataWriter(_socket.OutputStream);
            _dataReader = new DataReader(_socket.InputStream);
            _dataReader.InputStreamOptions = InputStreamOptions.Partial;
            
            IsConnected = true;
            ConnectedDeviceName = deviceName;
            
            Debug.Log($"Successfully connected to {deviceName}");
            OnConnectionSucceeded?.Invoke();
            
            // Start listening for messages
            StartListening();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Bluetooth connection error: {ex.Message}");
            OnConnectionFailed?.Invoke(ex.Message);
            Disconnect();
        }
#else
        Debug.LogWarning("Bluetooth functionality is only available in UWP builds.");
        OnConnectionFailed?.Invoke("Bluetooth is only available in UWP builds");
#endif
    }

    /// <summary>
    /// Sends a message to the connected device
    /// </summary>
    public async void SendMessage(string message)
    {
        if (!IsConnected)
        {
            Debug.LogWarning("Cannot send message: Not connected to any device");
            return;
        }

#if UNITY_WSA && !UNITY_EDITOR
        try
        {
            _dataWriter.WriteString(message);
            await _dataWriter.StoreAsync();
            Debug.Log($"Message sent: {message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending message: {ex.Message}");
            Disconnect();
        }
#endif
    }

    /// <summary>
    /// Disconnects from the current Bluetooth device
    /// </summary>
    public void Disconnect()
    {
#if UNITY_WSA && !UNITY_EDITOR
        _isListening = false;
        
        if (_dataReader != null)
        {
            _dataReader.Dispose();
            _dataReader = null;
        }
        
        if (_dataWriter != null)
        {
            _dataWriter.Dispose();
            _dataWriter = null;
        }
        
        if (_socket != null)
        {
            _socket.Dispose();
            _socket = null;
        }
        
        if (_rfcommService != null)
        {
            _rfcommService.Dispose();
            _rfcommService = null;
        }
        
        if (_bluetoothDevice != null)
        {
            _bluetoothDevice = null;
        }
#endif

        if (IsConnected)
        {
            IsConnected = false;
            ConnectedDeviceName = null;
            OnDisconnected?.Invoke();
            Debug.Log("Bluetooth device disconnected");
        }
    }

    /// <summary>
    /// Starts listening for incoming messages
    /// </summary>
    private async void StartListening()
    {
#if UNITY_WSA && !UNITY_EDITOR
        _isListening = true;
        
        try
        {
            while (_isListening)
            {
                uint bytesRead = await _dataReader.LoadAsync(1024);
                if (bytesRead > 0)
                {
                    string receivedChunk = _dataReader.ReadString(bytesRead);
                    Debug.Log($"Received chunk: {receivedChunk}");
                
                    // Append to buffer
                    _messageBuffer.Append(receivedChunk);
                
                    // Check if we have complete messages (ending with newline)
                    string bufferContents = _messageBuffer.ToString();
                    string[] messages = bufferContents.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                
                    // If we have at least one complete message
                    if (bufferContents.Contains('\n') || bufferContents.Contains('\r'))
                    {
                        // Process all but potentially the last message (which might be incomplete)
                        int completeMessages = bufferContents.EndsWith("\n") || bufferContents.EndsWith("\r") ? 
                                              messages.Length : messages.Length - 1;
                    
                        for (int i = 0; i < completeMessages; i++)
                        {
                            string message = messages[i];
                            if (!string.IsNullOrEmpty(message))
                            {
                                string completeMessage = message; // Create a copy for the closure
                                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                                    OnMessageReceived?.Invoke(completeMessage);
                                });
                            }
                        }
                    
                        // Reset buffer with any remaining incomplete message
                        _messageBuffer.Clear();
                        if (completeMessages < messages.Length)
                        {
                            _messageBuffer.Append(messages[messages.Length - 1]);
                        }
                    }
                }
                else
                {
                    // Connection might be closed or no data available
                    await Task.Delay(10);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading data: {ex.Message}");
            
            // Use Unity's main thread to disconnect
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                Disconnect();
            });
        }
#endif
    }
}

// Helper class to run code on Unity's main thread
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private readonly Queue<Action> _executionQueue = new Queue<Action>();
    private readonly object _lock = new object();

    public static UnityMainThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("UnityMainThreadDispatcher");
            _instance = go.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }
        return _instance;
    }

    public void Enqueue(Action action)
    {
        lock (_lock)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        lock (_lock)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
}