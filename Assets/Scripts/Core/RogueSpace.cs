using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueSpace : IRogueTilemapView
    {
        private readonly RogueObjList _objs;

        /// <summary>
        /// 空間移動により null が含まれる可能性があるため、要素の null チェック必須。
        /// </summary>
        public Spanning<RogueObj> Objs => _objs.Span;

        public RogueTilemap Tilemap { get; private set; }

        private RogueObj[][] colliderMap;
        private RogueObj[][] asTileColliderMap;

        private RectInt[] rooms;
        private RogueObjList[] roomObjs;
        public int RoomCount => rooms?.Length ?? 0;

        [System.NonSerialized] private RogueSpaceRandom _spaceRandom;
        private RogueSpaceRandom SpaceRandom => _spaceRandom ??= new RogueSpaceRandom(this);

        Vector2Int IRogueTilemapView.Size => Tilemap?.Rect.size ?? Vector2Int.zero;
        Spanning<RogueObj> IRogueTilemapView.VisibleObjs => _objs.Span;

        private static readonly RectInt[] noRooms = new RectInt[0];
        private static readonly RogueObjList[] noRoomObjs = new RogueObjList[0];
        private const bool cantViewRoomFromSide = true;

        [Objforming.CreateInstance, SuppressMessage("Style", "IDE0051")]
        private RogueSpace(bool _) { }

        internal RogueSpace()
        {
            _objs = new RogueObjList();
            rooms = noRooms;
            roomObjs = noRoomObjs;
        }

        internal RogueSpace(RogueSpace space)
        {
            _objs = new RogueObjList();
            if (space.Tilemap != null) { Tilemap = new RogueTilemap(space.Tilemap); }
            rooms = space.rooms;
            roomObjs = new RogueObjList[space.rooms.Length];
        }

        public RogueObj GetColliderObj(Vector2Int position)
        {
            if (Tilemap == null) throw new RogueException($"この空間はタイルマップを持ちません。");
            if (!Tilemap.Rect.Contains(position)) return null;

            return colliderMap[position.y][position.x];
        }

        public void SetTilemap(RogueTilemap tilemap)
        {
            Tilemap = tilemap;

            // colliderMap の初期化
            colliderMap = new RogueObj[tilemap.Height][];
            asTileColliderMap = new RogueObj[tilemap.Height][];
            for (int y = 0; y < tilemap.Height; y++)
            {
                colliderMap[y] = new RogueObj[tilemap.Width];
                asTileColliderMap[y] = new RogueObj[tilemap.Width];
            }

            // colliderMap を現在のオブジェクトで設定する。
            foreach (var obj in _objs.Span)
            {
                if (obj == null || !obj.HasCollider) continue;

                var position = obj.Position;
                if (obj.AsTile)
                {
                    if (asTileColliderMap[position.y][position.x] != null)
                        throw new RogueException("当たり判定のあるオブジェクトが重なっています。");

                    asTileColliderMap[position.y][position.x] = obj;
                }
                else
                {
                    if (colliderMap[position.y][position.x] != null)
                        throw new RogueException("当たり判定のあるオブジェクトが重なっています。");

                    colliderMap[position.y][position.x] = obj;
                }
            }
        }

        public void SetRooms(Spanning<RectInt> rooms)
        {
            this.rooms = new RectInt[rooms.Count];
            roomObjs = new RogueObjList[this.rooms.Length];
            for (int i = 0; i < this.rooms.Length; i++)
            {
                var room = rooms[i];
                if (room.width < 2 || room.height < 2) throw new System.ArgumentException("部屋のサイズは 2x2 以上である必要があります。");

                this.rooms[i] = room;
                roomObjs[i] = new RogueObjList();
            }

            SpaceRandom.Reset(this);
        }

        /// <summary>
        /// <paramref name="roomObjs"/> には null が含まれる可能性があるため、要素の null チェック必須。
        /// </summary>
        public void GetRoom(int roomIndex, out RectInt room, out Spanning<RogueObj> roomObjs)
        {
            room = rooms[roomIndex];
            roomObjs = this.roomObjs[roomIndex].Span;
        }

        /// <summary>
        /// <paramref name="position"/> から見た部屋情報を取得する。
        /// <see cref="cantViewRoomFromSide"/> == true のとき、部屋の端にいるときは部屋情報を取得できない。
        /// <paramref name="roomObjs"/> には null が含まれる可能性があるため、要素の null チェック必須。
        /// </summary>
        public bool TryGetRoomView(Vector2Int position, out RectInt room, out Spanning<RogueObj> roomObjs)
        {
            for (int i = 0; i < rooms.Length; i++)
            {
                var rect = rooms[i];
                var includeRect = cantViewRoomFromSide ? new RectInt(rect.xMin + 1, rect.yMin + 1, rect.width - 2, rect.height - 2) : rect;
                if (!includeRect.Contains(position)) continue;

                room = rect;
                roomObjs = this.roomObjs[i].Span;
                return true;
            }
            room = default;
            roomObjs = default;
            return false;
        }

        private void UpdateRoom(RogueObj obj, Vector2Int position)
        {
            var yet = true; // 一つのオブジェクトは一つの部屋のみに属する。
            for (int i = 0; i < rooms.Length; i++)
            {
                if (yet && rooms[i].Contains(position))
                {
                    roomObjs[i].TryAddUnique(obj);
                    yet = false;
                }
                else
                {
                    roomObjs[i].Remove(obj);
                }
            }
        }

        public bool TryGetRandomPositionInRoom(IRogueRandom random, out Vector2Int position)
        {
            if (Tilemap == null) throw new RogueException($"{Tilemap} の設定されていない空間からランダム位置を取得することはできません。");

            return SpaceRandom.TryGetRandomPositionInRoom(this, random, out position);
        }

        public bool TryGetRandomPositionInRoom(IRogueRandom random, int roomIndex, out Vector2Int position)
        {
            if (Tilemap == null) throw new RogueException($"{Tilemap} の設定されていない空間からランダム位置を取得することはできません。");

            return SpaceRandom.GetRandomPositionInRoom(this, random, roomIndex, out position);
        }

        public bool CollideAt(Vector2Int position, bool collide = true, bool tileCollide = true)
        {
            // タイルマップを持たない空間に移動する場合、 (0, 0) に限定する。
            if (Tilemap == null) return position != Vector2Int.zero;

            if (!Tilemap.Rect.Contains(position))
            {
                // タイルマップ範囲外と衝突
                return true;
            }
            else if (collide && colliderMap[position.y][position.x] != null)
            {
                // オブジェクト同士の衝突
                return true;
            }
            else if (tileCollide && (asTileColliderMap[position.y][position.x] != null || Tilemap.GetTop(position).Info.HasCollider))
            {
                // タイルオブジェクトまたはタイルと衝突
                return true;
            }
            else
            {
                // 衝突なし
                return false;
            }
        }

        /// <summary>
        /// <see cref="RogueObj.TryLocate(RogueObj, Vector2Int, bool, bool, bool, bool, StackOption)"/> 以外では実行しない。
        /// </summary>
        internal bool TryLocate(RogueObj obj, Vector2Int position, bool asTile, bool collide, bool tileCollide)
        {
            // 衝突したら移動失敗。
            var collision = CollideAt(position, collide, tileCollide);
            if (collision) return false;

            if (obj.Location?.Space != this)
            {
                // 同じ空間でなければ、空間を移動させる。
                obj.Location?.Space.ReplaceWithNull(obj);
                _objs.Add(obj);
            }
            else
            {
                // 同じ空間なら当たり判定マップを修正するだけ。
                if (colliderMap != null && obj.HasCollider)
                {
                    var oldPosition = obj.Position;
                    if (obj.AsTile)
                    {
                        asTileColliderMap[oldPosition.y][oldPosition.x] = null;
                    }
                    else
                    {
                        colliderMap[oldPosition.y][oldPosition.x] = null;
                    }
                    SpaceRandom.AddRandomPosition(this, oldPosition);
                }
            }

            if (collide && Tilemap != null)
            {
                SpaceRandom.RemoveRandomPosition(this, position);
                if (asTile)
                {
                    asTileColliderMap[position.y][position.x] = obj;
                }
                else
                {
                    colliderMap[position.y][position.x] = obj;
                }
            }
            UpdateRoom(obj, position);

            return true;
        }

        /// <summary>
        /// 空間外へ移動したオブジェクトをリストから即削除すると <see cref="IRogueObjUpdater"/> 実行対象のインデックスがずれるため、
        /// null に置き換える。
        /// </summary>
        internal void ReplaceWithNull(RogueObj obj)
        {
            var index = _objs.IndexOf(obj);
            if (index == -1) throw new RogueException();

            _objs[index] = null;

            if (obj.HasCollider && Tilemap != null)
            {
                var oldPosition = obj.Position;
                if (obj.AsTile)
                {
                    asTileColliderMap[oldPosition.y][oldPosition.x] = null;
                }
                else
                {
                    colliderMap[oldPosition.y][oldPosition.x] = null;
                }
                SpaceRandom.AddRandomPosition(this, oldPosition);
            }

            if (roomObjs == null)
            {
                rooms = noRooms;
                roomObjs = noRoomObjs;
            }

            foreach (var objs in roomObjs)
            {
                var roomObjIndex = objs.IndexOf(obj);
                if (roomObjIndex == -1) continue;

                objs[roomObjIndex] = null;
            }
        }

        public void RemoveAllNull()
        {
            for (int i = _objs.Count - 1; i >= 0; i--)
            {
                if (_objs[i] == null) _objs.RemoveAt(i);
            }

            if (roomObjs == null)
            {
                rooms = noRooms;
                roomObjs = noRoomObjs;
            }

            foreach (var objs in roomObjs)
            {
                for (int i = objs.Count - 1; i >= 0; i--)
                {
                    if (objs[i] == null) objs.RemoveAt(i);
                }
            }
        }

        public bool Contains(RogueObj obj)
        {
            return _objs.Contains(obj);
        }

        public bool Stack(RogueObj obj, Vector2Int position, int maxStack)
        {
            return _objs.Stack(obj, position, maxStack);
        }

        /// <summary>
        /// 指定位置+レイヤーのタイル設定可否を取得する。タイル同士の衝突に <see cref="IRogueTileInfo.HasCollider"/> は影響しないので注意。
        /// </summary>
        /// <param name="overwrite">true のとき指定位置+レイヤーにタイルが存在しても衝突しない</param>
        /// <param name="bury">true のとき指定位置+レイヤーが埋まっていても、上層のタイルやオブジェクトと衝突しない</param>
        public bool TileCollideAt(Vector2Int position, RogueTileLayer layer, bool collide, bool overwrite = false, bool bury = false)
        {
            // タイルマップ範囲外に衝突
            if (Tilemap == null || CollideAt(position, false, false)) return true;

            // すでに敷かれているタイルを上書きしない
            if (!overwrite && Tilemap.Get(position, layer) != null) return true; // 指定位置+レイヤーの既存タイルと衝突

            // すでに敷かれているタイルの下に潜り込ませない
            if (!bury)
            {
                var topTile = Tilemap.GetTop(position);
                if (topTile.Info.Layer > layer) return true; // 指定位置+レイヤーが埋まっているため上層のタイルと衝突

                foreach (var obj in _objs.Span)
                {
                    if (obj == null || obj.Position != position) continue;

                    if (obj.AsTile) return true; // 指定位置が埋まっているため上層のタイルオブジェクトと衝突

                    if (obj.HasTileCollider && collide) return true; // 指定位置がオブジェクトに乗られているため衝突
                    // 設計メモ: bury = true で壁タイルの下に埋められるならオブジェクトが重なっている壁タイルの下にも埋められるべきなので、
                    // オブジェクト衝突は bury の影響を受ける
                    // ゲーム表示上ではオブジェクトが壁タイルに埋まっているとする場合でも、内部処理的には上からObj→BuildingTile→GroundTileとしたほうがスムーズ
                }
            }

            return false;
        }

        /// <param name="overwrite">true のとき指定位置+レイヤーにタイルが存在しても衝突しない</param>
        /// <param name="bury">true のとき指定位置+レイヤーが埋まっていても、上層のタイルやオブジェクトと衝突しない</param>
        public bool TrySet(IRogueTile tile, Vector2Int position, bool overwrite = false, bool bury = false)
        {
            if (tile == null) throw new System.ArgumentNullException(nameof(tile));
            if (TileCollideAt(position, tile.Info.Layer, tile.Info.HasCollider, overwrite, bury)) return false;

            //var topTile = Tilemap.GetTop(position);
            //if (tile.Info.HasCollider || tile.Info.Layer != RogueTileLayer.Ground)
            //{
            //    SpaceRandom.RemoveRandomPosition(this, position);
            //}
            Tilemap.Set(tile, position);
            //if (topTile.Info.HasCollider || topTile.Info.Layer != RogueTileLayer.Ground)
            //{
            //    SpaceRandom.AddRandomPosition(this, position);
            //}
            SpaceRandom.Reset(this);
            return true;
        }

        public bool TryRemove(Vector2Int position, RogueTileLayer layer, bool bury = false)
        {
            if (TileCollideAt(position, layer, false, true, bury)) return false;

            //var topTile = Tilemap.GetTop(position);
            Tilemap.Remove(position, layer);
            //if (topTile.Info.HasCollider || topTile.Info.Layer != RogueTileLayer.Ground)
            //{
            //    SpaceRandom.AddRandomPosition(this, position);
            //}
            SpaceRandom.Reset(this);
            return true;
        }

        public void Sort(Spanning<RogueObj> sorted)
        {
            _objs.Sort(sorted);
        }

        void IRogueTilemapView.GetTile(Vector2Int position, out bool visible, out IRogueTile groundTile, out IRogueTile buildingTile, out RogueObj tileObj)
        {
            if (Tilemap == null || !Tilemap.Rect.Contains(position))
            {
                visible = false;
                groundTile = null;
                buildingTile = null;
                tileObj = null;
                return;
            }

            visible = true;
            groundTile = Tilemap.Get(position, RogueTileLayer.Ground);
            buildingTile = Tilemap.Get(position, RogueTileLayer.Building);
            tileObj = asTileColliderMap[position.y][position.x];

            // 当たり判定のないタイルのオブジェクトを取得する（階段など）
            for (int i = 0; i < _objs.Count; i++)
            {
                var obj = _objs[i];
                if (obj == null || !obj.AsTile || obj.Position != position) continue;

                tileObj = obj;
                break;
            }
        }
    }
}
