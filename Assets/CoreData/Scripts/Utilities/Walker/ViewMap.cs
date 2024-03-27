using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    internal class ViewMap
    {
        public RogueObj Location { get; private set; }

        // オブジェクトが変化すると視界記憶上のオブジェクトも変化する。
        public RogueObjList VisibleObjs { get; private set; }

        private bool[][] visibles;

        private IRogueTile[][] floorTilemap;

        private IRogueTile[][] buildingTilemap;

        private RogueObj[][] tilemapObjs;

        public int Width => visibles[0].Length;

        public int Height => visibles.Length;

        private ViewMap() { }

        public ViewMap(Vector2Int size)
        {
            VisibleObjs = new RogueObjList();
            var height = size.y;
            var width = size.x;
            visibles = new bool[height][];
            floorTilemap = new IRogueTile[height][];
            buildingTilemap = new IRogueTile[height][];
            tilemapObjs = new RogueObj[height][];
            for (int y = 0; y < visibles.Length; y++)
            {
                visibles[y] = new bool[width];
                floorTilemap[y] = new IRogueTile[width];
                buildingTilemap[y] = new IRogueTile[width];
                tilemapObjs[y] = new RogueObj[width];
            }
        }

        public RogueObj GetVisibleObj(int index)
        {
            return VisibleObjs[index];
        }

        public void AddUnique(RogueObj obj)
        {
            if (obj == null) return;

            VisibleObjs.TryAddUnique(obj);
        }

        public void AddPoint(Vector2Int point)
        {
            var locationSpace = Location.Space;
            var floorTile = locationSpace.Tilemap.Get(point, RogueTileLayer.Floor);
            var tile = locationSpace.Tilemap.Get(point, RogueTileLayer.Building);
            SetTile(floorTile, tile, point.x, point.y);
        }

        public void AddView(RogueObj self)
        {
            if (self.Location != Location) return;
            if (self.Location == null) return;

            // プレイヤーキャラクターは常に視界内にする。
            visibles[self.Position.y][self.Position.x] = true;
            AddUnique(self);

            // 視界半径を取得
            using var value = AffectableValue.Get();
            value.Initialize(DungeonInfo.GetLocationVisibleRadius(self));
            ValueEffectState.AffectValue(StdKw.View, value, self);
            var visibleRadius = value.MainValue;

            // 視界がゼロのときは何もしない。
            if (visibleRadius <= 0f) return;

            var locationSpace = self.Location.Space;
            if (locationSpace.Tilemap == null) return;

            var position = self.Position;
            SetViewRadius(position, visibleRadius);

            SetView(position, locationSpace);

            // 周囲のオブジェクトを視界に追加する。
            // ペイントされたオブジェクトも追加する
            var locationObjs = locationSpace.Objs;
            var selfPosition = (Vector2)self.Position;
            var viewSqrRadius = visibleRadius * visibleRadius;
            for (int i = 0; i < locationObjs.Count; i++)
            {
                var locationObj = locationObjs[i];
                if (locationObj == null) continue;

                var relativePosition = locationObj.Position - selfPosition;
                var sqrDistance = relativePosition.sqrMagnitude;
                var statusEffectState = locationObj.Main.GetStatusEffectState(locationObj);
                if (sqrDistance >= viewSqrRadius && !statusEffectState.TryGetStatusEffect<PaintStatusEffect>(out _)) continue;

                if (locationObj.AsTile)
                {
                    var floorTile = locationSpace.Tilemap.Get(locationObj.Position, RogueTileLayer.Floor);
                    SetTile(floorTile, locationObj, locationObj.Position.x, locationObj.Position.y);
                    AddUnique(locationObj);
                }
                else
                {
                    AddUnique(locationObj);
                }
            }

            // 同じ部屋のオブジェクトを視界に追加する。
            if (locationSpace.TryGetRoomView(position, out _, out var roomObjs))
            {
                for (int i = 0; i < roomObjs.Count; i++)
                {
                    AddUnique(roomObjs[i]);
                }
            }
        }

        private void SetViewRadius(Vector2Int position, float visibleRadius)
        {
            var sqrVisibleRadius = visibleRadius * visibleRadius;
            var floorValue = Mathf.FloorToInt(visibleRadius);
            var tilemap = Location.Space.Tilemap;
            var minX = Mathf.Max(position.x - floorValue, 0);
            var maxX = Mathf.Min(position.x + floorValue, tilemap.Width - 1);
            var minY = Mathf.Max(position.y - floorValue, 0);
            var maxY = Mathf.Min(position.y + floorValue, tilemap.Height - 1);
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    var sqrDistance = new Vector2(x - position.x, y - position.y).sqrMagnitude;
                    if (sqrDistance >= sqrVisibleRadius) continue;

                    var point = new Vector2Int(x, y);
                    var floorTile = Location.Space.Tilemap.Get(point, RogueTileLayer.Floor);
                    var tile = Location.Space.Tilemap.Get(point, RogueTileLayer.Building);
                    SetTile(floorTile, tile, x, y);
                }
            }
        }

        private void SetView(Vector2Int position, RogueSpace space)
        {
            if (space.TryGetRoomView(position, out var room, out var roomObjs))
            {
                for (int y = room.yMin; y < room.yMax; y++)
                {
                    for (int x = room.xMin; x < room.xMax; x++)
                    {
                        var index = new Vector2Int(x, y);
                        var floorTile = space.Tilemap.Get(index, RogueTileLayer.Floor);
                        if (TryGetTileObj(roomObjs, index, out var obj))
                        {
                            SetTile(floorTile, obj, x, y);
                        }
                        else
                        {
                            // 上にタイル化されたオブジェクトが重なっていない時に限り、タイルを設定する。
                            var tile = space.Tilemap.Get(index, RogueTileLayer.Building);
                            SetTile(floorTile, tile, x, y);
                        }
                    }
                }

                for (int j = 0; j < roomObjs.Count; j++)
                {
                    var obj = roomObjs[j];
                    if (obj == null || !obj.AsTile) continue;

                    var floorTile = space.Tilemap.Get(obj.Position, RogueTileLayer.Floor);
                    SetTile(floorTile, obj, obj.Position.x, obj.Position.y);
                }
            }

            bool TryGetTileObj(Spanning<RogueObj> roomObjs, Vector2Int position, out RogueObj obj)
            {
                for (int i = 0; i < roomObjs.Count; i++)
                {
                    obj = roomObjs[i];
                    if (obj == null || !obj.AsTile || obj.Position != position) continue;

                    return true;
                }
                obj = default;
                return false;
            }
        }

        public void ResetVisibles(RogueObj location)
        {
            if (location == null) return;

            // 別の空間に移動したとき、地図をリセットする。
            if (location != Location)
            {
                for (int y = 0; y < visibles.Length; y++)
                {
                    for (int x = 0; x < visibles[0].Length; x++)
                    {
                        floorTilemap[y][x] = null;
                        buildingTilemap[y][x] = null;
                        tilemapObjs[y][x] = null;
                    }
                }
                VisibleObjs.Clear();
                Location = location;
            }

            // visible をリセット
            for (int y = 0; y < visibles.Length; y++)
            {
                var visiblesRow = visibles[y];
                for (int x = 0; x < visiblesRow.Length; x++)
                {
                    visiblesRow[x] = false;
                }
            }

            // キャラクター（当たり判定のあるオブジェクト）が視界から外れたとき、非表示にする。
            // また、空間が別になったときも非表示にする。
            for (int i = VisibleObjs.Count - 1; i >= 0; i--)
            {
                var visibleObj = VisibleObjs[i];
                if (visibleObj.HasCollider || visibleObj.Location != Location) VisibleObjs.RemoveAt(i);
            }
        }

        public void GetTile(Vector2Int position, out bool visible, out IRogueTile floorTile, out IRogueTile buildingTile, out RogueObj tileObj)
        {
            if (Location == null || Location.Space.Tilemap == null || !Location.Space.Tilemap.Rect.Contains(position))
            {
                visible = false;
                floorTile = null;
                buildingTile = null;
                tileObj = null;
                return;
            }

            visible = visibles[position.y][position.x];
            floorTile = floorTilemap[position.y][position.x];
            buildingTile = buildingTilemap[position.y][position.x];
            tileObj = tilemapObjs[position.y][position.x];
        }

        private void SetTile(IRogueTile floorTile, IRogueTile buildingTile, int x, int y)
        {
            visibles[y][x] = true;
            floorTilemap[y][x] = floorTile;
            buildingTilemap[y][x] = buildingTile;
            tilemapObjs[y][x] = null;
        }

        private void SetTile(IRogueTile floorTile, RogueObj tileObj, int x, int y)
        {
            visibles[y][x] = true;
            floorTilemap[y][x] = floorTile;
            buildingTilemap[y][x] = null;
            tilemapObjs[y][x] = tileObj;
        }
    }
}
