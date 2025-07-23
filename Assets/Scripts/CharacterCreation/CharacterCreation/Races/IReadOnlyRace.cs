using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyRace : IRogueDescribable, IReadOnlyMemberable
    {
        IRaceOption Option { get; }
        string CustomName { get; }
        Color BodyColor { get; }
        string CustomCaption { get; }
        IRogueDetails CustomDetails { get; }
        int Lv { get; }
        IRogueGender Gender { get; }
        string HpName { get; }
        string MpName { get; }
    }
}
