using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueDungeonRoom
    {
        public RectInt Rect { get; }

        public RectInt RightCorridor { get; private set; }
        public RectInt LeftCorridor { get; private set; }
        public RectInt UpCorridor { get; private set; }
        public RectInt DownCorridor { get; private set; }

        public int XMin => Rect.xMin;
        public int XMax => Rect.xMax;
        public int YMin => Rect.yMin;
        public int YMax => Rect.yMax;
        public int Width => Rect.width;
        public int Height => Rect.height;

        public RogueDungeonRoom(Vector2Int position, Vector2Int size)
        {
            Rect = new RectInt(position, size);
        }

        public RogueDungeonRoom(int xMin, int yMin, int width, int height)
        {
            Rect = new RectInt(xMin, yMin, width, height);
        }

        private RogueDungeonRoom Reshape(int? xMin = null, int? yMin = null, int? width = null, int? height = null)
        {
            var rect = ToRect(xMin, yMin, width, height);
            var room = new RogueDungeonRoom(rect.position, rect.size);
            room.RightCorridor = RightCorridor;
            room.LeftCorridor = LeftCorridor;
            room.UpCorridor = UpCorridor;
            room.DownCorridor = DownCorridor;
            return room;
        }

        private RectInt ToRect(int? xMin = null, int? yMin = null, int? width = null, int? height = null)
        {
            return new RectInt(xMin ?? XMin, yMin ?? YMin, width ?? Width, height ?? Height);
        }

        public void HorizontalDivide(
            int corridorYMin, int corridorHeight, out RogueDungeonRoom downRoom, out RectInt corridor, out RogueDungeonRoom upRoom)
        {
            downRoom = Reshape(height: corridorYMin);
            corridor = ToRect(yMin: downRoom.YMax, height: corridorHeight);
            upRoom = Reshape(yMin: corridor.yMax, height: YMax - corridor.yMax);

            downRoom.UpCorridor = corridor;
            upRoom.DownCorridor = corridor;
        }

        public void VerticalDivide(
            int corridorXMin, int corridorWidth, out RogueDungeonRoom leftRoom, out RectInt corridor, out RogueDungeonRoom rightRoom)
        {
            leftRoom = Reshape(width: corridorXMin);
            corridor = ToRect(xMin: leftRoom.XMax, width: corridorWidth);
            rightRoom = Reshape(xMin: corridor.xMax, width: XMax - corridor.xMax);

            leftRoom.RightCorridor = corridor;
            rightRoom.LeftCorridor = corridor;
        }

        public bool TryGetSharedCorridor(RogueDungeonRoom other, out RectInt corridor)
        {
            corridor = default;
            if (LeftCorridor.size != default && other.RightCorridor.Equals(LeftCorridor)) { corridor = LeftCorridor; }
            if (RightCorridor.size != default && other.LeftCorridor.Equals(RightCorridor)) { corridor = RightCorridor; }
            if (DownCorridor.size != default && other.UpCorridor.Equals(DownCorridor)) { corridor = DownCorridor; }
            if (UpCorridor.size != default && other.DownCorridor.Equals(UpCorridor)) { corridor = UpCorridor; }
            return corridor.size != default;
        }

        public bool HasCorridor(RectInt corridor)
        {
            return corridor.Equals(RightCorridor) || corridor.Equals(LeftCorridor) || corridor.Equals(UpCorridor) || corridor.Equals(DownCorridor);
        }

        public RectInt GetRandomShrinkedRoom(int minWidth, int maxWidth, IRogueRandom random)
        {
            var maxXWidth = Mathf.Min(maxWidth, Width);
            var maxYWidth = Mathf.Min(maxWidth, Height);
            var width = random.Next(minWidth, maxXWidth + 1);
            var height = random.Next(minWidth, maxYWidth + 1);
            var x = XMin + random.Next(0, Width - width + 1);
            var y = YMin + random.Next(0, Height - height + 1);
            return new RectInt(x, y, width, height);
        }
    }
}
