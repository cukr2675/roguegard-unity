using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class RogueDateTime
    {
        public delegate void Callback(System.DateTime dateTime);

        public static System.DateTime UtcNow()
        {
            System.DateTime dateTime = default;
            RogueDevice.Primary.AddObject(DeviceKw.GetDateTimeUtc, (Callback)(x => dateTime = x));
            return dateTime;
        }
    }
}
