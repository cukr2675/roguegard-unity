using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class ItemMember : IMember, IReadOnlyItemMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField, Objforming.IgnoreMember] private AssetStartingItem _editorItem;
        private StartingItem _item;
        public StartingItem Item => _item ??= new StartingItem(_editorItem);
        IReadOnlyStartingItem IReadOnlyItemMember.Item => Item;

        private ItemMember() { }

        public static IReadOnlyItemMember GetMember(IReadOnlyMemberable intrinsic)
        {
            return (IReadOnlyItemMember)intrinsic.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            var clone = new ItemMember { _item = _item };

            // CharacterCreationData 生成時に必ず Clone が実行されるため、この設定だけでシリアル化可能
            if (clone._item == null && _editorItem?.Option != null) { clone._item = new StartingItem(_editorItem); }

            return clone;
        }

        private class SourceType : IMemberSource
        {
            public IMember CreateMember()
            {
                return new ItemMember();
            }
        }
    }
}
