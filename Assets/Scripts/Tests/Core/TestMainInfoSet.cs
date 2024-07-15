using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;

namespace Roguegard
{
    public class TestMainInfoSet : IMainInfoSet
    {
        public static TestMainInfoSet Instance { get; } = new TestMainInfoSet();

        public string Name => "TesterRogueObj";
        public Sprite Icon => null;
        public Color Color => Color.white;
        public string Caption => null;
        public IRogueDetails Details => null;

        public IKeyword Category => null;
        public int MaxHP => 0;
        public int MaxMP => 0;
        public int ATK => 0;
        public int DEF => 0;
        public float Weight => 1;
        public float LoadCapacity => 0;

        public ISerializableKeyword Faction => null;
        public Spanning<ISerializableKeyword> TargetFactions => Spanning<ISerializableKeyword>.Empty;
        public MainInfoSetAbility Ability => MainInfoSetAbility.Object;
        public IRogueMaterial Material => null;
        public IRogueGender Gender => null;
        public string HPName => null;
        public string MPName => null;
        public float Cost => 0f;
        public bool CostIsUnknown => false;

        public Spanning<IWeightedRogueObjGeneratorList> LootTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        public IActiveRogueMethod Walk => TestRogueMethod.Instance;
        public IActiveRogueMethod Wait => TestRogueMethod.Instance;
        public ISkill Attack => TestRogueMethod.Instance;
        public ISkill Throw => TestRogueMethod.Instance;
        public IActiveRogueMethod PickUp => TestRogueMethod.Instance;
        public IActiveRogueMethod Put => TestRogueMethod.Instance;
        public IEatActiveRogueMethod Eat => TestRogueMethod.Instance;

        public IAffectRogueMethod Hit => TestRogueMethod.Instance;
        public IAffectRogueMethod BeDefeated => TestRogueMethod.Instance;
        public IChangeStateRogueMethod Locate => TestRogueMethod.Instance;
        public IChangeStateRogueMethod Polymorph => TestRogueMethod.Instance;

        public IApplyRogueMethod BeApplied => TestRogueMethod.Instance;
        public IApplyRogueMethod BeThrown => TestRogueMethod.Instance;
        public IApplyRogueMethod BeEaten => TestRogueMethod.Instance;

        public IMainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            return this;
        }

        public void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
        }

        public IMainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType, int deltaLv)
        {
            return this;
        }

        public IEquipmentInfo GetEquipmentInfo(RogueObj self)
        {
            return null;
        }

        public IEquipmentState GetEquipmentState(RogueObj self)
        {
            return null;
        }

        public void GetObjSprite(RogueObj self, out IRogueObjSprite objSprite, out ISpriteMotionSet motionSet)
        {
            throw new System.NotSupportedException();
        }

        public bool Equals(IMainInfoSet other)
        {
            return other?.GetType() == GetType();
        }
    }
}
