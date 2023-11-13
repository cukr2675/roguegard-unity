using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/Settings/WindowFrameList")]
    public class WindowFrameList : ScriptableLoader
    {
        private static WindowFrameList instance;

        [SerializeField] private Item[] _frames = null;

        public static int Count => instance._frames.Length;

        public static string GetName(int index)
        {
            var item = instance._frames[index];
            return item.Name;
        }

        public static void GetWindowFrame(int index, out Sprite backgroundA, out Sprite backgroundB)
        {
            var item = instance._frames[index];
            backgroundA = item.BackgroundA;
            backgroundB = item.BackgroundB;
        }

        public override IEnumerator LoadAsync()
        {
            instance = this;
            yield break;
        }

        public override void TestLoad()
        {
            instance = this;
        }

        [System.Serializable]
        private class Item
        {
            [SerializeField] private string _name;
            public string Name => _name;

            [SerializeField] private Sprite _backgroundA;
            public Sprite BackgroundA => _backgroundA;

            [SerializeField] private Sprite _backgroundB;
            public Sprite BackgroundB => _backgroundB;
        }
    }
}
