using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.Debuging
{
    public class DebugCommand : DebugCommandBase
    {
		private Action _command;
        public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
        {
			_command = command;
        }

		public void Invoke()
		{
			_command.Invoke();
		}
    }


    public class DebugCommandBase
    {
		private string _commandId;
        private string _commandFormat;
        private string _commandDescription;

        public string CommandID
		{
			get { return _commandId; }
			set { _commandId = value; }
		}

        public string CommandDescription 
		{ 
			get { return _commandDescription; } 
			set { _commandDescription = value; }
		}

		public string CommandFormat
		{
			get { return _commandFormat; }
			set { _commandFormat = value; }
		}

        public DebugCommandBase(string id, string description, string format)
        {
			_commandId = id;
			_commandDescription = description;
			_commandFormat = format;
        }
    }
}
