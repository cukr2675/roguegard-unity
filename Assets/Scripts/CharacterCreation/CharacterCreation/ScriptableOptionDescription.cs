using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/OptionDescription")]
    public class ScriptableOptionDescription : RogueDescriptionData
    {
        [SerializeField] private bool _colorIsEnabled = false;
        public bool ColorIsEnabled { get => _colorIsEnabled; set => _colorIsEnabled = value; }
    }
}
