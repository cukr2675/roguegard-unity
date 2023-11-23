using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class ItemMember : IMember, IReadOnlyItemMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField, ObjectFormer.IgnoreMember] private ScriptableStartingItem _item;
        private StartingItemBuilder builder;
        public StartingItemBuilder Item
        {
            get
            {
                if (builder == null) { builder = new StartingItemBuilder(_item); }
                return builder;
            }
        }
        IReadOnlyStartingItem IReadOnlyItemMember.Item => Item;

        private ItemMember() { }

        public static IReadOnlyItemMember GetMember(IMemberable intrinsic)
        {
            return (IReadOnlyItemMember)intrinsic.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            var clone = new ItemMember();
            clone.builder = builder;

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
