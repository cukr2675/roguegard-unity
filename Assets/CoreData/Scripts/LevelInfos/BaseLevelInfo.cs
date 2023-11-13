using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class BaseLevelInfo : ReferableScript, IRogueEffect, ILevelInfo
    {
        public abstract Spanning<int> NextTotalExps { get; }

        protected static void Initialize(RogueObj obj, IRogueEffect levelInfo, int initialLv)
        {
            obj.Main.UpdateLevelInfo(obj, null);
            obj.Main.Stats.SetLv(obj, initialLv);
            obj.Main.RogueEffects.AddOpen(obj, levelInfo);
        }

        public virtual void Open(RogueObj self)
        {
            self.Main.UpdateLevelInfo(self, this);
            RogueEffectUtility.AddFromRogueEffect(self, this);
        }

        public virtual void Close(RogueObj self)
        {
            RogueEffectUtility.RemoveClose(self, this);
        }

        public abstract void LevelUp(RogueObj self);
        public abstract void LevelDown(RogueObj self);

        public abstract bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other);
        public abstract IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf);
        public virtual IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
    }
}
