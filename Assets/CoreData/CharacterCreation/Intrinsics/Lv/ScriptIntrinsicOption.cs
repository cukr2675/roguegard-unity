using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/Intrinsic/Script")]
    [ObjectFormer.Referable]
    public class ScriptIntrinsicOption : IntrinsicOption
    {
        [SerializeField] private int _baseLv = 1;
        [SerializeField] private float _baseCost = 0f;
        [SerializeField] private bool _baseCostIsUnknown = false;
        [SerializeField] private ScriptField<Script> _script = null;

        public override Spanning<IMemberSource> MemberSources => _script.Ref.MemberSources;

        public override int GetLv(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
            return _script.Ref.GetLv(this, intrinsic, characterCreationData);
        }

        public override float GetCost(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, out bool costIsUnknown)
        {
            return _script.Ref.GetCost(this, intrinsic, characterCreationData, out costIsUnknown);
        }

        public override ISortedIntrinsic CreateSortedIntrinsic(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
        {
            var lv = GetLv(intrinsic, characterCreationData);
            return _script.Ref.CreateSortedIntrinsic(this, intrinsic, characterCreationData, lv);
        }

        public abstract class Script : ReferableScript
        {
            public virtual Spanning<IMemberSource> MemberSources => Spanning<IMemberSource>.Empty;

            public virtual int GetLv(
                ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData)
            {
                return parent._baseLv;
            }

            public virtual float GetCost(
                ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, out bool costIsUnknown)
            {
                costIsUnknown = parent._baseCostIsUnknown;
                return parent._baseCost;
            }

            public abstract ISortedIntrinsic CreateSortedIntrinsic(
                ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv);
        }
    }
}
