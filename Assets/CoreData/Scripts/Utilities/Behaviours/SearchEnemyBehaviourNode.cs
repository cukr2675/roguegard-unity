using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class SearchEnemyBehaviourNode : IRogueBehaviourNode
    {
        public int DistanceThreshold { get; set; }

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;
            if (!self.Location.Space.TryGetRoomView(self.Position, out var room, out _)) { room = new RectInt(); }

            // “G‚ªƒvƒŒƒCƒ„[‚ğ•Ç‰z‚µ‚É@’m‚µ‚Ä‹ß‚Ã‚¢‚Ä‚µ‚Ü‚í‚È‚¢‚æ‚¤‚É‹ŠE‹——£‚ÍŒÅ’è
            var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
            var sqrVisibleRadius = visibleRadius * visibleRadius;

            var spaceObjs = self.Location.Space.Objs;
            RogueObj nearestEnemy = null;
            var nearestSqrDistance = DistanceThreshold * DistanceThreshold;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var obj = spaceObjs[i];
                if (obj == null || !StatsEffectedValues.AreVS(self, obj)) continue;

                // ‹ŠEŠO‚Ì“G‚ğœŠO‚·‚é
                var sqrDistance = (obj.Position - self.Position).sqrMagnitude;
                if (sqrDistance >= sqrVisibleRadius && !room.Contains(obj.Position)) continue;

                if (sqrDistance < nearestSqrDistance)
                {
                    // Å’Z‹——£‚ğXV‚µ‚½‚Æ‚«A“G‚ğİ’è‚·‚é
                    nearestSqrDistance = sqrDistance;
                    nearestEnemy = obj;
                }
            }

            // Å‚à‹ß‚¢“G‚ğ’ÇÕ‘ÎÛ‚Éİ’è‚·‚é
            self.Main.Stats.TargetObj = nearestEnemy;
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
