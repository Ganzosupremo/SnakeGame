using Cinemachine;
using UnityEngine;

namespace SnakeGame.Miscelaneous
{
    public class ConfineCamera : MonoBehaviour
    {
        private PolygonCollider2D cameraConfiner;
        public CinemachineConfiner2D confiner2D;

        private void OnEnable()
        {
            StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        }

        private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
        {
            cameraConfiner = roomChangedEventArgs.room.InstantiatedRoom.cameraConfinerCollider;

            if (cameraConfiner != null)
            {
                confiner2D.InvalidateCache();
                confiner2D.m_BoundingShape2D = cameraConfiner;
            }
        }
    }
}
