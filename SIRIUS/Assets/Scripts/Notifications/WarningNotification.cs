using System;
using UnityEngine;

public class WarningNotification : Notification
{
    private readonly long _timestamp;
    private readonly string _message;

    private static readonly Color _color = new(1.0f, 0.529273f, 0.2714199f, 0.7019608f);
    private static readonly Sprite _icon = Resources.Load<Sprite>("NotificationIcons/Warning");

    public WarningNotification(string message)
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
