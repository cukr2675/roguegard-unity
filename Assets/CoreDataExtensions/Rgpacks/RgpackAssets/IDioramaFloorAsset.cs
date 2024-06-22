using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public interface IDioramaFloorAsset
    {
        void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random);
    }
}
