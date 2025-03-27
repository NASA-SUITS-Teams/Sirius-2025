using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// A custom observer for TSS telemetry data that updates a TMP_Text component
// This observer allows for retrieval of a data value from a JSON
public class CustomTMObserver : MonoBehaviour, TSSObserver
{
    [SerializeField]
    private TMP_Text textDisplay;

    [SerializeField]
    private string dataPath;

    [SerializeField]
    private List<string> fieldPath;

    // Include field name when displaying text
    [SerializeField]
    private bool includeFieldName = false;

    void Start()
    {
        // Adds itself as an observer
        TSSManager.Instance.AddObserverToClient(dataPath, this);
        textDisplay.text = dataPath;
    }

    // Updates the textDisplay text to updated field value - called by TSSClient
    public void UpdateObserver(AbstractTSSClient updatedClient)
    {
        if (updatedClient != null)
        {
            string displayText = updatedClient.GetTelemetryValue(fieldPath);
            if (includeFieldName)
            {
                displayText = fieldPath[fieldPath.Count - 1] + " : " + displayText;
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
        TSSManager.Instance.RemoveObserverFromClient(dataPath, this);
    }


}

