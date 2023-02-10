using System;
using UnityEngine;

public class DestroyEvent : MonoBehaviour
{
    public event Action<DestroyEvent, DestroyedEventArgs> OnDestroy;

    public void CallOnDestroy(bool disableGameobject, long points)
    {
        OnDestroy?.Invoke(this, new DestroyedEventArgs()
        {
            disableGameobject = disableGameobject,
            points = points
        });
    }
}

public class DestroyedEventArgs : EventArgs
{
    public bool disableGameobject;
    public long points;
}
