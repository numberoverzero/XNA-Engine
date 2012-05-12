using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Events
{
    public class GameEventArgs
    {
        public string msg { get; protected set; }

        public GameEventArgs() : this("") { }
        public GameEventArgs(string msg)
        {
            this.msg = msg;
        }

    }
}
