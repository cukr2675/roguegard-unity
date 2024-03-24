using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Objforming;
using Objforming.Unity.RuntimeInspector;

namespace Roguegard.Objforming.RuntimeInspector
{
    public class RogueObjListItemElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private Button _spaceButton = null;
        [SerializeField] private Button _detailsButton = null;

        private static readonly RogueNameBuilder nameBuilder = new RogueNameBuilder();

        public void Initialize(FormInspector inspector, RogueObj value, Former rogueSpaceFormer)
        {
            value.GetName(nameBuilder);
            _text.text = nameBuilder.ToString();
            var listMember = rogueSpaceFormer.GetMemberByCamel("objs");
            _spaceButton.onClick.AddListener(() =>
            {
                if (value.Space.Objs.Count == 0)
                {
                    inspector.SetTarget(value);
                }
                else
                {
                    inspector.SetTarget((RogueObjList)listMember.GetValue(value.Space));
                }
            });
            _detailsButton.onClick.AddListener(() => inspector.SetTarget(value));
        }
    }
}
