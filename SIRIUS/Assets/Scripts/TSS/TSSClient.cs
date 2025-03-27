using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


public class TSSClient : MonoBehaviour
{
    private const string serverIP = "127.0.0.1";
    private const int serverPort = 14141;
    private const int teamNumber = 0;
    private readonly string telemetryUrl = $"http://{serverIP}:{serverPort}/json_data/teams/{teamNumber}/ROVER_TELEMETRY.json";

    // This is really the only blurb of code you need to implement a Unity singleton
    private static TSSClient _Instance;

    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    private Dictionary<string, string> telemetryMap = new Dictionary<string, string>();

    private HashSet<TelemetryObserver> observers = new HashSet<TelemetryObserver>();

    public static TSSClient Instance
    {
        get
        {
            _Instance = FindObjectOfType<TSSClient>();
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<TSSClient>();
                // name it for easy recognition
                _Instance.name = _Instance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    public PrTelemetry PRTelemetry {
        get; private set;
    }

    void Start()
    {
        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        InvokeRepeating(nameof(StartTelemetryUpdate), 0f, 0.1f); // Calls every 1 second
    }

    void StartTelemetryUpdate()
    {
        StartCoroutine(GetTelemetry());
    }

    // Retrieves telemetry data from server as JSON, deserializes it, and updates the map
    IEnumerator GetTelemetry()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(telemetryUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Deserialize JSON into TelemetryData
                RoverTelemetry telemetryData = JsonUtility.FromJson<RoverTelemetry>(request.downloadHandler.text);
                PRTelemetry = telemetryData.pr_telemetry;

                // Convert JSON to Dictionary
                telemetryMap = UpdateTelemetryMap(telemetryData);

                // Alert observers of update so that their data is updated
                foreach (TelemetryObserver telemetryObserver in observers)
                {
                    telemetryObserver.UpdateTelemetryData();
                }
            }
            else
            {
                Debug.LogError("Error loading text: " + request.error);
            }
        }
    }

    // Updates the telemetry map based on the retrieved and deserialized JSON data
    public Dictionary<string, string> UpdateTelemetryMap(RoverTelemetry telemetryData)
    {
        Dictionary<string, string> telemetryMap = new Dictionary<string, string>();

        if (telemetryData?.pr_telemetry == null)
        {
            Debug.LogWarning("Telemetry data is null or invalid.");
            return telemetryMap;
        }

        // Use reflection to dynamically get field names and values
        var fields = typeof(PrTelemetry).GetFields();

        // Sets or updates the field with its value
        foreach (var field in fields)
        {
            object value = field.GetValue(telemetryData.pr_telemetry);
            if (value != null)
            {
                telemetryMap[field.Name] = value.ToString();
            }
        }

        return telemetryMap;
    }

    // Retrieves value from telemetry map - returns null if field not found
    public string GetTelemetryValue(string field)
    {
        return telemetryMap.GetValueOrDefault(field, null);
    }

    // Adds a telemetry observer
    public void AddTelemetryObserver(TelemetryObserver observer)
    {
        if (observer != null)
        {
            observers.Add(observer);
        }
    }

    // Removes a telemetry observer - use when object is destroy
    public bool RemoveTelemetryObserver(TelemetryObserver observer)
    {
        return observers.Remove(observer);
    }

    // Clears all telemetry observers
    public void ClearTelemetryObservers()
    {
        observers.Clear();
    }

    private byte[] ToBytes(uint value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    private byte[] ToBytes(bool value)
    {
        return new byte[] { 0, 0, 0, (byte)(value ? 1 : 0) };
    }

    private byte[] ToBytes(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    private async Task SendCommand(uint commandNumber, byte[] commandData)
    {
        byte[] packet = new byte[12];

        Array.Copy(ToBytes(Environment.TickCount), 0, packet, 0, 4);
        Array.Copy(ToBytes(commandNumber), 0, packet, 4, 4);
        Array.Copy(commandData, 0, packet, 8, 4);

        Console.WriteLine($"Sending command {commandNumber} with data {commandData}");
        await udpClient.SendAsync(packet, packet.Length, remoteEndPoint);
    }

    public async Task SendBrakes(bool brakes)
    {
        await SendCommand(1107, ToBytes(brakes));
    }

    public async Task SendThrottle(float throttle)
    {
        if (Math.Abs(throttle) > 100)
        {
            throw new ArgumentException($"Throttle must be in the range [-100, 100], was {throttle}");
        }

        await SendCommand(1109, ToBytes(throttle));
    }

    public async Task SendSteering(float steering)
    {
        if (Math.Abs(steering) > 1)
        {
            throw new ArgumentException($"Steering must be in the range [-1, 1], was {steering}");
        }

        await SendCommand(1110, ToBytes(steering));
    }
}
