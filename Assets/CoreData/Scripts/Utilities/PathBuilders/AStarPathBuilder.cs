using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class AStarPathBuilder : IPathBuilder
    {
        private readonly AStarPathBuilderNodemap nodemap;
        private readonly List<Vector2Int> path;
        private Vector2Int targetPosition;

        public AStarPathBuilder(Vector2Int capacity)
        {
            nodemap = new AStarPathBuilderNodemap(capacity);
            path = new List<Vector2Int>();
        }

        public bool UpdatePath(RogueObj self, Vector2Int targetPosition)
        {
            this.targetPosition = targetPosition;
            return UpdatePath(self, false);
        }

        private bool UpdatePath(RogueObj self, bool needsUpdateTile)
        {
            var updated = nodemap.UpdateMap(self);
            if (needsUpdateTile && !updated) return false;

            path.Clear();
            if (!nodemap.Rect.Contains(targetPosition) || nodemap[targetPosition].HasCollider || targetPosition == self.Position)
            {
                // �ړ��ł��Ȃ��ʒu���w�肵���ꍇ�A�p�X�𐶐������ɒ��i����
                return true;
            }

            nodemap.Reset();

            // �J�n�n�_���J��
            var startPosition = self.Position;
            nodemap[startPosition].TryOpen(0, targetPosition - startPosition, -Vector2Int.one);

            var size = nodemap.Rect.width * nodemap.Rect.height;
            for (int i = 0; i < size; i++)
            {
                if (!nodemap.TryGetLowestFOpenNodePosition(out var position)) break;

                var targetIsOpened = OpenNeighbors(position, targetPosition);
                if (targetIsOpened)
                {
                    SetPath(startPosition, targetPosition);
                    return true;
                }
            }
            return false;
        }

        private bool OpenNeighbors(Vector2Int position, Vector2Int targetPosition)
        {
            var positionNode = nodemap[position];
            positionNode.Close();
            for (int i = 0; i < 8; i++)
            {
                var deltaPosition = new RogueDirection(i).Forward;
                var openPosition = position + deltaPosition;
                if (!nodemap.Rect.Contains(openPosition)) continue; // �͈͊O�����O����

                // �΂߂Ɉړ��ł��Ȃ��ꍇ�����O����
                if (deltaPosition.x != 0 && deltaPosition.y != 0)
                {
                    if (nodemap[openPosition.y, position.x].HasCornerCollider) continue;
                    if (nodemap[position.y, openPosition.x].HasCornerCollider) continue;
                }

                // �ړ��ł��Ȃ��m�[�h�����O����
                var openNode = nodemap[openPosition];
                if (openNode.HasCollider) continue;

                var costedG = positionNode.G + 1; // �ړ��R�X�g 1 ���Z
                var relativePosition = targetPosition - openPosition;
                openNode.TryOpen(costedG, relativePosition, position);

                // �ڕW�ʒu�ɓ��B������ true
                if (openPosition == targetPosition) return true;
            }
            return false;
        }

        private void SetPath(Vector2Int startPosition, Vector2Int targetPosition)
        {
            // �m�[�h�̏�Ԃ����ƂɁA�J�n�n�_����S�[���܂ł̃p�X���m�肷��B
            path.Add(targetPosition);
            var node = nodemap[targetPosition];
            var size = nodemap.Rect.width * nodemap.Rect.height;
            for (int i = 0; i < size; i++)
            {
                path.Insert(0, node.CameFrom);
                if (node.CameFrom == startPosition) break;

                node = nodemap[node.CameFrom];
            }
        }

        public bool TryGetNextDirection(RogueObj self, out RogueDirection nextDirection)
        {
            nextDirection = RogueDirection.LowerLeft;
            var currentPosition = self.Position;
            if (currentPosition == targetPosition) return false;
            if (path.Count == 0 && MovementUtility.TryGetApproachDirection(self, targetPosition, true, out nextDirection))
            {
                // �p�X���Ȃ��ꍇ�́A�ړ��ł��Ȃ��ʒu���w�肳��Ă���Ɣ��f���Ē��i����
                return true;
            }
            else
            {
                // �p�X�����ǂ�
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var pathPoint = path[i];
                    if (pathPoint != currentPosition) continue;

                    nextDirection = RogueDirection.FromSignOrLowerLeft(path[i + 1] - currentPosition);
                    return true;
                }
            }

            if (!UpdatePath(self, true)) return false;
            if (path.Count <= 1) return false;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var pathPoint = path[i];
                if (pathPoint != currentPosition) continue;

                nextDirection = RogueDirection.FromSignOrLowerLeft(path[i + 1] - currentPosition);
                return true;
            }

            Debug.LogError($"{nameof(AStarPathBuilder)}�G���[");
            return false;
        }
    }
}
