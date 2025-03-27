using System;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationObject : MonoBehaviour
{
    const float Size = 1.0f;
    const float HiddenHeight = 100.0f;

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image icon;
    [SerializeField] Image background;

    Notification notif;

    float startTime;

    public void Init(Notification notif)
    {
        this.notif = notif;
    }

    void Start()
    {
        if (notif == null)
        {
            throw new InvalidOperationException("Notification started without being initialized");
        }

        text.text = notif.Message;
        icon.sprite = notif.Icon;
        background.color = notif.Color;

        startTime = Time.time;

        RectTransform transform = GetComponent<RectTransform>();
        transform.anchoredPosition = new Vector2(0, HiddenHeight);
        transform.localScale = Vector3.one * Size;
    }

    void Update()
    {
        RectTransform transform = GetComponent<RectTransform>();
        float elapsedTime = Time.time - startTime;

        if (elapsedTime < 1)
        {
            // Ease down for 1 second
            float y = Mathf.SmoothStep(HiddenHeight, 0, elapsedTime);
            transform.anchoredPosition = new Vector2(0, y);
        }
        else if (elapsedTime < 3)
        {
            // Stop for 2 seconds
            transform.anchoredPosition = new Vector2(0, 0);
        }
        else if (elapsedTime < 4)
        {
            // Ease up for 1 second
            float y = Mathf.SmoothStep(0, HiddenHeight, elapsedTime - 3);
            transform.anchoredPosition = new Vector2(0, y);
        }
        else
        {
            // Delete this NotificationObject
            Destroy(gameObject);
        }
    }
}
