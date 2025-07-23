using UnityEngine;

namespace Roguegard
{
    public class LearnSpellBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private ScriptRef<ISkill> _skill = null;

        private Closer closer;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            closer ??= new Closer(_skill.Ref);

            user.Main.Skills.Add(_skill.Ref, MainInfoSetType.Other);
            user.Main.RogueEffects.AddOpen(user, closer);
            return true;
        }

        [Objforming.Formable]
        private class Closer : IRogueEffect, IDungeonFloorCloser
        {
            private readonly ISkill skill;

            [Objforming.CreateInstance]
            private Closer() { }

            public Closer(ISkill skill)
            {
                this.skill = skill;
            }

            void IRogueEffect.Open(RogueObj self)
            {
                DungeonFloorCloserStateInfo.AddTo(self, this);
            }

            void IDungeonFloorCloser.RemoveClose(RogueObj self, bool exitDungeon)
            {
                if (!exitDungeon) return;

                self.Main.Skills.Remove(skill, MainInfoSetType.Other);
                self.Main.RogueEffects.Remove(this);
                DungeonFloorCloserStateInfo.ReplaceWithNull(self, this);
            }

            bool IRogueEffect.CanStack(RogueObj self, RogueObj comingObj, IRogueEffect coming) => false;
            IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
            IRogueEffect IRogueEffect.ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
