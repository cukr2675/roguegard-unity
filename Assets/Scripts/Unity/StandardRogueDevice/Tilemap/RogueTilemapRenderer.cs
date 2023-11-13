using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

namespace RoguegardUnity
{
    /// <summary>
    /// 子オブジェクトに <see cref="UnityEngine.Tilemaps.Tilemap"/> を持つコンポーネント
    /// </summary>
    public class RogueTilemapRenderer : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap = null;
        public Tilemap Tilemap => _tilemap;

        [SerializeField] private Tilemap _visibleTilemap = null;
        public Tilemap VisibleTilemap => _visibleTilemap;

        [SerializeField] private Tilemap _gridTilemap = null;
        public Tilemap GridTilemap => _gridTilemap;

        private TilemapRenderer _gridRenderer;

        [SerializeField] private TileBase _gridTile = null;
        public TileBase GridTile => _gridTile;

        public void SetGridIsEnabled(bool enabled)
        {
            _gridRenderer ??= _gridTilemap.GetComponent<TilemapRenderer>();
            _gridRenderer.enabled = enabled;
        }
    }
}
