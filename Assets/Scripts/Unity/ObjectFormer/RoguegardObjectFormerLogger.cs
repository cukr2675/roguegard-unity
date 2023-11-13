using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    public class RoguegardObjectFormerLogger : ObjectFormer.IObjectFormerLogger
    {
        public void Log(string text)
        {
            Debug.Log(text);
        }

        public void LogError(string text)
        {
            Debug.LogError(text);
        }

        public void LogWarning(string text)
        {
            Debug.LogWarning(text);
        }
    }
}
