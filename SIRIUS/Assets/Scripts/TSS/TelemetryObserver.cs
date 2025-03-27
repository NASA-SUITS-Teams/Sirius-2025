using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Simple observer of TelemetryClient that displays specified data value from Rover Telemetry
public class TelemetryObserver : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textDisplay;

    [SerializeField]
    private string field;

    // Include field name when displaying text
    [SerializeField]
    private bool includeFieldName = false;

    void Start()
    {
        // Adds itself as an observer
        TSSClient.Instance.AddTelemetryObserver(this);
        textDisplay.text = field;
    }

    // Updates the textDisplay text to updated field value - called by TSSClient
    public void UpdateTelemetryData()
    {
        string fieldValue = TSSClient.Instance.GetTelemetryValue(field);
        if (fieldValue != null)
        {
            string displayText = fieldValue;
            if (includeFieldName)
            {
                displayText = field + " : " + displayText;
            }
            textDisplay.text = displayText;
        }
        else
        {
            textDisplay.text = "Error retreiving field data. Previous data: " + textDisplay.text;
        }
    }

    void OnDestroy()
    {
        // Removes itself as observer when destroyed so it cannot be wrongfully referenced
        TSSClient.Instance.RemoveTelemetryObserver(this);
    }
}

