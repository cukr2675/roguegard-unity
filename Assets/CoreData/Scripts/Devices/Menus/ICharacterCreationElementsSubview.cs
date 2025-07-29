using Lysionium;

namespace Roguegard.Device
{
    public interface ICharacterCreationElementsSubview : ISubview
    {
        ISelectOption LoadPresetOption { get; }
    }
}
