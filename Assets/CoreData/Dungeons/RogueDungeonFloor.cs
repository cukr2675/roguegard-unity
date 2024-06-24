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
            // �p�[�e�B�����o�[���ړ�
            var party = player.Main.Stats.Party;
            var members = party.Members;
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (member == player) continue;
                if (member.Main.Stats.HP <= 0 && StatsEffectedValues.GetMaxHP(member) >= 1) continue; // �|��Ă�����ړ������Ȃ�
                if (default(IActiveRogueMethodCaller).LocateNextToAnyMember(member, null, 0f, party)) continue;

                // �����o�[�̈ړ��Ɏ��s�����烉���_���ʒu�ֈړ�
                if (floor.Space.TryGetRandomPositionInRoom(random, out var position) &&
                    default(IActiveRogueMethodCaller).Locate(player, null, floor, position, 0f)) continue;

                Debug.LogError("�����Ɏ��s���܂����B");
            }
        }
    }
}
