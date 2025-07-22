using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class SingleItemMember : IMember, IReadOnlySingleItemMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField, Objforming.IgnoreMember] private CharacterCreationDataAsset _item;
        private IStartingItemOption _itemOption;
        public IStartingItemOption ItemOption
        {
            get => _itemOption ??= _item;
            set => _itemOption = value;
        }

        private SingleItemMember() { }

        public static IReadOnlySingleItemMember GetMember(IMemberable intrinsic)
        {
            return (IReadOnlySingleItemMember)intrinsic.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            return new SingleItemMember
            {
                _itemOption = _itemOption ?? _item // CharacterCreationBuilder 生成時に必ず Clone が実行されるため、この設定だけでシリアル化可能
            };
        }

        private class SourceType : IMemberSource
        {
            public IMember CreateMember()
            {
                return new SingleItemMember();
            }
        }
    }
}
