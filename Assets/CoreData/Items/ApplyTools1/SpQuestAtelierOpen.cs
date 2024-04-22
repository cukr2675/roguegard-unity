using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class SpQuestAtelierOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private ScriptableStartingItem _monolith = null;
        [SerializeField] private RogueTileInfoData _floorTile = null;
        [SerializeField] private RogueTileInfoData _wallTile = null;

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            if (self.Space.Tilemap == null)
            {
                var tilemap = CreateTilemap();
                self.Space.SetTilemap(tilemap);
                var rooms = new RectInt[] { new RectInt(0, 0, tilemap.Width, tilemap.Height) };
                self.Space.SetRooms(rooms);
                _monolith.Option.CreateObj(_monolith, self, new Vector2Int(1, 1), RogueRandom.Primary);
            }
            return raceOption;
        }

        public void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        public IRaceOption Reopen(RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            NotepadInfo.Ready(self);
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {

        }

        private RogueTilemap CreateTilemap()
        {
            var tilemap = new RogueTilemap(new Vector2Int(16, 12));
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    tilemap.Set(_floorTile, x, y);

                    // マップ端は壁
                    if (x == 0 || x == tilemap.Width - 1 || y == 0 || y == tilemap.Height - 1)
                    {
                        tilemap.Set(_wallTile, x, y);
                    }
                }
            }
            return tilemap;
        }
    }
}
