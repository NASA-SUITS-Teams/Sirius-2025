using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// TSSObserver that is capable of receiving data values from the same or different clients
/* To use:
 *  1. Specify placeholder values and what data values needs to be received 
 *      - (placeholder amount MUST equal data value amount)
 *  2. Write out what the text should look like in Text Format inspector box
 *      - defined placeholders will be replaced with mapped data values
 * 
 *  For an example, look at Example MultiTSS prefab
 * 
 */
public class MultiTSSObserver : MonoBehaviour, TSSObserver
{
    [SerializeField]
    private TMP_Text textDisplay;

    [SerializeField]
    private string textFormat; // what text is shown

    [SerializeField]
    private List<string> placeholders = new List<string>(); // what placeholders are in place

    [SerializeField]
    private List<TMPath> paths = new List<TMPath>(); // paths to data values

    // Maps placeholders to paths
    private Dictionary<string, TMPath> placeholderMapper = new Dictionary<string, TMPath>(); 
    // Maps paths to retreived data values
    private Dictionary<TMPath, string> retrievedValues = new Dictionary<TMPath, string>();
    // Maps telemetry clients to connected paths
    private Dictionary<string, HashSet<TMPath>> clientAndJSONs = new Dictionary<string, HashSet<TMPath>>();

    void Start()
    {
        // Fills out dictionaries based on placeholders and paths
        if (paths.Count == placeholders.Count)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                string placeholder = placeholders[i];
                TMPath path = paths[i];;

                placeholderMapper.Add(placeholder, path);
                retrievedValues.Add(path, "DATA NOT RETRIEVED");

                if (clientAndJSONs.ContainsKey(path.GetJSONFilePath()))
                {
                    HashSet<TMPath> jsons = clientAndJSONs[path.GetJSONFilePath()];
                    jsons.Add(path);
                }
                else
                {
                    clientAndJSONs.Add(path.GetJSONFilePath(), new HashSet<TMPath>());
                    HashSet<TMPath> jsons = clientAndJSONs[path.GetJSONFilePath()];
                    jsons.Add(path);
                }
            }

            // Adds itself as an observer
            foreach (TMPath path in paths)
            {
                TSSManager.Instance.AddObserverToClient(path.GetJSONFilePath(), this);
            }
        }
        else
        {
            // Do nothing if placeholders dont match paths
            placeholders.Clear();
            paths.Clear();
        }
    }

    // Updates the textDisplay text to updated field value
    public void UpdateObserver(AbstractTSSClient updatedClient)
    {
        if (updatedClient != null)
        {
            string updated = textFormat;
            // Updates corresponding retreived values based on observed telemetry client
            if (clientAndJSONs.ContainsKey(updatedClient.GetTelemetryPath()))
            {
                HashSet<TMPath> tmpaths = clientAndJSONs[updatedClient.GetTelemetryPath()];
                foreach (TMPath tm in tmpaths) {
                    string updatedRetrieved = updatedClient.GetTelemetryValue(tm.GetJSONKeyPath());
                    retrievedValues[tm] = updatedRetrieved;
                }
            }

            // Replaces placeholders here
            foreach (KeyValuePair<string, TMPath> pair in placeholderMapper)
            {
                string replacedValue = retrievedValues[pair.Value];
                if (replacedValue != null)
                {
                    updated = updated.Replace(pair.Key, replacedValue);
                }
            }

            textDisplay.text = updated;
        }
        else
        {
            textDisplay.text = "Error retreiving field data. Previous data: " + textDisplay.text;
        }
    }

    void OnDestroy()
    {
        // Removes itself as observer when destroyed so it cannot be wrongfully referenced
        foreach (TMPath path in paths)
        {
            TSSManager.Instance.RemoveObserverFromClient(path.GetJSONFilePath(), this);
        }
    }


}

