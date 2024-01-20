using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class ModelsMenuViewItemButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private TMP_Text _stackedText = null;
        [SerializeField] private TMP_Text _stackText = null;
        [SerializeField] private TMP_Text _weightText = null;
        [SerializeField] private TMP_Text _equippedText = null;

        private ModelsMenuView view;

        private IModelsMenuItemController controller;

        private object source;

        public RectTransform RectTransform { get; private set; }

        public CanvasGroup CanvasGroup => _canvasGroup;

        private static readonly RogueNameBuilder nameBuilder = new RogueNameBuilder();

        private const float iconWidth = 130f;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(ModelsMenuView view)
        {
            this.view = view;
        }

        public void SetItem(IModelsMenuItemController controller, object source)
        {
            this.controller = controller;
            this.source = source;
            if (controller is IPartyMenuItemController partyController)
            {
                var obj = (RogueObj)source;
                SetIcon(obj);
                SetName(_text, obj);
                _stackedText.text = null;
                _stackText.text = null;
                _weightText.text = null;
                _equippedText.text = null;
            }
            else if (controller is ISkillMenuItemController skillController)
            {
                var obj = skillController.Obj;
                if (obj == null) return;

                var skill = (ISkill)source;
                _icon.enabled = skill.Icon != null;
                _icon.sprite = skill.Icon;
                SetName(_stackedText, skill, obj);
                var requiredMP = StatsEffectedValues.GetRequiredMP(obj, skill.RequiredMP);
                _weightText.SetText("{0} MP", requiredMP);
                _weightText.fontSize = 96;
                _text.text = null;
                _stackText.text = null;
                _equippedText.text = null;
            }
            else
            {
                if (source is RogueObj obj)
                {
                    if (obj.Stack == 1)
                    {
                        SetIcon(obj);
                        SetName(_text, obj);
                        _stackedText.text = null;
                        _stackText.text = null;
                    }
                    else
                    {
                        SetIcon(obj);
                        _text.text = null;
                        SetName(_stackedText, obj);
                        _stackText.SetText("x{0}", obj.Stack);
                    }
                    {
                        var weight = WeightCalculator.Get(obj);
                        _weightText.SetText(string.Format("重:{0:0.##}", weight.TotalWeight));
                        _weightText.fontSize = 48;
                    }
                    var equipmentInfo = obj.Main.GetEquipmentInfo(obj);
                    var vehicleInfo = VehicleInfo.Get(obj);
                    var equipeed = equipmentInfo?.EquipIndex >= 0 || vehicleInfo?.Rider != null;
                    _equippedText.text = equipeed ? "E" : null;
                    _equippedText.color = Color.yellow;
                }
                else
                {
                    var text = controller.GetName(source, view.Root, view.Self, view.User, view.Arg);
                    text = StandardRogueDeviceUtility.Localize(text);
                    _text.text = text;
                    _text.color = new Color32(240, 240, 240, 255);
                    if (_stackedText != null)
                    {
                        _icon.enabled = false;
                        _stackedText.text = null;
                        _stackText.text = null;
                        _weightText.text = null;
                        _equippedText.text = null;
                    }
                    if (source is RogueFile file)
                    {
                        _weightText.text = file.LastModified.ToString();
                        _weightText.fontSize = 48;
                    }
                }
            }
        }

        private void SetIcon(RogueObj obj)
        {
            if (obj.Main.InfoSet.Icon == null)
            {
                _icon.enabled = false;
                return;
            }

            _icon.enabled = true;
            _icon.sprite = obj.Main.InfoSet.Icon;
            _icon.color = RogueColorUtility.GetColor(obj);
            _icon.SetNativeSize();

            // アイコンを枠内に収める
            var rectTransform = _icon.rectTransform;
            var rectWidth = Mathf.Max(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
            rectTransform.sizeDelta *= iconWidth / Mathf.Max(RoguegardSettings.PixelsPerUnit, rectWidth);
        }

        private void SetName(TMP_Text text, RogueObj obj)
        {
            obj.GetName(nameBuilder);
            StandardRogueDeviceUtility.Localize(nameBuilder);
            text.text = nameBuilder.ToString();
            text.color = Color.white;
        }

        private void SetName(TMP_Text text, ISkill skill, RogueObj obj)
        {
            SkillNameEffectStateInfo.GetEffectedName(nameBuilder, obj, skill);
            StandardRogueDeviceUtility.Localize(nameBuilder);
            text.text = nameBuilder.ToString();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            controller.Activate(source, view.Root, view.Self, view.User, view.Arg);
        }
    }
}
