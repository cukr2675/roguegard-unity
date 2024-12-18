using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class ScrollOfStinkingCloudBeApplied : ConsumeApplyRogueMethod
    {
        [SerializeField] private ScriptableStartingItem _stinkingCloud = null;

        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => InTheRoomRogueMethodRange.Instance;

        protected override bool BeApplied(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var visible = MessageWorkListener.TryOpenHandler(user.Location, user.Position, out var handler);
            if (visible)
            {
                handler.AppendText(user).AppendText("は").AppendText(self).AppendText("を読んだ！\n");
            }

            using var value = EffectableValue.Get();
            value.Initialize(DungeonInfo.GetLocationVisibleRadius(self));
            ValueEffectState.AffectValue(StdKw.View, value, self);
            var sqrVisibleRadius = value.MainValue * value.MainValue;
            if (!user.Location.Space.TryGetRoomView(user.Position, out var room, out _))
            {
                room = new RectInt(0, 0, 0, 0);
            }
            for (int y = 0; y < user.Location.Space.Tilemap.Height; y++)
            {
                for (int x = 0; x < user.Location.Space.Tilemap.Width; x++)
                {
                    var position = new Vector2Int(x, y);
                    var distance = position - user.Position;
                    if (distance.sqrMagnitude >= sqrVisibleRadius || !room.Contains(position)) continue;
                    if (user.Location.Space.CollideAt(new Vector2Int(x, y))) continue;

                    _stinkingCloud.Option.CreateObj(_stinkingCloud, user.Location, new Vector2Int(x, y), RogueRandom.Primary);
                }
            }

            if (visible)
            {
                handler.AppendText(user).AppendText("のまわりに毒ガスが広がった！\n");
                handler.Dispose();
            }
            return true;
        }
    }
}
