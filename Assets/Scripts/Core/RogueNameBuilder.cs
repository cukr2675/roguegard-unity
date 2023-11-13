using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;

namespace Roguegard
{
    public class RogueNameBuilder
    {
        private readonly List<string> _texts = new List<string>();
        public Spanning<string> Texts => _texts;

        public int Bonus { get; set; }

        private static readonly StringBuilder builder = new StringBuilder();

        public void Append(string text)
        {
            _texts.Add(text);
        }

        public void Insert0(string text)
        {
            _texts.Insert(0, text);
        }

        public void Set(int index, string text)
        {
            _texts[index] = text;
        }

        public void Clear()
        {
            _texts.Clear();
            Bonus = 0;
        }

        public void CopyFormatTo(StringBuilder stringBuilder)
        {
            stringBuilder.Clear();
            stringBuilder.AppendJoin("", _texts);

            if (Bonus >= 1)
            {
                stringBuilder.Append(" +{0}");
            }
            else if (Bonus <= -1)
            {
                stringBuilder.Append(" {0}");
            }
        }

        public void CopyTo(StringBuilder stringBuilder)
        {
            stringBuilder.Clear();
            stringBuilder.AppendJoin("", _texts);

            if (Bonus >= 1)
            {
                stringBuilder.Append(" +");
                stringBuilder.Append(Bonus);
            }
            else if (Bonus <= -1)
            {
                stringBuilder.Append(" ");
                stringBuilder.Append(Bonus);
            }
        }

        public override string ToString()
        {
            CopyTo(builder);
            return builder.ToString();
        }
    }
}
