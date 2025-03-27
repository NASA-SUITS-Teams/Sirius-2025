using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages all TSSClients. Handles adding and removing observers to clients.
public class TSSManager : MonoBehaviour
{
    private static TSSManager _Instance;

    public static TSSManager Instance
    {
        get
        {
            _Instance = FindObjectOfType<TSSManager>();
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<TSSManager>();
                // name it for easy recognition
                _Instance.name = _Instance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    // Change this to receive data from different server
    public static readonly string serverURLBase = "http://127.0.0.1:14141/json_data/"; 

    // Contains all telemetry clients and what JSON they are receiving from
    private Dictionary<string, TelemetryClient> clients = new Dictionary<string, TelemetryClient>();

    private void Start()
    {
        // Starts all clients
        foreach (var client in clients.Values)
        {
            client.StartTelemetryUpdates(5f);
        }
    }

    // Adds an observer to a client based on the client key / path
    public void AddObserverToClient(string clientKey, TSSObserver observer)
    {
        if (clients.ContainsKey(clientKey))
        {
            TelemetryClient client = clients[clientKey];
            client.AddObserver(observer);
        }
        else
        {
            TelemetryClient newClient = new CustomTSSAsyncClient(clientKey);
            clients.Add(clientKey, newClient);
            newClient.AddObserver(observer);
        }
    }

    // Removes an observer from a client based on the client key / path
    public void RemoveObserverFromClient(string clientKey, TSSObserver observer)
    {
        if (!clients.ContainsKey(clientKey))
        {
            return;
        }

        TelemetryClient client = clients[clientKey];
        client.RemoveObserver(observer);
    }

    private void OnDestroy()
    {
        if (clients != null)
        {
            // Stop all clients from receiving data
            foreach (TelemetryClient client in clients.Values)
            {
                client.StopTelemetryUpdates();
            }
        }

        _Instance = null;
    }
}
