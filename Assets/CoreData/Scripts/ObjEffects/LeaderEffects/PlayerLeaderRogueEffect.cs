using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class PlayerLeaderRogueEffect : IRogueEffect, IPlayerLeaderInfo
    {
        void IRogueEffect.Open(RogueObj self) => Open(self);
        protected virtual void Open(RogueObj self)
        {
            RogueEffectUtility.AddFromRogueEffect(self, this);
            self.Main.UpdatePlayerLeaderInfo(self, this);
        }

        void IPlayerLeaderInfo.Move(RogueObj from, RogueObj to) => Move(from, to);
        protected virtual void Move(RogueObj from, RogueObj to)
        {
            if (from == to) return;

            RogueEffectUtility.RemoveClose(from, this);

            RogueEffectUtility.AddFromRogueEffect(to, this);
            to.Main.UpdatePlayerLeaderInfo(to, this);
        }

        void IPlayerLeaderInfo.RemoveClose(RogueObj self) => RemoveClose(self);
        protected virtual void RemoveClose(RogueObj self)
        {
            RogueEffectUtility.RemoveClose(self, this);
        }

        // プレイヤーパーティのリーダー用なのでスタックしない。
        bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;

        // プレイヤーパーティのリーダー用なので複製しない。
        IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
        IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;

        protected abstract class BasePartyMemberRogueEffect<T> : PartyMemberRogueEffect
            where T : IPlayerLeaderInfo
        {
            protected override bool MemberIsEffecter(RogueObj partyMember)
            {
                return partyMember.Main.GetPlayerLeaderInfo(partyMember) is T;
            }
        }
    }
}
