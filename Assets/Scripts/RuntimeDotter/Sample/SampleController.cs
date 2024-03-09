using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeDotter.Sample
{
    public class SampleController : MonoBehaviour
    {
        [SerializeField] private DotterToolSet _toolSet = null;

        private void Start()
        {
            var size = Vector2Int.one * 32;
            //var palette = new Color32[16]
            //{
            //    Color.clear,
            //    Color.Lerp(Color.red, Color.white, .5f),
            //    Color.red,
            //    Color.Lerp(Color.red, Color.black, .5f),
            //    Color.Lerp(Color.blue, Color.white, .5f),
            //    Color.blue,
            //    Color.Lerp(Color.blue, Color.black, .5f),
            //    Color.Lerp(Color.yellow, Color.white, .5f),
            //    Color.yellow,
            //    Color.Lerp(Color.yellow, Color.black, .5f),
            //    Color.Lerp(Color.green, Color.white, .5f),
            //    Color.green,
            //    Color.Lerp(Color.green, Color.black, .5f),
            //    Color.white,
            //    Color.gray,
            //    Color.black
            //};
            var palette = new ShiftableColor[16]
            {
                new ShiftableColor(Color.clear, false),
                new ShiftableColor(Color.Lerp(Color.red, Color.white, .5f), false),
                new ShiftableColor(Color.red, false),
                new ShiftableColor(Color.Lerp(Color.red, Color.black, .5f), false),
                new ShiftableColor(Color.Lerp(Color.blue, Color.white, .5f), false),
                new ShiftableColor(Color.blue, false),
                new ShiftableColor(Color.Lerp(Color.blue, Color.black, .5f), false),
                new ShiftableColor(Color.Lerp(Color.yellow, Color.white, .5f), false),
                new ShiftableColor(Color.yellow, false),
                new ShiftableColor(Color.Lerp(Color.yellow, Color.black, .5f), false),
                new ShiftableColor(Color.Lerp(Color.green, Color.white, .5f), false),
                new ShiftableColor(Color.green, false),
                new ShiftableColor(Color.Lerp(Color.green, Color.black, .5f), false),
                new ShiftableColor(Color.white, false),
                new ShiftableColor(Color.gray, false),
                new ShiftableColor(Color.black, false)
            };
            var board = new DotterBoard(size, palette.Length);
            _toolSet.Load(board, palette, Color.grey);
        }
    }
}
