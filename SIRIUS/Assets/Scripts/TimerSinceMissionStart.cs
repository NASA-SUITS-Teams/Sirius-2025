using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerSinceMissionStart : MonoBehaviour
{
    
    private float timeSinceStart;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        timeSinceStart = 0;
    }

    void Update()
    {
        timeSinceStart += Time.deltaTime;
        text.text = timeSinceStart.ToString("F2");
    }
}
