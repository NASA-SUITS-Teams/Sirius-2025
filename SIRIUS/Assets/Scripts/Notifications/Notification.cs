using UnityEngine;

public abstract class Notification
{
    public abstract long Timestamp { get; }

    public abstract string Message { get; }

    public abstract Color Color { get; }

    public abstract Sprite Icon { get; }

}
