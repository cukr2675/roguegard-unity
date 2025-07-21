using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class ItemMember : IMember, IReadOnlyItemMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField, Objforming.IgnoreMember] private ScriptableStartingItem _item;
        private StartingItemBuilder builder;
        public StartingItemBuilder Item => builder ??= new StartingItemBuilder(_item);
        IReadOnlyStartingItem IReadOnlyItemMember.Item => Item;

        private ItemMember() { }

        public static IReadOnlyItemMember GetMember(IMemberable intrinsic)
        {
            return (IReadOnlyItemMember)intrinsic.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            var clone = new ItemMember { builder = builder };

            // CharacterCreationBuilder 生成時に必ず Clone が実行されるため、この設定だけでシリアル化可能
            if (clone.builder == null && _item?.Option != null) { clone.builder = new StartingItemBuilder(_item); }

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
