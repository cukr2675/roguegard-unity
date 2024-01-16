using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class RogueSpace : IRogueTilemapView
    {
        /// <summary>
        /// 空間移動により null が含まれる可能性があるため、要素の null チェック必須。
        /// </summary>
        public Spanning<RogueObj> Objs => _objs;

        private RogueObjList _objs;

        public RogueTilemap Tilemap { get; private set; }

        private RogueObj[][] colliderMap;

        private RogueObj[][] tileColliderMap;

        private RectInt[] rooms;

        private RogueObjList[] roomObjs;

        [System.NonSerialized] private RogueSpaceRandom _spaceRandom;
        private RogueSpaceRandom spaceRandom => _spaceRandom ??= new RogueSpaceRandom(this);

        public int RoomCount => rooms?.Length ?? 0;

        Vector2Int IRogueTilemapView.Size => Tilemap?.Rect.size ?? Vector2Int.zero;

        Spanning<RogueObj> IRogueTilemapView.VisibleObjs => _objs;

        private static readonly RectInt[] empty = new RectInt[0];
        private static readonly RogueObjList buffer = new RogueObjList();

        private const bool cantViewRoomFromSide = true;

        [ObjectFormer.CreateInstance]
        private RogueSpace(bool flag) { }

        internal RogueSpace()
        {
            _objs = new RogueObjList();
            rooms = empty;
        }

        internal RogueSpace(RogueSpace space)
        {
            _objs = new RogueObjList();
            if (space.Tilemap != null) { Tilemap = new RogueTilemap(space.Tilemap); }
            rooms = space.rooms;
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
            tileColliderMap = new RogueObj[tilemap.Height][];
            for (int y = 0; y < tilemap.Height; y++)
            {
                colliderMap[y] = new RogueObj[tilemap.Width];
                tileColliderMap[y] = new RogueObj[tilemap.Width];
            }

            // colliderMap を現在のオブジェクトで設定する。
            for (int i = 0; i < _objs.Count; i++)
            {
                var obj = _objs[i];
                if (obj == null || !obj.HasCollider) continue;

                var position = obj.Position;
                if (obj.AsTile)
                {
                    if (tileColliderMap[position.y][position.x] != null)
                        throw new RogueException("当たり判定のあるオブジェクトが重なっています。");

                    tileColliderMap[position.y][position.x] = obj;
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
                if (room.width < 2 || room.height < 2) throw new System.ArgumentException("部屋のサイズは 2 以上である必要があります。");

                this.rooms[i] = room;
                roomObjs[i] = new RogueObjList();
            }

            spaceRandom.Reset(this);
        }

        /// <summary>
        /// <paramref name="roomObjs"/> には null が含まれる可能性があるため、要素の null チェック必須。
        /// </summary>
        public void GetRoom(int roomIndex, out RectInt room, out Spanning<RogueObj> roomObjs)
        {
            room = rooms[roomIndex];
            roomObjs = this.roomObjs[roomIndex];
        }

        /// <summary>
        /// <paramref name="position"/> から見た部屋情報を取得する。
        /// <see cref="cantViewRoomFromSide"/> == true のとき、部屋の端にいるときは部屋情報を取得できない。
        /// <paramref name="roomObjs"/> には null が含まれる可能性があるため、要素の null チェック必須。
        /// </summary>
        public bool TryGetRoomView(Vector2Int position, out RectInt room, out Spanning<RogueObj> roomObjs)
        {
            room = new RectInt();
            for (int i = 0; i < rooms.Length; i++)
            {
                var rect = rooms[i];
                var includeRect = cantViewRoomFromSide ? new RectInt(rect.xMin + 1, rect.yMin + 1, rect.width - 2, rect.height - 2) : rect;
                if (!includeRect.Contains(position)) continue;

                room = rect;
                roomObjs = this.roomObjs[i];
                return true;
            }
            roomObjs = default;
            return false;
        }

        private void UpdateRoom(RogueObj obj, Vector2Int position)
        {
            if (rooms == null) return;

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

        public Vector2Int GetRandomPositionInRoom(IRogueRandom random)
        {
            if (Tilemap == null) throw new RogueException($"{Tilemap} の設定されていない空間からランダム位置を取得することはできません。");

            return spaceRandom.GetRandomPositionInRoom(this, random);
        }

        public Vector2Int GetRandomPositionInRoom(IRogueRandom random, int roomIndex)
        {
            if (Tilemap == null) throw new RogueException($"{Tilemap} の設定されていない空間からランダム位置を取得することはできません。");

            return spaceRandom.GetRandomPositionInRoom(this, random, roomIndex);
        }

        public bool CollideAt(Vector2Int position, bool collide = true, bool tileCollide = true)
        {
            // タイルマップを持たない空間に移動する場合、 (0, 0) に限定する。
            if (Tilemap == null) return position != Vector2Int.zero;

            if (!Tilemap.Rect.Contains(position))
            {
                return true;
            }
            else if (collide && colliderMap[position.y][position.x] != null)
            {
                return true;
            }
            else if (tileCollide && (tileColliderMap[position.y][position.x] != null || Tilemap.GetTop(position).Info.HasCollider))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// <see cref="RogueObj.TryLocate(RogueObj, Vector2Int, bool, bool, bool)"/> 以外では実行しない。
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
                        tileColliderMap[oldPosition.y][oldPosition.x] = null;
                    }
                    else
                    {
                        colliderMap[oldPosition.y][oldPosition.x] = null;
                    }
                    spaceRandom.AddRandomPosition(this, oldPosition);
                }
            }

            if (collide && Tilemap != null)
            {
                spaceRandom.RemoveRandomPosition(this, position);
                if (asTile)
                {
                    tileColliderMap[position.y][position.x] = obj;
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
                    tileColliderMap[oldPosition.y][oldPosition.x] = null;
                }
                else
                {
                    colliderMap[oldPosition.y][oldPosition.x] = null;
                }
                spaceRandom.AddRandomPosition(this, oldPosition);
            }

            if (rooms == null) return;

            for (int i = 0; i < rooms.Length; i++)
            {
                var roomObjIndex = roomObjs[i].IndexOf(obj);
                if (roomObjIndex == -1) continue;

                roomObjs[i][roomObjIndex] = null;
            }
        }

        public void RemoveAllNull()
        {
            for (int i = _objs.Count - 1; i >= 0; i--)
            {
                if (_objs[i] == null) _objs.RemoveAt(i);
            }

            if (roomObjs == null) return;

            for (int i = 0; i < roomObjs.Length; i++)
            {
                var objs = roomObjs[i];
                for (int j = objs.Count - 1; j >= 0; j--)
                {
                    if (objs[j] == null) objs.RemoveAt(j);
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

        public bool TileCollideAt(Vector2Int position, RogueTileLayer layer, bool collide, bool overwrite = false, bool bury = false)
        {
            // タイル範囲外に衝突したら移動失敗
            if (Tilemap == null || CollideAt(position, false, false)) return true;

            // すでに敷かれているタイルを上書きしない
            if (!overwrite && Tilemap.Get(position, layer) != null) return true;

            // すでに敷かれているタイルの下に潜り込ませない
            if (!bury)
            {
                var topTile = Tilemap.GetTop(position);
                if (topTile.Info.Layer > layer) return true;

                for (int i = 0; i < _objs.Count; i++)
                {
                    var obj = _objs[i];
                    if (obj == null || obj.Position != position) continue;

                    // AsTile == true のオブジェクトの下に潜り込ませない
                    if (obj.AsTile) return true;

                    // オブジェクトを壁タイルに埋めない
                    if (obj.HasTileCollider && collide) return true;
                }
            }

            return false;
        }

        public bool TrySet(IRogueTile tile, Vector2Int position, bool overwrite = false, bool bury = false)
        {
            if (tile == null) throw new System.ArgumentNullException(nameof(tile));
            if (TileCollideAt(position, tile.Info.Layer, tile.Info.HasCollider, overwrite, bury)) return false;

            //var topTile = Tilemap.GetTop(position);
            //if (tile.Info.HasCollider || tile.Info.Layer != RogueTileLayer.Floor)
            //{
            //    spaceRandom.RemoveRandomPosition(this, position);
            //}
            Tilemap.Set(tile, position);
            //if (topTile.Info.HasCollider || topTile.Info.Layer != RogueTileLayer.Floor)
            //{
            //    spaceRandom.AddRandomPosition(this, position);
            //}
            spaceRandom.Reset(this);
            return true;
        }

        public bool TryRemove(Vector2Int position, RogueTileLayer layer, bool bury = false)
        {
            if (TileCollideAt(position, layer, false, true, bury)) return false;

            //var topTile = Tilemap.GetTop(position);
            Tilemap.Remove(position, layer);
            //if (topTile.Info.HasCollider || topTile.Info.Layer != RogueTileLayer.Floor)
            //{
            //    spaceRandom.AddRandomPosition(this, position);
            //}
            spaceRandom.Reset(this);
            return true;
        }

        public void Sort(Spanning<RogueObj> sorted)
        {
            if (sorted.Count != _objs.Count) throw new System.ArgumentException("要素数が一致しません。");

            // 例外を投げるときソートをキャンセルしたいので、一通り例外判定してからソートする
            for (int i = 0; i < sorted.Count; i++)
            {
                if (!sorted.Contains(_objs[i])) throw new System.ArgumentException($"{nameof(sorted)} が要素を網羅していません。");
            }

            _objs.Clear();
            for (int i = 0; i < sorted.Count; i++)
            {
                _objs.Add(sorted[i]);
            }
        }

        void IRogueTilemapView.GetTile(Vector2Int position, out bool visible, out IRogueTile tile, out RogueObj tileObj)
        {
            if (Tilemap == null || !Tilemap.Rect.Contains(position))
            {
                visible = false;
                tile = null;
                tileObj = null;
                return;
            }

            visible = true;
            tile = Tilemap.GetTop(position);
            tileObj = tileColliderMap[position.y][position.x];

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
