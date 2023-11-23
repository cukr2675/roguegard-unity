using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class SingleItemMember : IMember, IReadOnlySingleItemMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField, ObjectFormer.IgnoreMember] private ScriptableCharacterCreationData _item;
        private IStartingItemOption builder;
        public IStartingItemOption ItemOption
        {
            get
            {
                if (_item != null) throw new RogueException();
                else return builder;
            }
        }
        IStartingItemOption IReadOnlySingleItemMember.ItemOption => _item ?? builder;

        private SingleItemMember() { }

        public static IReadOnlySingleItemMember GetMember(IMemberable intrinsic)
        {
            return (IReadOnlySingleItemMember)intrinsic.GetMember(SourceInstance);
        }

        public IMember Clone()
        {
            var clone = new SingleItemMember();
            clone._item = _item;
            clone.builder = builder;
            return clone;
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
