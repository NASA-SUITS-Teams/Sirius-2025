using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;

// This version uses async/await to retrieve data from TSS server asynchronously
public class CustomTSSAsyncClient : AbstractTSSClient
{
    private JSONNode lastJson; // last retrieved JSON
    private static readonly HttpClient httpClient = new HttpClient(); // Reuse HTTP Client
    
    private CancellationTokenSource cancellationTokenSource; // used to stop HTTP Client

    public CustomTSSAsyncClient(string path) : base(path)
    {   
        lastJson = null;
        cancellationTokenSource = new CancellationTokenSource();
    }

    public override void StartTelemetryUpdates(float intervalInSeconds = 5f)
    {
        isRunning = true;
        _ = RunTelemetryLoop(intervalInSeconds); // fire and forget - result is discarded
    }

    // Grabs JSON data from TSS server every intervalInSeconds
    private async Task RunTelemetryLoop(float intervalInSeconds)
    {
        while (isRunning)
        {
            await UpdateTelemetry();
            await Task.Delay(TimeSpan.FromSeconds(intervalInSeconds));
        }
    }

    // Recieves data from TSS server and updates observers
    public async Task UpdateTelemetry()
    {
        if (observers.Count == 0) return;

        try
        {
            string responseText = await GetTelemetryData();
            if (!string.IsNullOrEmpty(responseText))
            {
                // Debug.Log(responseText);    
                lastJson = JSON.Parse(responseText);
                foreach (var obs in observers)
                {
                    obs.UpdateObserver(this);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Telemetry request failed: {e.Message}");
        }
    }

    // Actual method with HTTPClient to grab JSON from TSS server
    private async Task<string> GetTelemetryData()
    {
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(telemetryURL);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching telemetry data: {e.Message}");
            return null;
        }
    }

    public override string GetTelemetryValue(List<string> jsonPath)
    {
        JSONNode currentNode = lastJson;

        foreach (var node in jsonPath)
        {
            if (currentNode == null) return null;

            // Check if node is an array index
            if (int.TryParse(node, out int index) && currentNode.IsArray)
            {
                currentNode = currentNode[index]; // Access array index
            }
            else
            {
                currentNode = currentNode[node]; // Access object key
            }
        }

        return currentNode != null ? currentNode.Value : null;
    }

    public override void StopTelemetryUpdates()
    {
        base.StopTelemetryUpdates();
        cancellationTokenSource.Cancel();
    }
}
