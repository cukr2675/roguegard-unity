using Lysionium;

namespace Roguegard.Device
{
    public static class RoguegardSubviews
    {
        public static string Face => "Face";
        public static string Summary => "Summary";
        public static string TextEditor => "TextEditor";
        public static string CharacterCreation => "CharacterCreation";
        public static string Paint => "Paint";
        public static string Dopesheet => "Dopesheet";

        public static IElementsSubview GetFace(MMgr manager)
        {
            return manager.GetSubview(Face);
        }

        public static ISummaryElementsSubview GetSummary(MMgr manager)
        {
            return (ISummaryElementsSubview)manager.GetSubview(Summary);
        }

        public static ITextEditorElementsSubview GetTextEditor(MMgr manager)
        {
            return (ITextEditorElementsSubview)manager.GetSubview(TextEditor);
        }

        public static ICharacterCreationElementsSubview GetCharacterCreation(MMgr manager)
        {
            return (ICharacterCreationElementsSubview)manager.GetSubview(CharacterCreation);
        }

        public static IPaintElementsSubview GetPaint(MMgr manager)
        {
            return (IPaintElementsSubview)manager.GetSubview(Paint);
        }

        public static IElementsSubview GetDopesheet(MMgr manager)
        {
            return manager.GetSubview(Dopesheet);
        }
    }
}
