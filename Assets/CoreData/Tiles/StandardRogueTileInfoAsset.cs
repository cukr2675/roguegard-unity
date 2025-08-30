using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Tiles/Tile Info")]
    [Objforming.Referable]
    public class StandardRogueTileInfoAsset : RogueTileInfoAsset
    {
        [SerializeField] private string _descriptionName = null;
        [System.NonSerialized] private string _nameCache; // null にするため NonSerialized を設定する
        protected override string DescriptionName => _nameCache ??= (string.IsNullOrWhiteSpace(_descriptionName) ? $":{name}" : _descriptionName);

        [SerializeField] private Sprite _icon = null;
        public override Sprite Icon => _icon;

        [SerializeField] private TileBase _tile = null;

        [SerializeField] private Color _color = Color.white;
        public override Color Color => _color;

        [SerializeField] private string _caption = null;
        public override string Caption => _caption;

        [SerializeField] private ScriptRef<IRogueDetails> _details = null;
        public override IRogueDetails Details => _details.Ref;

        [SerializeField] private KeywordAsset[] _tags;
        public override Spanning<IKeyword> Tags => _tags;



        [SerializeField] private KeywordAsset _category = null;
        public override IKeyword Category => _category;



        [SerializeField] private RogueTileLayer _layer = RogueTileLayer.Ground;
        public override RogueTileLayer Layer => _layer;
        
        [SerializeField] private bool _hasCollider = false;
        public override bool HasCollider => _hasCollider;

        [SerializeField] private bool _hasSightCollider = false;
        public override bool HasSightCollider => _hasSightCollider;



        [SerializeField] private ScriptRef<IAffectRogueMethod> _hit = null;
        public override IAffectRogueMethod Hit => _hit.Ref;

        [SerializeField] private ScriptRef<IAffectRogueMethod> _beDefeated = null;
        public override IAffectRogueMethod BeDefeated => _beDefeated.Ref;

        [SerializeField] private ScriptRef<IApplyRogueMethod> _beSteppedOnAsTile = null;
        public override IApplyRogueMethod BeSteppedOnAsTile => _beSteppedOnAsTile.Ref;



        private TileObject _tileObject;
        public override TileBase Tile
        {
            get
            {
                if (_tile != null) return _tile;
                if (_tileObject == null)
                {
                    _tileObject = CreateInstance<TileObject>();
                    _tileObject.sprite = Icon;
                    _tileObject.color = EffectedColor;
                }
                return _tileObject;
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
