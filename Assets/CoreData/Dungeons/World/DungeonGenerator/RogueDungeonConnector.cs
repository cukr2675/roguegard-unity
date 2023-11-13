using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="RogueDungeonRoom"/> の接続情報を持つクラス
    /// </summary>
    public class RogueDungeonConnector
    {
        public RogueDungeonRoom Room0 { get; private set; }
        public RectInt Corridor { get; }
        public RogueDungeonRoom Room1 { get; private set; }

        public RogueDungeonConnector(RogueDungeonRoom room0, RogueDungeonRoom room1)
        {
            Room0 = room0;
            Room1 = room1;
            if (!room0.TryGetSharedCorridor(room1, out var corridor)) throw new RogueException("共通の通路を持たない部屋を接続しようとしました。");

            Corridor = corridor;
        }

        /// <summary>
        /// <see cref="Room0"/> を分割した二つの部屋のどちらかに再設定する。どちらか片方としか隣接しない場合はそちらを、両方と隣接する場合はランダムで設定する。
        /// </summary>
        public void SetRoom0(RogueDungeonRoom a, RogueDungeonRoom b, IRogueRandom random)
        {
            Room0 = GetConnectableRoom(a, b, random);
        }

        /// <summary>
        /// <see cref="Room1"/> を分割した二つの部屋のどちらかに再設定する。どちらか片方としか隣接しない場合はそちらを、両方と隣接する場合はランダムで設定する。
        /// </summary>
        public void SetRoom1(RogueDungeonRoom a, RogueDungeonRoom b, IRogueRandom random)
        {
            Room1 = GetConnectableRoom(a, b, random);
        }

        private RogueDungeonRoom GetConnectableRoom(RogueDungeonRoom a, RogueDungeonRoom b, IRogueRandom random)
        {
            var aHasCorridor = a.HasCorridor(Corridor);
            var bHasCorridor = b.HasCorridor(Corridor);
            if (aHasCorridor && bHasCorridor) return random.Choice(a, b);
            if (aHasCorridor && !bHasCorridor) return a;
            if (!aHasCorridor && bHasCorridor) return b;
            throw new RogueException("共通の通路を持たない部屋を接続しようとしました。");
        }
    }
}
