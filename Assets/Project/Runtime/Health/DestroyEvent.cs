using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace SnakeGame.HealthSystem
{
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

}