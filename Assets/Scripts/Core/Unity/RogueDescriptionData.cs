using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class RogueDescriptionData : ScriptableObject, IRogueDescription
    {
        [SerializeField] private string _descriptionName = null;
        protected string DescriptionNameSource => _descriptionName;
        [System.NonSerialized] private string _nameCache; // null ‚É‚·‚é‚½‚ß NonSerialized ‚ðÝ’è‚·‚é
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
        public Sprite Icon { get => _icon; set => _icon = value; }

        [SerializeField] private Color _color = Color.white;
        public Color Color { get => _color; set => _color = value; }

        [SerializeField] private string _caption = null;
        public string Caption { get => _caption; set => _caption = value; }

        [SerializeField] private ScriptField<IRogueDetails> _details = null;
        public IRogueDetails Details => _details.Ref;
        private ScriptField<IRogueDetails> DetailsSource { get => _details; set => _details = value; }
    }
}
