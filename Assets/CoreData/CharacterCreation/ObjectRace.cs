using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [System.Serializable]
    [Objforming.IgnoreRequireRelationalComponent]
    public class ObjectRace : ObjectRaceOption, IReadOnlyRace
    {
        [Header("Race")]

        [SerializeField] private int _lv;
        public int Lv => _lv;

        [SerializeField] private RogueGender _gender;
        public IRogueGender Gender => _gender;

        [SerializeField] private string _hpName;
        public string HPName => _hpName;

        [SerializeField] private string _mpName;
        public string MPName => _mpName;

        [SerializeField] private MemberList _members;

        IRaceOption IReadOnlyRace.Option => this;
        string IReadOnlyRace.OptionName => null;
        Color IReadOnlyRace.BodyColor => Color;
        string IReadOnlyRace.OptionCaption => null;
        IRogueDetails IReadOnlyRace.OptionDetails => null;
        IMemberableOption IMemberable.MemberableOption => this;

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
        {
            var members = (Spanning<IMember>)_members;
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException($"{source} の {nameof(IMember)} が見つかりません。");
        }
    }
}
