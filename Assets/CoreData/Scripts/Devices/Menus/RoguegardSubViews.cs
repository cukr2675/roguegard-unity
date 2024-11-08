using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public static class RoguegardSubViews
    {
        public static string Summary => "Summary";
        public static string TextEditor => "TextEditor";
        public static string CharacterCreation => "CharacterCreation";
        public static string Paint => "Paint";

        public static ISummaryElementsSubView GetSummary(MMgr manager)
        {
            return (ISummaryElementsSubView)manager.GetSubView(Summary);
        }

        public static ITextEditorElementsSubView GetTextEditor(MMgr manager)
        {
            return (ITextEditorElementsSubView)manager.GetSubView(TextEditor);
        }

        public static ICharacterCreationElementsSubView GetCharacterCreation(MMgr manager)
        {
            return (ICharacterCreationElementsSubView)manager.GetSubView(CharacterCreation);
        }

        public static IPaintElementsSubView GetPaint(MMgr manager)
        {
            return (IPaintElementsSubView)manager.GetSubView(Paint);
        }
    }
}
