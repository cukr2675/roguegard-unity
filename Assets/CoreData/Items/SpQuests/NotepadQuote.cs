using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class NotepadQuote
    {
        public string Name { get; }

        public string Text { get; }

        private NotepadQuote() { }

        public NotepadQuote(string name, string text)
        {
            Name = name;
            Text = text;
        }
    }
}
