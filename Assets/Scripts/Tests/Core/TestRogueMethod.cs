using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class TestRogueMethod : ISkill, IEatActiveRogueMethod, IApplyRogueMethod, IAffectRogueMethod, IChangeStateRogueMethod, IChangeEffectRogueMethod
    {
        public static TestRogueMethod Instance { get; } = new TestRogueMethod();

        public string Name => "TestRogueMethod";
        public Sprite Icon => null;
        public Color Color => Color.white;
        public string Caption => null;
        public object Details => null;

        public IRogueMethodTarget Target => null;
        public IRogueMethodRange Range => null;
        public int RequiredMP => 0;
        public Spanning<IKeyword> AmmoCategories => Spanning<IKeyword>.Empty;
        public Spanning<IKeyword> Edibles => Spanning<IKeyword>.Empty;

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            Debug.Log($"{typeof(TestRogueMethod)}: {self}, {user}, {activationDepth}");
            return true;
        }

        public int GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        public bool Equals(ISkill other)
        {
            return other.GetType() == GetType();
        }
    }
}
