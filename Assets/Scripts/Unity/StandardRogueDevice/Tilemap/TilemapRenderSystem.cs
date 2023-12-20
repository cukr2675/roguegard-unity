using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using Roguegard;

namespace RoguegardUnity
{
    internal class TilemapRenderSystem
    {
        private RogueTilemapRenderer tilemapGrid;

        public void Open(RogueTilemapRenderer tilemapGrid)
        {
            var name = "TilemapRenderSystem";
            this.tilemapGrid = tilemapGrid;
            tilemapGrid.name = $"{name} - Grid";
        }

        public void Update(RogueObj player, bool openGrid)
        {
            tilemapGrid.SetGridIsEnabled(openGrid);

            var location = player.Location;
            if (location == null || location.Space.Tilemap == null) return;

            var origin = player.Position;
            var direction = player.Main.Stats.Direction;
            IRogueTilemapView tilemap;
            if (player.TryGet<ViewInfo>(out var view)) { tilemap = view; }
            else { tilemap = location.Space; }

            for (int y = 0; y < tilemap.Size.y; y++)
            {
                for (int x = 0; x < tilemap.Size.x; x++)
                {
                    var position = new Vector3Int(x, y, 0);
                    tilemap.GetTile(new Vector2Int(x, y), out var visible, out var tile, out var tileObj);
                    if (tileObj != null)
                    {
                        tileObj.Main.Sprite.Update(tileObj);
                        tilemapGrid.Tilemap.SetTile(position, tileObj.Main.Sprite.Sprite);
                        tilemapGrid.Tilemap.SetColor(position, tileObj.Main.Sprite.Sprite.IconColor);
                    }
                    else if (tile != null)
                    {
                        if (tile.Tile != tilemapGrid.Tilemap.GetTile(position))
                        {
                            tilemapGrid.Tilemap.SetTile(position, null);
                            tilemapGrid.Tilemap.SetTile(position, tile.Tile);
                        }
                    }
                    else
                    {
                        tilemapGrid.Tilemap.SetTile(position, null);
                    }

                    var visibleColor = visible ? Color.clear : new Color(0f, 0f, 0f, .75f);
                    tilemapGrid.VisibleTilemap.SetTile(position, InvisibleTile.Instance);
                    tilemapGrid.VisibleTilemap.SetColor(position, visibleColor);

                    // マス目を表示する。
                    var showGrid = ShowGrid();
                    if (showGrid)
                    {
                        var position2 = new Vector2Int(position.x, position.y);
                        var color = Any(origin, direction, position2) ? new Color(1f, 0f, 0f, .5f) : new Color(1f, 1f, 0f, .5f);
                        tilemapGrid.GridTilemap.SetTile(position, tilemapGrid.GridTile);
                        tilemapGrid.GridTilemap.SetColor(position, color);
                    }
                    else
                    {
                        tilemapGrid.GridTilemap.SetTile(position, null);
                    }

                    bool ShowGrid()
                    {
                        // 見えない範囲や壁には表示しない。
                        if (!openGrid || !visible) return false;
                        if (tileObj != null && tileObj.HasCollider) return false;
                        if (tile != null && tile.Info.HasCollider) return false;

                        var position2 = new Vector2Int(position.x, position.y);
                        var deltaPosition2 = position2 - origin;
                        if (Mathf.Abs(deltaPosition2.x) > 10 || Mathf.Abs(deltaPosition2.y) > 10) return false;

                        return true;
                    }
                }
            }

            static bool Any(Vector2 origin, RogueDirection direction, Vector2Int position)
            {
                for (int i = 0; i < 11; i++)
                {
                    if ((origin + direction.Forward * i) == position) return true;
                }
                return false;
            }
        }

        private class InvisibleTile : TileBase
        {
            public static InvisibleTile Instance { get; } = CreateInstance<InvisibleTile>();

            private static readonly Sprite white;

            static InvisibleTile()
            {
                var texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.white);
                white = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(.5f, .5f), 1f);
            }

            public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
            {
                tileData.sprite = white;
                //tileData.color = new Color(0f, 0f, 0f, .75f);
            }
        }
    }
}
