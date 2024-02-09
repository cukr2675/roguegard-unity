using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class NotepadQuote
    {
        public string Text { get; }

        private NotepadQuote() { }

        public NotepadQuote(string text)
        {
            Text = text;
        }
    }
}
