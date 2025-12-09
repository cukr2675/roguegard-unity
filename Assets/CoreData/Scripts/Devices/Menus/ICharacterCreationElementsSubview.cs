using Lysionium;

namespace Roguegard.Device
{
    public interface ICharacterCreationElementsSubview : IListHandlerSubview
    {
        ISelectOption<MMgr, MArg> LoadPresetOption { get; }
    }
}
