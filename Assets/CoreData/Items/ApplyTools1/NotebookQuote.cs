using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class NotebookQuote
    {
        public string Text { get; }

        public NotebookQuote(string text)
        {
            Text = text;
        }
    }
}
