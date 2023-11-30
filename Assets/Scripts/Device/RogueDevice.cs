using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Roguegard.Device;

namespace Roguegard
{
    public static class RogueDevice
    {
        public static IRogueDevice Primary { get; private set; }

        public static T NewGame<T>(IRogueDeviceSave<T> save)
            where T : IRogueDevice
        {
            Primary?.Close();
            Primary = null;

            var device = save.NewGame();
            Primary = device;
            return device;
        }

        public static T LoadGame<T>(IRogueDeviceSave<T> save, Stream stream)
            where T : IRogueDevice
        {
            Primary?.Close();
            Primary = null;

            var device = save.LoadGame(stream);
            Primary = device;
            return device;
        }

        public static void Add(IKeyword invokeKeyword, int value)
        {
            Primary.AddInt(invokeKeyword, value);
        }

        public static void Add(IKeyword invokeKeyword, float value)
        {
            Primary.AddFloat(invokeKeyword, value);
        }

        public static void AddWork(IKeyword invokeKeyword, in RogueCharacterWork work)
        {
            Primary.AddWork(invokeKeyword, work);
        }

        public static void Add(IKeyword invokeKeyword, object obj)
        {
            Primary.AddObject(invokeKeyword, obj);
        }
    }
}
