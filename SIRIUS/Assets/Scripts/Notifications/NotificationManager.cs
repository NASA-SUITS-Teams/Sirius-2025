using System;
using System.Collections.Generic;
using UnityEngine;



public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField] NotificationObject notificationPrefab;
    [SerializeField] RectTransform notificationAnchor;

    private readonly List<Notification> notifications = new();

    void Start()
    {
        if (Instance != null)
        {
            throw new InvalidOperationException("NotificationManager started when one already exists");
        }

        Instance = this;
    }

    public void SendNotification(Notification notif)
    {
        NotificationObject notifObj = Instantiate(notificationPrefab, notificationAnchor.transform);
        notifObj.Init(notif);

        notifications.Add(notif);
    }
}
