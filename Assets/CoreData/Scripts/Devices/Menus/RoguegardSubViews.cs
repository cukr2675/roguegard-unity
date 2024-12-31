using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public static class RoguegardSubViews
    {
        public static string Face => "Face";
        public static string Summary => "Summary";
        public static string TextEditor => "TextEditor";
        public static string CharacterCreation => "CharacterCreation";
        public static string Paint => "Paint";
        public static string Dopesheet => "Dopesheet";

        public static IElementsSubView GetFace(MMgr manager)
        {
            return manager.GetSubView(Face);
        }

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

        public static IElementsSubView GetDopesheet(MMgr manager)
        {
            return manager.GetSubView(Dopesheet);
        }
    }
}
