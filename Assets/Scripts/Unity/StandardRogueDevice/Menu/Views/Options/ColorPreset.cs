using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/Settings/ColorPreset")]
    public class ColorPreset : ScriptableLoader
    {
        private static ColorPreset instance;

        [SerializeField] private Item[] _colors = null;

        public static int Count => instance._colors.Length;

        public static string GetName(int index)
        {
            var item = instance._colors[index];
            return item.Name;
        }

        public static Color GetColor(int index)
        {
            var item = instance._colors[index];
            return item.Color;
        }

        public static int IndexOf(Color color)
        {
            for (int i = 0; i < instance._colors.Length; i++)
            {
                if (instance._colors[i].Color == color) return i;
            }
            return -1;
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

            [SerializeField] private Color _color;
            public Color Color => _color;
        }
    }
}
