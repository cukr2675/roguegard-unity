using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class EquipMember : IMember, IReadOnlyEquipMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField] private bool _isEquipped = false;
        public bool IsEquipped { get => _isEquipped; set => _isEquipped = value; }
        bool IReadOnlyEquipMember.IsEquipped => _isEquipped;

        private EquipMember() { }

        public static IReadOnlyEquipMember GetMember(IMemberable startingItem)
        {
            return (IReadOnlyEquipMember)startingItem.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            var clone = new EquipMember();
            clone.IsEquipped = _isEquipped;
            return clone;
        }

        private class SourceType : IMemberSource
        {
            public IMember CreateMember()
            {
                return new EquipMember();
            }
        }
    }
}
