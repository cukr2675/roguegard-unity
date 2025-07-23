using UnityEngine;

namespace Roguegard
{
    // 命名メモ: RogueDescriptionData だと非 ScriptableObject のデータと名の付くクラスと区別しづらいため RogueDescriptionAsset
    // Asset とはいっても画像や音声のようなリソースを持つわけではない
    public abstract class RogueDescriptionAsset : ScriptableObject, IRogueDescription
    {
        [SerializeField] private string _descriptionName = null;
        protected string DescriptionNameSource => _descriptionName;
        [System.NonSerialized] private string _nameCache; // null にするため NonSerialized を設定する
        public virtual string DescriptionName
        {
            get
            {
                return _nameCache ??= (string.IsNullOrWhiteSpace(_descriptionName) ? $":{name}" : _descriptionName);
            }
            set
            {
                _descriptionName = value;
                _nameCache = null;
            }
        }
        string IRogueDescription.Name => DescriptionName;



        [SerializeField] private Sprite _icon = null;

        /// <summary>
        /// デフォルト: null
        /// </summary>
        public Sprite Icon { get => _icon; set => _icon = value; }



        [SerializeField] private Color _color = Color.white;

        /// <summary>
        /// デフォルト: <see cref="Color.white"/>
        /// </summary>
        public Color Color { get => _color; set => _color = value; }



        [SerializeField] private string _caption = null;

        /// <summary>
        /// デフォルト: null
        /// </summary>
        public string Caption { get => _caption; set => _caption = value; }



        [SerializeField] private ScriptRef<IRogueDetails> _details = null;

        /// <summary>
        /// デフォルト: null
        /// </summary>
        public IRogueDetails Details => _details?.Ref;

        private ScriptRef<IRogueDetails> DetailsSource { get => _details; set => _details = value; }
    }
}
