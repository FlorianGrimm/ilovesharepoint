using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Drawing;

namespace iLoveSharePoint.Debug
{
    [ServiceBehavior(Namespace = "http://iLoveSharePoint.com/debug/2009/04/07", InstanceContextMode = InstanceContextMode.Single)]
    public class DebugConsole : IDebugConsole
    {
        public void Write(string text)
        {
            Console.Write(text);
        }

        public void Write(string text, ConsoleColor foregroundColor)
        {
            ConsoleColor oldForegroundColor = foregroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(text);

            Console.ForegroundColor = foregroundColor;
        }

        public void Write(string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor oldForegroundColor = foregroundColor;
            ConsoleColor oldBackgroudColor = backgroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            Console.Write(text);

            Console.ForegroundColor = oldForegroundColor;
            Console.BackgroundColor = oldBackgroudColor;
        }

        public string ReadLine()
        {
            return Console.ReadLine();

        }

        public Size GetBufferSize()
        {
            Size size = new Size(Console.BufferWidth,Console.BufferHeight);
            return size;
        }

    }
}
