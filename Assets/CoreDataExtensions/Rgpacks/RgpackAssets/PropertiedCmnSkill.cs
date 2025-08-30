using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class PropertiedCmnSkill : ISkill
    {
        private readonly PropertiedCmnReference reference;

        public string Name => null;
        public Sprite Icon => null;
        public Color Color => Color.white;
        public string Caption => null;
        public IRogueDetails Details => null;
        public Spanning<IKeyword> Tags => Spanning<IKeyword>.Empty;

        public IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public IRogueMethodRange Range => FrontRogueMethodRange.Instance;
        public int RequiredMp => 0;
        public Spanning<IKeyword> AmmoCategories => Spanning<IKeyword>.Empty;

        private PropertiedCmnSkill(PropertiedCmnData data, string envRgpackId)
        {
            reference = data.ToReference(envRgpackId);
        }

        public static ISkill Create(PropertiedCmnData data, string envRgpackId, ISkill defaultMethod)
        {
            return string.IsNullOrWhiteSpace(data.Cmn) ? defaultMethod : new PropertiedCmnSkill(data, envRgpackId);
        }

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var result = reference.Invoke();
            return result == null || result is bool boolean && boolean == true;
        }

        public int GetAtk(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        public bool Equals(ISkill other)
        {
            return other == this;
        }
    }
}
