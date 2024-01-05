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

            // �G���v���C���[��ǉz���Ɏ@�m���ċ߂Â��Ă��܂�Ȃ��悤�Ɏ��E�����͌Œ�
            var visibleRadius = RoguegardSettings.DefaultVisibleRadius;
            var sqrVisibleRadius = visibleRadius * visibleRadius;

            var spaceObjs = self.Location.Space.Objs;
            RogueObj nearestEnemy = null;
            var nearestSqrDistance = DistanceThreshold * DistanceThreshold;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var obj = spaceObjs[i];
                if (obj == null || !StatsEffectedValues.AreVS(self, obj)) continue;

                // ���E�O�̓G�����O����
                var sqrDistance = (obj.Position - self.Position).sqrMagnitude;
                if (sqrDistance >= sqrVisibleRadius && !room.Contains(obj.Position)) continue;

                if (sqrDistance < nearestSqrDistance)
                {
                    // �ŒZ�������X�V�����Ƃ��A�G��ݒ肷��
                    nearestSqrDistance = sqrDistance;
                    nearestEnemy = obj;
                }
            }

            // �ł��߂��G��ǐՑΏۂɐݒ肷��
            self.Main.Stats.TargetObj = nearestEnemy;
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
