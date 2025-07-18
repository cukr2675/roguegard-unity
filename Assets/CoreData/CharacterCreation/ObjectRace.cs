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
        public string HpName => _hpName;

        [SerializeField] private string _mpName;
        public string MpName => _mpName;

        [SerializeField] private MemberList _members;

        IRaceOption IReadOnlyRace.Option => this;
        string IReadOnlyRace.OptionName => null;
        Color IReadOnlyRace.BodyColor => Color;
        string IReadOnlyRace.OptionCaption => null;
        IRogueDetails IReadOnlyRace.OptionDetails => null;
        Spanning<IMemberSource> IMemberable.MemberSources => ((IRaceOption)this).MemberSources;

        IReadOnlyMember IMemberable.GetMember(IMemberSource source)
        {
            foreach (var member in _members.Span)
            {
                if (member.Source == source) return member;
            }
            throw new System.ArgumentException($"{source} の {nameof(IMember)} が見つかりません。");
        }
    }
}
