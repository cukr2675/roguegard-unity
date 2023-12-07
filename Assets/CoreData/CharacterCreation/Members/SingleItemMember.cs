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
        private IStartingItemOption _itemOption;
        public IStartingItemOption ItemOption
        {
            get
            {
                if (_itemOption == null) { _itemOption = _item; }
                return _itemOption;
            }
        }

        private SingleItemMember() { }

        public static IReadOnlySingleItemMember GetMember(IMemberable intrinsic)
        {
            return (IReadOnlySingleItemMember)intrinsic.GetMember(SourceInstance);
        }

        public void SetRandom(ICharacterCreationDatabase database, IRogueRandom random)
        {
            if (_item != null) throw new RogueException();

            //_itemOption = database.
        }

        public IMember Clone()
        {
            var clone = new SingleItemMember();
            clone._itemOption = _itemOption ?? _item; // CharacterCreationBuilder 生成時に必ず Clone が実行されるため、この設定だけでシリアル化可能
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
