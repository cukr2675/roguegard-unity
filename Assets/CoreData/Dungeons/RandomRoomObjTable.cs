using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    public class RandomRoomObjTable : ScriptableStartingItemList
    {
        [SerializeField] private int _minFrequency = 0;
        public int MinFrequency => _minFrequency;

        [SerializeField] private int _maxFrequency = 0;
        public int MaxFrequency => _maxFrequency;

        public void GenerateFloor(RogueObj player, RogueObj floor, IRogueRandom random, int frequency = -1)
        {
            if (frequency == -1)
            {
                frequency = random.Next(_minFrequency, _maxFrequency + 1);
            }

            for (int i = 0; i < frequency; i++)
            {
                if (TryGetRandomPosition(player, floor, random, out var position))
                {
                    WeightedRogueObjGeneratorUtility.CreateObj(this, floor, position, random);
                }
            }
        }

        public void GenerateFloor(RogueObj player, RogueObj floor, Vector2Int position, IRogueRandom random, int frequency = -1)
        {
            if (frequency == -1)
            {
                frequency = random.Next(_minFrequency, _maxFrequency + 1);
            }

            for (int i = 0; i < frequency; i++)
            {
                WeightedRogueObjGeneratorUtility.CreateObj(this, floor, position, random);
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
