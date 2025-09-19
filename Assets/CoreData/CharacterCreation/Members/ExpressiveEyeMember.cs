using OchalikeSprites;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class ExpressiveEyeMember : IMember, IReadOnlyExpressiveEyeMember
    {
        public static IMemberSource SourceInstance { get; } = new SourceType();

        IMemberSource IReadOnlyMember.Source => SourceInstance;

        [SerializeField] private ExpressiveEyeType _type;
        public ExpressiveEyeType Type { get => _type; set => _type = value; }

        [System.NonSerialized] private readonly List<(ExpressiveEyeType type, Sprite icon)> typeItems = new();
        [System.NonSerialized] private string[] _types;
        public Spanning<string> Types
        {
            get
            {
                if (typeItems.Count != _types?.Length) { _types = typeItems.Select((x) => x.type.ToString()).ToArray(); }

                return _types;
            }
        }

        private ExpressiveEyeMember() { }

        public static IReadOnlyExpressiveEyeMember GetMember(IReadOnlyAppearance appearance)
        {
            return (IReadOnlyExpressiveEyeMember)appearance.GetMember(SourceInstance);
        }

        public void AddTypeItem(ExpressiveEyeType type, Sprite icon)
        {
            typeItems.Add((type, icon));
        }

        public void ClearTypeItems()
        {
            typeItems.Clear();
        }

        public IMember Clone()
        {
            return new ExpressiveEyeMember
            {
                _type = _type
            };
        }

        private class SourceType : IMemberSource
        {
            public IMember CreateMember()
            {
                return new ExpressiveEyeMember();
            }
        }
    }
}
