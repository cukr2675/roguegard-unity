using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/Dungeon/Levels/Standard")]
    public class StandardDungeonLevel : RogueDungeonLevel
    {
        [SerializeField] private RogueDungeonGenerator _dungeonGenerator = null;
        public override Spanning<IRogueTile> FillTiles => _dungeonGenerator.FillTiles;
        public override Spanning<IRogueTile> NoizeTiles => _dungeonGenerator.NoiseTiles;
        public override Spanning<IRogueTile> RoomFloorTiles => _dungeonGenerator.RoomFloorTiles;
        public override Spanning<IRogueTile> RoomWallTiles => _dungeonGenerator.RoomWallTiles;

        [SerializeField] private RandomRoomObjTable[] _enemyTable = null;
        public override Spanning<IWeightedRogueObjGeneratorList> EnemyTable => _enemyTable;

        [SerializeField] private RandomRoomObjTable[] _itemTable = null;
        public override Spanning<IWeightedRogueObjGeneratorList> ItemTable => _itemTable;

        [SerializeField] private RandomRoomObjTable[] _otherTable = null;
        public override Spanning<IWeightedRogueObjGeneratorList> OtherTable => _otherTable;

        [SerializeField] private float _monsterHouseRate = .2f;

        public override void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random)
        {
            var dungeon = player.Location;
            var lv = dungeon.Main.Stats.Lv;

            // ダンジョン地形生成
            {
                floor.Main.Stats.SetLv(floor, lv);
                _dungeonGenerator.Generate(floor.Space, random);
                
                var effect = new Effect();
                effect.lv = lv;
                floor.Main.RogueEffects.AddOpen(floor, effect);

                var monsterHouse = random.NextFloat(0f, 1f);
                if (monsterHouse <= _monsterHouseRate)
                {
                    var roomIndex = random.Next(0, floor.Space.RoomCount);
                    MonsterHouseEffect.SetTo(floor, roomIndex);
                }
            }

            // プレイヤーキャラを移動
            {
                var position = floor.Space.GetRandomPositionInRoom(random);
                //if (!SpaceUtility.TryLocate(player, floor, position)) { Debug.LogError("生成に失敗しました。"); }
                if (!default(IActiveRogueMethodCaller).Locate(player, null, floor, position, 0f)) { Debug.LogError("生成に失敗しました。"); }
            }

            // 敵を生成
            for (int i = 0; i < _enemyTable.Length; i++)
            {
                _enemyTable[i].GenerateFloor(player, floor, random);
            }

            // アイテムを生成
            for (int i = 0; i < _itemTable.Length; i++)
            {
                _itemTable[i].GenerateFloor(player, floor, random);
            }

            // その他を生成
            for (int i = 0; i < _otherTable.Length; i++)
            {
                _otherTable[i].GenerateFloor(player, floor, random);
            }

            // 空間移動後は obj.Main.IsTicked = true になる。
            // フロアの IsTicked を true にすることで、フロア内の全オブジェクトが同時に行動開始できるようにする。
            // （プレイヤーパーティを IsTicked = false にする手もあるが、オブジェクトも設定しないといけないので面倒）
            floor.Main.IsTicked = true;
        }

        /// <summary>
        /// 敵の自然湧きエフェクト
        /// </summary>
        [ObjectFormer.Formable]
        private class Effect : IRogueEffect, IRogueObjUpdater
        {
            public int lv;

            [System.NonSerialized] private StandardDungeonLevel parent;

            float IRogueObjUpdater.Order => 100f;

            void IRogueEffect.Open(RogueObj self)
            {
                RogueEffectUtility.AddFromRogueEffect(self, this);
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                if (RogueDevice.Primary == null) return default;

                if (parent == null)
                {
                    if (!DungeonInfo.TryGet(self, out var info) ||
                        !info.TryGetLevel(lv, out var level) ||
                        !(level is StandardDungeonLevel standardLevel))
                    {
                        Debug.LogError("ダンジョン階層データの取得に失敗しました。");
                        return default;
                    }

                    parent = standardLevel;
                }

                var enemyTable = parent._enemyTable[0];
                var enemyFaction = enemyTable[0].Option.Race.Option.Faction;
                var enemyMaxCount = enemyTable.MinFrequency; // 最小湧き数に合わせる

                var spaceObjs = self.Space.Objs;
                var enemyCount = 0;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    var obj = spaceObjs[i];
                    if (obj == null) continue;

                    // 敵対キャラを数える
                    // InfoSet に設定されているデフォルト勢力から敵対キャラか判断する
                    // （InfoSet 自体で判断すると自己変化するキャラに対応できない）
                    if (obj.Main.InfoSet.Faction == enemyFaction) { enemyCount++; }
                }

                if (enemyCount < enemyMaxCount)
                {
                    // 敵の数が最大数より少なかった場合、敵を出現させる
                    enemyTable.GenerateFloor(RogueDevice.Primary.Player, self, RogueRandom.Primary, 1);
                }
                return default;
            }

            bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
            IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
            IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
