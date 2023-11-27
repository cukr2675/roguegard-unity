using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyRace : IRogueDescription, IMemberable
    {
        IRaceOption Option { get; }
        string OptionName { get; }
        Color BodyColor { get; }
        string OptionCaption { get; }
        IRogueDetails OptionDetails { get; }
        int Lv { get; }
        IRogueGender Gender { get; }
        string HPName { get; }
        string MPName { get; }
    }
}
