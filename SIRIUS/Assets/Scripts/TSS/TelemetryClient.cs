using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Retrieves telemetry data from TSS server
public interface TelemetryClient
{
    // Adds an observer to the client
    void AddObserver(TSSObserver observer);
    // Removes an observer from the client
    void RemoveObserver(TSSObserver observer);
    // Clears all observers from the client
    void ClearObservers();
    // Parse through JSON to reach relevant value by iterating through jsonPath
    string GetTelemetryValue(List<string> jsonPath);
    // Start receiving data from the TSS server every interval
    void StartTelemetryUpdates(float intervalInSeconds = 5f);
    // Stop receiving data from the TSS server
    void StopTelemetryUpdates();

    // Get the telemetry path the client is currently receiving data from
    string GetTelemetryPath();

}
