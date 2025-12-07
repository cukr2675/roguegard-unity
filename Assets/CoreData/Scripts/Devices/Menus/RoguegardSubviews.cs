using Lysionium;

namespace Roguegard.Device
{
    public static class RoguegardSubviews
    {
        public static IListHandlerSubview GetFace(MMgr manager)
        {
            return (manager as IMMgr)?.Face;
        }

        public static ISummaryElementsSubview GetSummary(MMgr manager)
        {
            return (manager as IMMgr)?.Summary;
        }

        public static ITextEditorElementsSubview GetTextEditor(MMgr manager)
        {
            return (manager as IMMgr)?.TextEditor;
        }

        public static ICharacterCreationElementsSubview GetCharacterCreation(MMgr manager)
        {
            return (manager as IMMgr)?.CharacterCreation;
        }

        public static IPaintElementsSubview GetPaint(MMgr manager)
        {
            return (manager as IMMgr)?.Paint;
        }

        public static IListHandlerSubview GetDopesheet(MMgr manager)
        {
            return (manager as IMMgr)?.Dopesheet;
        }
    }
}
