using Lysionium;

namespace Roguegard.Device
{
    public interface ICharacterCreationElementsSubview : IElementsSubview
    {
        ISelectOption LoadPresetOption { get; }
    }
}
