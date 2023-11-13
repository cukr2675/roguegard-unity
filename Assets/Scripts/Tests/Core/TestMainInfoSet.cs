using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class TestMainInfoSet : MainInfoSet
    {
        public static TestMainInfoSet Instance { get; } = new TestMainInfoSet();

        public override string Name => "TesterRogueObj";
        public override Sprite Icon => null;
        public override Color Color => Color.white;
        public override string Caption => null;
        public override object Details => null;

        public override IKeyword Category => null;
        public override int MaxHP => 0;
        public override int MaxMP => 0;
        public override int ATK => 0;
        public override int DEF => 0;
        public override float Weight => 1;
        public override float LoadCapacity => 0;

        public override ISerializableKeyword Faction => null;
        public override Spanning<ISerializableKeyword> TargetFactions => Spanning<ISerializableKeyword>.Empty;
        public override MainInfoSetAbility Ability => MainInfoSetAbility.Object;
        public override IRogueMaterial Material => null;
        public override IRogueGender Gender => null;
        public override string HPName => null;
        public override string MPName => null;
        public override float Cost => 0f;
        public override bool CostIsUnknown => false;

        public override Spanning<IWeightedRogueObjGeneratorList> LootTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        public override IActiveRogueMethod Walk => TestRogueMethod.Instance;
        public override IActiveRogueMethod Wait => TestRogueMethod.Instance;
        public override ISkill Attack => TestRogueMethod.Instance;
        public override ISkill Throw => TestRogueMethod.Instance;
        public override IActiveRogueMethod PickUp => TestRogueMethod.Instance;
        public override IActiveRogueMethod Put => TestRogueMethod.Instance;
        public override IEatActiveRogueMethod Eat => TestRogueMethod.Instance;

        public override IAffectRogueMethod Hit => TestRogueMethod.Instance;
        public override IAffectRogueMethod BeDefeated => TestRogueMethod.Instance;
        public override IChangeStateRogueMethod Locate => TestRogueMethod.Instance;
        public override IChangeStateRogueMethod Polymorph => TestRogueMethod.Instance;

        public override IApplyRogueMethod BeApplied => TestRogueMethod.Instance;
        public override IApplyRogueMethod BeThrown => TestRogueMethod.Instance;
        public override IApplyRogueMethod BeEaten => TestRogueMethod.Instance;

        public override MainInfoSet Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
        {
            return this;
        }

        public override void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
        {
        }

        public override MainInfoSet Reopen(RogueObj self, MainInfoSetType infoSetType)
        {
            return this;
        }

        public override IEquipmentInfo GetEquipmentInfo(RogueObj self)
        {
            return null;
        }

        public override IEquipmentState GetEquipmentState(RogueObj self)
        {
            return null;
        }

        public override void GetObjSprite(RogueObj self, out RogueObjSprite objSprite, out IMotionSet motionSet)
        {
            throw new System.NotSupportedException();
        }

        public override bool Equals(MainInfoSet other)
        {
            return other?.GetType() == GetType();
        }
    }
}
