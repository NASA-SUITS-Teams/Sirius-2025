using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A telemetry observer
public interface TSSObserver
{
    // Update the observer based on the TSSClient
    void UpdateObserver(AbstractTSSClient updatedClient);
}
