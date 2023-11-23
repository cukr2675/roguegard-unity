using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class AlphabetTypeMember : IMember, IReadOnlyAlphabetTypeMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField] private int _typeIndex;
        public int TypeIndex { get => _typeIndex; set => _typeIndex = value; }

        public char TypeChar => typeChars[_typeIndex];

        [System.NonSerialized] private readonly List<Sprite> typeItems = new List<Sprite>();

        private static readonly char[] typeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private AlphabetTypeMember() { }

        public static IReadOnlyAlphabetTypeMember GetMember(IReadOnlyAppearance appearance)
        {
            return (IReadOnlyAlphabetTypeMember)appearance.GetMember(SourceInstance);
        }

        public void AddTypeItem(Sprite icon)
        {
            typeItems.Add(icon);
        }

        public void ClearTypeItems()
        {
            typeItems.Clear();
        }

        public IMember Clone()
        {
            var clone = new AlphabetTypeMember();
            clone._typeIndex = _typeIndex;
            return clone;
        }

        private class SourceType : IMemberSource
        {
            public IMember CreateMember()
            {
                return new AlphabetTypeMember();
            }
        }
    }
}
