using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;

namespace Roguegard
{
    public class StatusEffectState
    {
        private readonly List<IStatusEffect> _statusEffects;

        private bool isDirty;
        private readonly List<IStatusEffect> _statusEffectsWithHeadIcon;

        public Spanning<IStatusEffect> StatusEffects => _statusEffects;

        public Spanning<IStatusEffect> StatusEffectsWithHeadIcon
        {
            get
            {
                if (!isDirty) return _statusEffectsWithHeadIcon;

                isDirty = false;
                _statusEffectsWithHeadIcon.Clear();
                foreach (var statusEffect in _statusEffects)
                {
                    if (statusEffect.HeadIcon != null) { _statusEffectsWithHeadIcon.Add(statusEffect); }
                }
                return _statusEffectsWithHeadIcon;
            }
        }

        public StatusEffectState()
        {
            _statusEffects = new List<IStatusEffect>();
            _statusEffectsWithHeadIcon = new List<IStatusEffect>();
        }

        public void AddFromInfoSet(RogueObj self, IStatusEffect statusEffect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningEffects) throw new RogueException();
            if (statusEffect == null) throw new System.ArgumentNullException(nameof(statusEffect));

            isDirty = true;
            for (int i = 0; i < _statusEffects.Count; i++)
            {
                // 同じ Order の要素が存在するときその手前に追加する。
                if (_statusEffects[i] != null && _statusEffects[i].Order >= statusEffect.Order)
                {
                    _statusEffects.Insert(i, statusEffect);
                    return;
                }
            }
            _statusEffects.Add(statusEffect);
        }

        public void AddFromRogueEffect(RogueObj self, IStatusEffect statusEffect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningInfoSet) throw new RogueException();
            if (statusEffect == null) throw new System.ArgumentNullException(nameof(statusEffect));

            isDirty = true;
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                // 同じ Order の要素が存在するときその後ろに追加する。
                if (_statusEffects[i] != null && _statusEffects[i].Order <= statusEffect.Order)
                {
                    _statusEffects.Insert(i + 1, statusEffect);
                    return;
                }
            }
            _statusEffects.Insert(0, statusEffect);
        }

        public bool Remove(IStatusEffect statusEffect)
        {
            isDirty = true;
            return _statusEffects.Remove(statusEffect);
        }

        public bool TryGetStatusEffect<T>(out T statusEffect)
            where T : IStatusEffect
        {
            return TryGetStatusEffect(typeof(T), out statusEffect);
        }

        public bool TryGetStatusEffect<T>(System.Type type, out T statusEffect)
            where T : IStatusEffect
        {
            foreach (var item in _statusEffects)
            {
                if (item.GetType() == type)
                {
                    statusEffect = (T)item;
                    return true;
                }
            }
            statusEffect = default;
            return false;
        }

        public bool Contains(IStatusEffect statusEffect)
        {
            return _statusEffects.Contains(statusEffect);
        }

        public void GetEffectedName(RogueNameBuilder refName, RogueObj self)
        {
            refName.Clear();
            refName.Append(self.Main.InfoSet.Name);
            foreach (var item in _statusEffects)
            {
                item.GetEffectedName(refName, self);
            }
        }
    }
}
