using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class SpQuestAtelierBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private ScriptableStartingItem _monolith = null;
        [SerializeField] private RogueTileInfoData _groundTile = null;
        [SerializeField] private RogueTileInfoData _wallTile = null;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            // �A�g���G�ɋ�Ԃ��ݒ肳��Ă��Ȃ���ΐV�K�쐬
            if (self.Space.Tilemap == null)
            {
                var tilemap = CreateTilemap();
                self.Space.SetTilemap(tilemap);
                var rooms = new RectInt[] { new RectInt(0, 0, tilemap.Width, tilemap.Height) };
                self.Space.SetRooms(rooms);
                _monolith.Option.CreateObj(_monolith, self, new Vector2Int(1, 1), RogueRandom.Primary);
            }

            // �A�g���G�̒��֋�Ԉړ�
            RogueDevice.Add(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
            this.Locate(user, null, self, new Vector2Int(2, 1), activationDepth);
            return false;
        }

        private RogueTilemap CreateTilemap()
        {
            var tilemap = new RogueTilemap(new Vector2Int(16, 12));
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    tilemap.Set(_groundTile, x, y);

                    // �}�b�v�[�͕�
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
