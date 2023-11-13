using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class RogueDescriptionData : ScriptableObject, IRogueDescription
    {
        [SerializeField] private string _descriptionName = null;
        public string DescriptionName { get => _descriptionName; set => _descriptionName = value; }
        string IRogueDescription.Name => _descriptionName;

        [SerializeField] private Sprite _icon = null;
        public Sprite Icon { get => _icon; set => _icon = value; }

        [SerializeField] private Color _color = Color.white;
        public Color Color { get => _color; set => _color = value; }

        [SerializeField] private string _caption = null;
        public string Caption { get => _caption; set => _caption = value; }

        [SerializeField] private ScriptField<IRogueDetails> _details = null;
        public object Details => _details.Ref;
        private ScriptField<IRogueDetails> DetailsSource { get => _details; set => _details = value; }
    }
}
