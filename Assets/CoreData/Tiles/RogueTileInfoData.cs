using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Tile")]
    [ObjectFormer.Referable]
    public class RogueTileInfoData : ScriptableRogueTileInfo
    {
        [SerializeField] private string _descriptionName = null;
        private string _name;
        protected override string DescriptionName => _name ??= (string.IsNullOrEmpty(_descriptionName) ? $":{name}" : _descriptionName);

        [SerializeField] private Sprite _sprite = null;
        public override Sprite Sprite => _sprite;

        [SerializeField] private Color _color = Color.white;
        public override Color Color => _color;

        [SerializeField] private string _caption = null;
        public override string Caption => _caption;

        [SerializeField] private ScriptField<IRogueDetails> _details = null;
        public override IRogueDetails Details => _details.Ref;



        [SerializeField] private KeywordData _category = null;
        public override IKeyword Category => _category;



        [SerializeField] private RogueTileLayer _layer = RogueTileLayer.Floor;
        public override RogueTileLayer Layer => _layer;
        
        [SerializeField] private bool _hasCollider = false;
        public override bool HasCollider => _hasCollider;

        [SerializeField] private bool _hasSightCollider = false;
        public override bool HasSightCollider => _hasSightCollider;



        [SerializeField] private ScriptField<IAffectRogueMethod> _hit = null;
        public override IAffectRogueMethod Hit => _hit.Ref;

        [SerializeField] private ScriptField<IAffectRogueMethod> _beDefeated = null;
        public override IAffectRogueMethod BeDefeated => _beDefeated.Ref;

        [SerializeField] private ScriptField<IApplyRogueMethod> _beApplied = null;
        public override IApplyRogueMethod BeApplied => _beApplied.Ref;



        private TileObject _tile;
        public override TileBase Tile
        {
            get
            {
                if (_tile == null)
                {
                    _tile = CreateInstance<TileObject>();
                    _tile.sprite = Sprite;
                    _tile.color = Color;
                }
                return _tile;
            }
        }

        public override bool Equals(IRogueTile other)
        {
            return ReferenceEquals(other, this);
        }

        private class TileObject : TileBase
        {
            public Sprite sprite;
            public Color color;

            public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
            {
                tileData.sprite = sprite;
                tileData.color = color;
            }
        }
    }
}
