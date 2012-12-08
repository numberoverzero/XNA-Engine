using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.FileHandlers
{
    public class ConsoleLog : Log
    {
        protected override void WriteMsg(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
