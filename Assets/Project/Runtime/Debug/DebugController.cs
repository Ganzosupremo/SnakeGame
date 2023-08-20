using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.Debuging
{
    public class DebugController : SingletonMonoBehaviour<DebugController>
    {
        public static DebugCommand GIVE_HEALTH;
        public List<object> CommandsList;

        private static bool _showConsole = false;
        private string _input;

        protected override void Awake()
        {
            base.Awake();

            GIVE_HEALTH = new DebugCommand("give_health", "Grants one hit point", "give_health", () =>
            {
                GameManager.Instance.GetSnake().health.IncreaseHealth(1);
            });

            CommandsList = new List<object>
            {
                GIVE_HEALTH
            };
        }

        private void OnGUI()
        {
            if (!_showConsole) return;

            float y = 0f;

            GUI.Box(new Rect(0, y, Screen.width, 100f), "");
            GUI.backgroundColor = Color.black;
            _input = GUI.TextField(new Rect(10f, y + 10f, Screen.width - 20f, 80f), _input);
        }

        public void OnToggleDebug()
        {
            _showConsole = !_showConsole;

            if (_showConsole)
            {
                GameManager.Instance.GetSnake().GetSnakeControler().DisableSnake();
            }
            else
            {
                GameManager.Instance.GetSnake().GetSnakeControler().EnableSnake();
            }
        }

        public void OnReturn()
        {
            if (_showConsole)
            {
                HandleInput();
                _input = "";
            }
        }

        private void HandleInput()
        {
            for (int i = 0; i < CommandsList.Count; i++)
            {
                DebugCommandBase debugCommandBase = CommandsList[i] as DebugCommandBase;

                if (!_input.Contains(debugCommandBase.CommandID)) return;

                if (debugCommandBase == null) return;

                // cast to a command an invoke it
                (CommandsList[i] as DebugCommand).Invoke();
            }
        }
    }
}
