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

        private static readonly Vector3Int[] deltaPositions = new[]
        {
            new Vector3Int(-1, -1),
            new Vector3Int(+0, -1),
            new Vector3Int(+1, -1),
            new Vector3Int(-1, +0),
            new Vector3Int(+1, +0),
            new Vector3Int(-1, +1),
            new Vector3Int(+0, +1),
            new Vector3Int(+1, +1),
        };

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
            if (ViewInfo.TryGet(player, out var view)) { tilemap = view; }
            else { tilemap = location.Space; }

            for (int y = 0; y < tilemap.Size.y; y++)
            {
                for (int x = 0; x < tilemap.Size.x; x++)
                {
                    var position = new Vector3Int(x, y, (int)RogueTileLayer.Building);
                    tilemap.GetTile(new Vector2Int(x, y), out var visible, out var floorTile, out var buildingTile, out var tileObj);
                    if (tileObj != null)
                    {
                        tileObj.Main.Sprite.Update(tileObj);
                        var sprite = tileObj.Main.Sprite.Sprite;
                        SetTile(tilemapGrid.Tilemap, position, sprite.Tile, sprite.EffectedColor);
                    }
                    else if (buildingTile != null)
                    {
                        SetTile(tilemapGrid.Tilemap, position, buildingTile.Tile, buildingTile.EffectedColor);
                    }
                    else if (floorTile != null)
                    {
                        SetTile(tilemapGrid.Tilemap, position, InvisibleTile.Instance, Color.clear);
                    }
                    else
                    {
                        SetTile(tilemapGrid.Tilemap, position, null, Color.clear);
                    }

                    var floorPosition = new Vector3Int(x, y, (int)RogueTileLayer.Floor);
                    if (floorTile != null)
                    {
                        tilemapGrid.Tilemap.SetTile(floorPosition, floorTile.Tile);
                        tilemapGrid.Tilemap.SetColor(floorPosition, floorTile.Info.Color);
                    }
                    else
                    {
                        tilemapGrid.Tilemap.SetTile(floorPosition, null);
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
                        var topTile = buildingTile ?? floorTile;
                        if (topTile != null && topTile.Info.HasCollider) return false;

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

            static void SetTile(Tilemap tilemap, Vector3Int position, TileBase tile, Color color)
            {
                var oldTile = tilemap.GetTile(position);
                if (tile != oldTile)
                {
                    tilemap.SetTile(position, tile);
                    foreach (var delta in deltaPositions)
                    {
                        // タイルが変更されたとき、周囲8マスを更新する
                        tilemap.RefreshTile(position + delta);
                    }
                }

                tilemap.SetColor(position, color);
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
