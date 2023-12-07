using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class StatusEffectPartyMemberRogueEffect<T> : PartyMemberRogueEffect
        where T : IStatusEffect
    {
        protected sealed override bool MemberIsEffecter(RogueObj partyMember)
        {
            var statusEffectState = partyMember.Main.GetStatusEffectState(partyMember);
            return statusEffectState.TryGetStatusEffect<T>(out _);
        }
    }
}
