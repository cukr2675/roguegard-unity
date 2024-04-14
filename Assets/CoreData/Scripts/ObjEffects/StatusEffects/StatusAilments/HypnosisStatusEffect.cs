using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    [Objforming.Formable]
    public class HypnosisStatusEffect : TimeLimitedStackableStatusEffect, IValueEffect, IBoneSpriteEffect
    {
        public static IAffectCallback Callback { get; } = new AffectCallback(new HypnosisStatusEffect());

        public override string Name => "催眠";
        public override IKeyword EffectCategory => EffectCategoryKw.StatusAilment;
        protected override int MaxStack => 1;
        protected override int InitialLifeTime => 1;

        protected override float UpdaterOrder => -100f; // 反撃で付与されたとき、そのターンでは催眠行動しない。
        float IValueEffect.Order => 0f;
        float IBoneSpriteEffect.Order => -100f;

        private HypnosisStatusEffect() { }

        protected override IRogueEffect AffectTo(RogueObj target, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var statusEffect = base.AffectTo(target, user, activationDepth, arg);
            if (MessageWorkListener.TryOpenHandler(target.Location, target.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(target).AppendText("は").AppendText(this).AppendText("にかかった！\n");
            }
            return statusEffect;
        }

        public override void Open(RogueObj self)
        {
            base.Open(self);
            SpeedCalculator.SetDirty(self);
        }

        protected override void RemoveClose(RogueObj self, StatusEffectCloseType closeType = StatusEffectCloseType.Manual)
        {
            SpeedCalculator.SetDirty(self);
            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(self).AppendText("の").AppendText(this).AppendText("が解けた\n");
            }
            base.RemoveClose(self);
        }

        protected override RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            if (!self.Location.Space.TryGetRoomView(self.Position, out var room, out _)) { room = new RectInt(); }

            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(self).AppendText("は混乱している\n");
            }

            // アイテム使用
            // 敵が催眠にかかったときプレイヤーを壁越しに察知しないようにする
            // プレイヤーが催眠にかかったときは敵と同等の振る舞いをさせる
            var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
            if (AutoAction.TryHypnosisOtherAction(self, activationDepth, visibleRadius, room, RogueRandom.Primary)) return default;

            // 何もしない
            return default;
        }

        void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
        {
            if (keyword == StatsKw.Speed)
            {
                value.SubValues[StatsKw.BeInhibited] = 1f;
            }
        }

        public override bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is HypnosisStatusEffect effect && effect.Stack == Stack && effect.LifeTime == LifeTime;
        }

        public override IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            return new HypnosisStatusEffect()
            {
                Stack = Stack,
                LifeTime = LifeTime
            };
        }

        void IBoneSpriteEffect.AffectSprite(RogueObj self, IReadOnlyNodeBone rootNode, AffectableBoneSpriteTable boneSpriteTable)
        {
            // 目の色を変更
            var color = new Color32(255, 0, 255, 255);
            boneSpriteTable.SetFirstSprite(BoneKeyword.LeftEye, color);
            boneSpriteTable.SetFirstSprite(BoneKeyword.RightEye, color);
        }
    }
}
