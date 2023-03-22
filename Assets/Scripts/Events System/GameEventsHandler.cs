using UnityEngine;

namespace SnakeGame.EventSystem
{
    public class GameEventsHandler : MonoBehaviour
    {
        public GameEventsHandler Current;
        private void Awake()
        {
            Current = this;
        }
        
    }
}
