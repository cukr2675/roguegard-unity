using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public static class RoguegardCharacterCreationSettings
    {
        public static ILevelInfoInitializer LevelInfoInitializer { get; set; }

        public static IKeyword EquipPartOfInnerwear { get; set; }

        public static IKeyword HairBoneName { get; set; }
    }
}
