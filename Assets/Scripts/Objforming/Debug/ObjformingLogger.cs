using System;
using System.Collections.Generic;
using System.Text;

namespace Objforming
{
    public static class ObjformingLogger
    {
        public static IObjformingLogger Primary { get; set; }

        public static void Log(string text)
        {
            Primary?.Log(text);
        }

        public static void LogWarning(string text)
        {
            Primary?.LogWarning(text);
        }

        public static void LogError(string text)
        {
            Primary?.LogError(text);
        }
    }
}
