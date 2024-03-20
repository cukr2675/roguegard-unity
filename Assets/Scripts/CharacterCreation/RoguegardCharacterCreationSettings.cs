using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public static class RoguegardCharacterCreationSettings
    {
        public static ILevelInfoInitializer LevelInfoInitializer { get; set; }

        public static IKeyword EquipPartOfInnerwear { get; set; }

        public static int GetStars(float cost)
        {
            if (cost <= 0) return 0;
            if (cost <= 5) return 1;
            if (cost <= 10) return 2;
            if (cost <= 15) return 3;
            if (cost <= 20) return 4;
            return (int)cost / 10 + 3;
        }
    }
}
