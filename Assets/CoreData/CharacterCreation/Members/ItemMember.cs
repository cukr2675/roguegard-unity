using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class ItemMember : IMember, IReadOnlyItemMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField] private ScriptableStartingItem _item;
        private StartingItemBuilder builder;
        public StartingItemBuilder Item
        {
            get
            {
                if (_item != null) throw new RogueException();
                else return builder ??= new StartingItemBuilder();
            }
        }
        IReadOnlyStartingItem IReadOnlyItemMember.Item => (IReadOnlyStartingItem)_item ?? builder;

        private ItemMember() { }

        public static IReadOnlyItemMember GetMember(IMemberable intrinsic)
        {
            return (IReadOnlyItemMember)intrinsic.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            var clone = new ItemMember();
            clone._item = _item;
            clone.builder = builder;
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
