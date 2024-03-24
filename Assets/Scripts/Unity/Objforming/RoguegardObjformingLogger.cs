using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    public class RoguegardObjformingLogger : Objforming.IObjformingLogger
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
