using Lysionium;

namespace Roguegard.Device
{
    public interface IMMgr
    {
        IListHandlerSubview Face { get; }
        ISummaryElementsSubview Summary { get; }
        ITextEditorElementsSubview TextEditor { get; }
        IPaintElementsSubview Paint { get; }
        IListHandlerSubview Dopesheet { get; }
        ICharacterCreationElementsSubview CharacterCreation { get; }
        IListHandlerSubview TitleMenu { get; }
    }
}
