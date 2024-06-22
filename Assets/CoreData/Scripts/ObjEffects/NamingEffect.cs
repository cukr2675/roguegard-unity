using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class NamingEffect : StackableStatusEffect
    {
        /// <summary>
        /// <see cref="RogueMethodArgument.Other"/> の <see cref="string"/> をオブジェクトに命名する
        /// </summary>
        public static IAffectCallback Callback { get; } = new AffectCallback(new NamingEffect());

        public string Naming { get; set; }
        public string NamingCaption { get; set; }
        public IRogueDetails NamingDetails { get; set; }

        public override string Name => null;
        public override IKeyword EffectCategory => EffectCategoryKw.Dummy;
        protected override int MaxStack => 1;

        public static NamingEffect Get(RogueObj obj)
        {
            if (obj.Main.RogueEffects.TryGetEffect<NamingEffect>(out var effect)) return effect;
            else return null;
        }

        public override void GetEffectedName(RogueNameBuilder refName, RogueObj self)
        {
            refName.Insert0("と名付けられた");
            refName.Insert0(Naming);
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is NamingEffect effect && effect.Naming == Naming && effect.NamingCaption == NamingCaption && effect.NamingDetails == NamingDetails;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new NamingEffect() { Naming = Naming, NamingCaption = NamingCaption, NamingDetails = NamingDetails };
        }
    }
}
