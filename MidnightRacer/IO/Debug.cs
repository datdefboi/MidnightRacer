using System;

namespace MidnightRacer.Engine
{
    static class Debug
    {
        public static string logBuffer = "";
        public static void Write(string metric, string value)
        {
            logBuffer += $"{metric}: {value}\n";
        }

        public static void Write(string metric, float value) => Write(metric, Math.Round(value,2).ToString());
        public static void Write(string metric, double value) => Write(metric, Math.Round(value,2).ToString());
        public static void Write(string metric, int value) => Write(metric, value.ToString());
    }
}
