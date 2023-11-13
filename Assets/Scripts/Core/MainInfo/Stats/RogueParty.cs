using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class RogueParty
    {
        private readonly List<RogueObj> _members;
        public Spanning<RogueObj> Members => _members;

        public ISerializableKeyword Faction { get; set; }

        private ISerializableKeyword[] _targetFactions;
        public Spanning<ISerializableKeyword> TargetFactions => _targetFactions;

        [ObjectFormer.CreateInstance]
        private RogueParty() { }

        public RogueParty(ISerializableKeyword faction, Spanning<ISerializableKeyword> targetFactions)
        {
            _members = new List<RogueObj>();
            Faction = faction;
            _targetFactions = targetFactions.ToArray();
        }

        internal bool TryAddMember(RogueObj obj)
        {
            if (_members.Contains(obj)) return false;

            _members.Add(obj);
            return true;
        }

        internal bool RemoveMember(RogueObj obj)
        {
            return _members.Remove(obj);
        }

        public static bool Equals(RogueObj left, RogueObj right)
        {
            // どちらかが null ならその時点で false
            if (left == null || right == null) return false;

            // 同じキャラか、同じパーティなら true
            return left == right || left.Main.Stats.Party == right.Main.Stats.Party;
        }
    }
}
