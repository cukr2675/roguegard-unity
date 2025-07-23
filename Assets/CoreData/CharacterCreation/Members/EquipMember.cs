using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class EquipMember : IMember, IReadOnlyEquipMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField] private bool _isEquipped = false;
        public bool IsEquipped { get => _isEquipped; set => _isEquipped = value; }
        bool IReadOnlyEquipMember.IsEquipped => _isEquipped;

        private EquipMember() { }

        public static IReadOnlyEquipMember GetMember(IReadOnlyMemberable memberable)
        {
            return (IReadOnlyEquipMember)memberable.GetMember(SourceInstance);
        }

        public static EquipMember GetMember(IMemberableBuilder memberable)
        {
            return (EquipMember)memberable.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            return new EquipMember
            {
                IsEquipped = _isEquipped
            };
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
