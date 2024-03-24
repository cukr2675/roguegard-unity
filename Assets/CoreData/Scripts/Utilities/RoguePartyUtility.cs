using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Extensions
{
    public static class RoguePartyUtility
    {
        /// <summary>
        /// いずれかのパーティメンバーの隣へ移動する
        /// </summary>
        public static bool LocateNextToAnyMember(
            this IChangeStateRogueMethodCaller method, RogueObj self, RogueObj user, float activationDepth, RogueParty party)
        {
            var partyMembers = party.Members;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                if (member == null) continue;

                for (int j = 0; j < 8; j++)
                {
                    var direction = new RogueDirection(j);
                    var position = member.Position + direction.Forward;
                    if (method.Locate(self, user, member.Location, position, activationDepth)) return true;
                }
            }
            return false;
        }

        public static bool TryLocateNextToAnyMember(RogueObj self, RogueParty party)
        {
            var partyMembers = party.Members;
            for (int i = 0; i < partyMembers.Count; i++)
            {
                var member = partyMembers[i];
                if (member == null) continue;

                for (int j = 0; j < 8; j++)
                {
                    var direction = new RogueDirection(j);
                    var position = member.Position + direction.Forward;
                    if (SpaceUtility.TryLocate(self, member.Location, position)) return true;
                }
            }
            return false;
        }
    }
}
