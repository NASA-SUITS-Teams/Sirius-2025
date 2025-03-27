using System;
using UnityEngine;

public class InfoNotification : Notification
{
    private readonly string _message;
    private readonly long _timestamp;

    private static readonly Color _color = new(0.4392157f, 0.6313726f, 1.0f, 0.7019608f);
    private static readonly Sprite _icon = Resources.Load<Sprite>("NotificationIcons/Info");

    public InfoNotification(string message)
    {
        _message = message;
        _timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public override long Timestamp
    {
        get
        {
            return _timestamp;
        }
    }

    public override string Message
    {
        get
        {
            return _message;
        }
    }

    public override Color Color
    {
        get
        {
            return _color;
        }
    }

    public override Sprite Icon
    {
        get
        {
            return _icon;
        }
    }
}
