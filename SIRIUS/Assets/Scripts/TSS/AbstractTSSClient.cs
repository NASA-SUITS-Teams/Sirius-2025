using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// An abstract class to represent what all TelemetryClients should have to work with TSSManager
public abstract class AbstractTSSClient : TelemetryClient
{
    protected string telemetryPath;
    protected string telemetryURL;
    protected HashSet<TSSObserver> observers;
    protected bool isRunning;
    
    // Creates a TelemetryClient with set values and user set path/url
    public AbstractTSSClient(string path)
    {
        isRunning = false;
        observers = new HashSet<TSSObserver>();
        telemetryPath = path;
        telemetryURL = TSSManager.serverURLBase + path;
    }

    public void AddObserver(TSSObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(TSSObserver observer)
    {
        observers.Remove(observer);
    }

    public void ClearObservers()
    {
        observers.Clear();
    }


    public abstract string GetTelemetryValue(List<string> jsonPath);

    public abstract void StartTelemetryUpdates(float intervalInSeconds = 5f);

    public virtual void StopTelemetryUpdates()
    {
        isRunning = false;
    }

    public string GetTelemetryPath()
    {
        return telemetryPath;
    }
}
