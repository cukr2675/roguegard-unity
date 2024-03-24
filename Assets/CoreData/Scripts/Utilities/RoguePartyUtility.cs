using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard.Extensions
{
    public static class RoguePartyUtility
    {
        /// <summary>
        /// いずれかのパーティメンバーの隣へ移動する。ただしリーダーと同じ空間に限定される。
        /// </summary>
        public static bool LocateNextToAnyMember(
            this IChangeStateRogueMethodCaller method, RogueObj self, RogueObj user, float activationDepth, RogueParty party)
        {
            var partyMembers = party.Members;
            var targetLocation = partyMembers[0].Location;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                if (member == null || member.Location != targetLocation) continue;

                for (int j = 0; j < 8; j++)
                {
                    var direction = new RogueDirection(j);
                    var position = member.Position + direction.Forward;
                    if (method.Locate(self, user, targetLocation, position, activationDepth)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// いずれかのパーティメンバーの隣へ移動する。ただしリーダーと同じ空間に限定される。
        /// </summary>
        public static bool TryLocateNextToAnyMember(RogueObj self, RogueParty party)
        {
            var partyMembers = party.Members;
            var targetLocation = partyMembers[0].Location;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                if (member == null || member.Location != targetLocation) continue;

                for (int j = 0; j < 8; j++)
                {
                    var direction = new RogueDirection(j);
                    var position = member.Position + direction.Forward;
                    if (SpaceUtility.TryLocate(self, member.Location, position)) return true;
                }
            }
            return false;
        }

        public static bool LocateWithPartyMembers(
            this IChangeStateRogueMethodCaller method, RogueObj self, RogueObj user, RogueObj location, float activationDepth)
        {
            var result = method.Locate(self, user, location, activationDepth);
            var party = self.Main.Stats.Party;
            if (result && party != null)
            {
                var partyMembers = party.Members;
                for (int i = 0; i < partyMembers.Count; i++)
                {
                    var member = partyMembers[i];
                    if (member == self) continue;
                    if (!method.Locate(member, user, location, activationDepth) && !SpaceUtility.TryLocate(member, location))
                    {
                        Debug.LogError($"{member} の移動に失敗しました。");
                    }
                }
            }
            return result;
        }

        public static bool TryLocateWithPartyMembers(RogueObj self, RogueObj location)
        {
            var result = SpaceUtility.TryLocate(self, location);
            var party = self.Main.Stats.Party;
            if (result && party != null)
            {
                var partyMembers = party.Members;
                for (int i = 0; i < partyMembers.Count; i++)
                {
                    var member = partyMembers[i];
                    if (member == self) continue;
                    if (!SpaceUtility.TryLocate(member, location))
                    {
                        Debug.LogError($"{member} の移動に失敗しました。");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// ダンジョンに突入・脱出したときの処理。エフェクトの解除・レベル初期化・全回復をおこなう。
        /// </summary>
        public static void Reset(RogueParty party, IRogueEffect leaderEffect)
        {
            var partyMembers = party.Members;
            {
                // リーダー用エフェクトを設定
                var leader = partyMembers[0];
                leader.Main.RogueEffects.AddOpen(leader, leaderEffect);
            }
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                if (i != 0)
                {
                    // リーダー以外はエフェクトを解除
                    member.Main.UpdatePlayerLeaderInfo(member, null);
                }

                DungeonFloorCloserStateInfo.CloseAndRemoveNull(member, true);

                // レベルを初期化
                RoguegardCharacterCreationSettings.LevelInfoInitializer.InitializeLv(member, 1);

                // 探索開始前に全回復する
                member.Main.Stats.Reset(member);

                member.Main.Stats.Direction = RogueDirection.Down;
            }
        }

        public static void CloseDungeonFloorClosers(RogueParty party, bool exitDungeon)
        {
            var partyMembers = party.Members;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                DungeonFloorCloserStateInfo.CloseAndRemoveNull(member, exitDungeon);
            }
        }

        public static void AssignWithPartyMembers(RogueObj self, RogueParty to)
        {
            var from = self.Main.Stats.Party;
            if (from != null)
            {
                while (from.Members.Count >= 1)
                {
                    var member = from.Members[0];
                    member.Main.Stats.TryAssignParty(member, to);
                }
            }
            else
            {
                self.Main.Stats.TryAssignParty(self, to);
            }
        }
    }
}
