using System;

namespace Pipeline
{
    public class Logger
    {
        private static Logger _instance;

        public Logger()
        {
            _instance = this;
        }

        public static void LogStatic(string text)
        {
            var i = _instance;
            _instance.Log(text);
        }

        public void Log(string text)
        {
            var time = DateTime.Now.ToShortTimeString();
            var s = $"{time}\t {text}";
            Console.WriteLine(s);
        }

        public void Error(string text)
        {
            Log($"Error: {text}");
        }
    }
}