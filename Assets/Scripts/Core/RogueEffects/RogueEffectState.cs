using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueEffectState
    {
        private readonly List<IRogueEffect> _effects;

        public Spanning<IRogueEffect> Effects => _effects;

        internal static StaticInitializable<RogueObj> openingObj = new StaticInitializable<RogueObj>(() => null);
        internal static StaticInitializable<bool> openingNow = new StaticInitializable<bool>(() => false);

        [Objforming.CreateInstance]
        private RogueEffectState(bool dummy) { }

        public RogueEffectState()
        {
            _effects = new List<IRogueEffect>();
        }

        public void AddOpen(RogueObj self, IRogueEffect effect)
        {
            //if (self.Main.RogueEffectOpenState == RogueEffectOpenState.NotStarted)
            if (self.Main.RogueEffectOpenState != RogueEffectOpenState.Finished)
            {
                _effects.Add(effect);
                return;
            }
            if (self.Main.RogueEffectOpenState != RogueEffectOpenState.Finished) throw new RogueException(
                $"{self} への {nameof(IRogueEffect)} ({effect}) の付与に失敗しました。 " +
                $"{nameof(RogueObj)} のエフェクト準備中に新しいエフェクトを追加することはできません。");
            //if (openingObj.Value != null) throw new RogueException(
            //    $"{self} への {nameof(IRogueEffect)} ({effect}) の付与に失敗しました。 " +
            //    $"いずれかの {nameof(RogueObj)} ({openingObj.Value}) のエフェクト追加・準備中に新しいエフェクトを追加することはできません。");

            openingObj.Value = self;
            _effects.Add(effect);
            effect.Open(self);
            openingObj.Value = null;
        }

        /// <summary>
        /// 注意：このメソッドはリストから <see cref="IRogueEffect"/> を削除するのみで、
        /// <see cref="ValueEffectState.Remove(IValueEffect)"/> などの解除処理はしない。
        /// （RogueEffectUtility で設定したエフェクトは RogueEffectUtility.RemoveClose で解除可能）
        /// </summary>
        public bool Remove(IRogueEffect effect)
        {
            if (openingObj.Value != null && openingNow.Value) throw new RogueException(
                $"{nameof(IRogueEffect)} ({effect}) の削除に失敗しました。 " +
                $"いずれかの {nameof(RogueObj)} ({openingObj.Value}) のエフェクト準備中にエフェクトを削除することはできません。");

            return _effects.Remove(effect);
        }

        public bool TryGetEffect<T>(out T effect)
            where T : IRogueEffect
        {
            return TryGetEffect(typeof(T), out effect);
        }

        public bool TryGetEffect<T>(System.Type type, out T effect)
            where T : IRogueEffect
        {
            foreach (var item in _effects)
            {
                if (item.GetType() == type)
                {
                    effect = (T)item;
                    return true;
                }
            }
            effect = default;
            return false;
        }

        public bool Contains(IRogueEffect effect)
        {
            return _effects.Contains(effect);
        }

        internal void Open(RogueObj self)
        {
            if (openingObj.Value != null) throw new RogueException(
                $"{self} のエフェクト準備に失敗しました。 " +
                $"いずれかの {nameof(RogueObj)} ({openingObj.Value}) のエフェクト追加・準備中に新しいエフェクト準備を開始することはできません。");

            openingObj.Value = self;
            openingNow.Value = true;
            for (int i = 0; i < _effects.Count; i++)
            {
                _effects[i]?.Open(self);
            }
            openingObj.Value = null;
            openingNow.Value = false;
        }

        internal bool CanStack(RogueObj self, RogueObj other)
        {
            var selfIndex = 0;
            var otherIndex = 0;
            while (true)
            {
                IRogueEffect selfEffect = null;
                IRogueEffect otherEffect = null;
                if (selfIndex < _effects.Count)
                {
                    selfEffect = _effects[selfIndex];
                    selfIndex++;
                    if (selfEffect.CanStack(self, other, null)) continue;
                }
                if (otherIndex < other.Main.RogueEffects._effects.Count)
                {
                    otherEffect = other.Main.RogueEffects._effects[otherIndex];
                    otherIndex++;
                    if (otherEffect.CanStack(self, other, null)) continue;
                }
                if (selfEffect == null && otherEffect == null) break;

                if (selfEffect == null || otherEffect == null || !selfEffect.CanStack(self, other, otherEffect)) return false;
            }
            return true;
        }

        internal RogueEffectState Clone(RogueObj self, RogueObj clonedSelf)
        {
            var clone = new RogueEffectState();
            foreach (var effect in _effects)
            {
                var copiedEffect = effect.DeepOrShallowCopy(self, clonedSelf);
                clone._effects.Add(copiedEffect);
            }
            return clone;
        }

        internal void ReplaceCloned(RogueObj obj, RogueObj clonedObj)
        {
            for (int i = 0; i < _effects.Count; i++)
            {
                _effects[i] = _effects[i].ReplaceCloned(obj, clonedObj);
            }
        }
    }
}
