using Roguegard.Extensions;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class DungeonFloorAsset : ScriptableObject
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
            foreach (var member in party.Members)
            {
                if (member == player) continue;
                if (member.Main.Stats.Hp <= 0 && StatsEffectedValues.GetMaxHp(member) >= 1) continue; // 倒れていたら移動させない
                if (default(IActiveRogueMethodCaller).LocateNextToAnyMember(member, null, 0f, party)) continue;

                // メンバーの移動に失敗したらランダム位置へ移動
                if (floor.Space.TryGetRandomPositionInRoom(random, out var position) &&
                    default(IActiveRogueMethodCaller).Locate(player, null, floor, position, 0f)) continue;

                Debug.LogError("生成に失敗しました。");
            }
        }

        protected static void GenerateFloor(AssetStartingItemList list, RogueObj player, RogueObj floor, IRogueRandom random, int frequency = -1)
        {
            if (frequency == -1)
            {
                frequency = random.Next(list.MinFrequency, list.MaxFrequency + 1);
            }

            for (int i = 0; i < frequency; i++)
            {
                if (TryGetRandomPosition(player, floor, random, out var position))
                {
                    WeightedRogueObjGeneratorUtility.CreateObj(list, floor, position, random);
                }
            }
        }

        protected static void GenerateFloor(AssetStartingItemList list, RogueObj floor, Vector2Int position, IRogueRandom random, int frequency = -1)
        {
            if (frequency == -1)
            {
                frequency = random.Next(list.MinFrequency, list.MaxFrequency + 1);
            }

            for (int i = 0; i < frequency; i++)
            {
                WeightedRogueObjGeneratorUtility.CreateObj(list, floor, position, random);
            }
        }

        private static bool TryGetRandomPosition(RogueObj player, RogueObj floor, IRogueRandom random, out Vector2Int position)
        {
            const int iteration = 10;
            const int minSqrDistance = 10 * 10;

            if (floor.Stack == 0)
            {
                // 消滅した空間では生成しない
                position = default;
                return false;
            }
            else if (player.Location == floor)
            {
                // プレイヤーキャラがこの階層にいるとき、プレイヤーと同じ部屋または半径10マス以内での出現を避ける

                if (floor.Space.RoomCount >= 2 && floor.Space.TryGetRoomView(player.Position, out var room, out _))
                {
                    for (int j = 0; j < iteration; j++)
                    {
                        if (!floor.Space.TryGetRandomPositionInRoom(random, out position))
                        {
                            position = default;
                            return false;
                        }

                        var sqrDistance = (position - player.Position).sqrMagnitude;
                        if (room.Contains(position) || sqrDistance < minSqrDistance) continue;

                        return true;
                    }
                }

                // 同部屋外の出現に失敗した場合も再試行する
                for (int j = 0; j < iteration; j++)
                {
                    if (!floor.Space.TryGetRandomPositionInRoom(random, out position))
                    {
                        position = default;
                        return false;
                    }

                    var sqrDistance = (position - player.Position).sqrMagnitude;
                    if (sqrDistance < minSqrDistance) continue;

                    return true;
                }

                // 条件に一致する位置が見つからなければとりあえず生成
                return floor.Space.TryGetRandomPositionInRoom(random, out position);

                //// 条件に一致する位置が見つからなければ失敗として位置を返さない
                //position = default;
                //return false;
            }
            else
            {
                return floor.Space.TryGetRandomPositionInRoom(random, out position);
            }
        }
    }
}
