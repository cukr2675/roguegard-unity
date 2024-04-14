using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class ScrollOfConfuseMonsterBeApplied : ConsumeApplyRogueMethod
    {
        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => InTheRoomRogueMethodRange.Instance;

        protected override bool BeApplied(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var visible = MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var handler);
            if (visible)
            {
                handler.AppendText(user).AppendText("は").AppendText(self).AppendText("を読んだ！\n");
                handler.Dispose();
            }

            using var value = AffectableValue.Get();
            value.Initialize(DungeonInfo.GetLocationVisibleRadius(self));
            ValueEffectState.AffectValue(StdKw.View, value, self);
            var visibleRadius = value.MainValue;
            using var predicator = Target.GetPredicator(user, 0f, null);
            if (!user.Location.Space.TryGetRoomView(user.Position, out var room, out _))
            {
                room = new RectInt(0, 0, 0, 0);
            }
            Range.Predicate(predicator, user, 0f, null, visibleRadius, room);
            predicator.EndPredicate();
            var objs = predicator.GetObjs(user.Position);
            for (int i = 0; i < objs.Count; i++)
            {
                this.TryAffect(objs[i], activationDepth, ConfusionStatusEffect.Callback, self, user);
            }
            return true;
        }
    }
}
