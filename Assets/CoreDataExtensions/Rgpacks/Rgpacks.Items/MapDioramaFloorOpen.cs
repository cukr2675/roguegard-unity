using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    public class MapDioramaFloorOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private RogueTileInfoData _groundTile = null;
        [SerializeField] private RogueTileInfoData _wallTile = null;

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        public IRaceOption Reopen(RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            var tilemap = new RogueTilemap(new Vector2Int(10, 10));
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    tilemap.Set(_groundTile, x, y);

                    // マップ端は壁
                    if (x == 0 || x == tilemap.Width - 1 || y == 0 || y == tilemap.Height - 1)
                    {
                        tilemap.Set(_wallTile, x, y);
                    }
                }
            }
            self.Space.SetTilemap(tilemap);
            DioramaFloorInfo.SetTo(self, new MapDioramaFloorInfo());
        }
    }
}
