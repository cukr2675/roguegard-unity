using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectFormer
{
    public interface IObjectFormerLogger
    {
        void Log(string text);

        void LogWarning(string text);

        void LogError(string text);
    }
}
