using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueTilemap
    {
        private readonly IRogueTile[][][] tilemap;

        public int Width => tilemap.Length == 0 ? 0 : tilemap[0].Length;

        public int Height => tilemap.Length;

        public RectInt Rect => new RectInt(0, 0, Width, Height);

        private const int depth = 2;

        [Objforming.CreateInstance]
        private RogueTilemap() { }

        public RogueTilemap(Vector2Int size)
        {
            tilemap = new IRogueTile[size.y][][];
            for (int y = 0; y < size.y; y++)
            {
                var tilemapRow = new IRogueTile[size.x][];
                for (int x = 0; x < size.x; x++)
                {
                    tilemapRow[x] = new IRogueTile[depth];
                }
                tilemap[y] = tilemapRow;
            }
        }

        public RogueTilemap(RogueTilemap tilemap)
            : this(tilemap.Rect.size)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        this.tilemap[y][x][z] = tilemap.tilemap[y][x][z];
                    }
                }
            }
        }

        public IRogueTile Get(Vector2Int index, RogueTileLayer layer)
        {
            if (!Rect.Contains(index)) return null;

            return tilemap[index.y][index.x][(int)layer];
        }

        public IRogueTile GetTop(Vector2Int index)
        {
            if (!Rect.Contains(index)) return null;

            var tiles = tilemap[index.y][index.x];
            for (int z = depth - 1; z >= 0; z--)
            {
                if (tiles[z] != null) return tiles[z];
            }
            return null;
        }

        public void Set(IRogueTile tile, int x, int y)
        {
            if (tile == null) throw new System.ArgumentNullException(nameof(tile));

            tilemap[y][x][(int)tile.Info.Layer] = tile;
        }

        public void Set(IRogueTile tile, Vector2Int index)
        {
            Set(tile, index.x, index.y);
        }

        public void Remove(Vector2Int index, RogueTileLayer layer)
        {
            tilemap[index.y][index.x][(int)layer] = null;
        }

        public void Replace(Spanning<IRogueTile> tiles, int x, int y)
        {
            if (y < 0 || Height <= y || x < 0 || Width <= x) throw new System.ArgumentOutOfRangeException($"x: {x}, y: {y}");

            for (int z = 0; z < depth; z++)
            {
                tilemap[y][x][z] = null;
            }
            for (int i = 0; i < tiles.Count; i++)
            {
                Set(tiles[i], x, y);
            }
        }
    }
}
