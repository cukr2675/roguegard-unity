using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.Formable]
    public class AlphabetTypeMember : IMember, IReadOnlyAlphabetTypeMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField] private int _typeIndex;
        public int TypeIndex { get => _typeIndex; set => _typeIndex = value; }

        public string Type => typeChars[_typeIndex].ToString();

        [System.NonSerialized] private readonly List<Sprite> typeItems = new List<Sprite>();
        [System.NonSerialized] private string[] _types;
        public Spanning<string> Types
        {
            get
            {
                if (typeItems.Count != _types?.Length) { _types = typeItems.Select((x, i) => typeStrings[i]).ToArray(); }

                return _types;
            }
        }

        private static readonly char[] typeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly string[] typeStrings = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(x => x.ToString()).ToArray();

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
