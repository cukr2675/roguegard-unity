using Lysionium;

namespace Roguegard.Device
{
    public interface ICharacterCreationElementsSubview : IListHandlerSubview
    {
        ISelectOption LoadPresetOption { get; }
    }
}
