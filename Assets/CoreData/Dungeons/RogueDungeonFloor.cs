using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public abstract class RogueDungeonFloor : ScriptableObject
    {
        [SerializeField] private int _endLv = 0;
        public int EndLv => _endLv;

        public abstract Spanning<IRogueTile> FillTiles { get; }
        public abstract Spanning<IRogueTile> NoizeTiles { get; }
        public abstract Spanning<IRogueTile> RoomGroundTiles { get; }
        public abstract Spanning<IRogueTile> RoomWallTiles { get; }

        public abstract Spanning<IWeightedRogueObjGeneratorList> EnemyTable { get; }
        public abstract Spanning<IWeightedRogueObjGeneratorList> ItemTable { get; }
        public abstract Spanning<IWeightedRogueObjGeneratorList> OtherTable { get; }

        public abstract void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random);

        protected static void LocatePartyMembers(RogueObj player, RogueObj floor, IRogueRandom random)
        {
            // パーティメンバーを移動
            var party = player.Main.Stats.Party;
            var members = party.Members;
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (member == player) continue;
                if (member.Main.Stats.HP <= 0 && StatsEffectedValues.GetMaxHP(member) >= 1) continue; // 倒れていたら移動させない
                if (default(IActiveRogueMethodCaller).LocateNextToAnyMember(member, null, 0f, party)) continue;

                // メンバーの移動に失敗したらランダム位置へ移動
                if (floor.Space.TryGetRandomPositionInRoom(random, out var position) &&
                    default(IActiveRogueMethodCaller).Locate(player, null, floor, position, 0f)) continue;

                Debug.LogError("生成に失敗しました。");
            }
        }
    }
}
