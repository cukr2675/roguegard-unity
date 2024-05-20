using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IDioramaFloor
    {
        void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random);

        void AddAssets(RogueObj dioramaFloor, string rgpackID);
    }
}
