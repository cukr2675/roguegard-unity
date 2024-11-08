using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class RogueButtonViewElement : ViewElement
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private TMP_Text _stackText = null;
        [SerializeField] private TMP_Text _info1Text = null;
        [SerializeField] private TMP_Text _info2Text = null;
        [SerializeField] private TMP_Text _equipText = null;
        private float iconWidth;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";
        [Space, SerializeField] private Button.ButtonClickedEvent _onClickWithoutBlock = null;
        private Animator animator;

        private IButtonElementHandler handler;
        private object element;

        private static readonly RogueNameBuilder nameBuilder = new();

        private void Awake()
        {
            iconWidth = _icon.rectTransform.rect.width;

            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (IsBlocked) return;

                _onClickWithoutBlock.Invoke();
                handler.HandleClick(element, Manager, Arg);
            });

            TryGetComponent(out animator);
        }

        protected override void InnerSetElement(IElementHandler handler, object element)
        {
            this.handler = handler as IButtonElementHandler;
            this.element = element;

            var color = RoguegardSettings.White;
            Sprite icon = null;
            var iconColor = RoguegardSettings.White;
            int? stack = null;
            float? stars = null;
            string infoText1 = null;
            string infoText2 = null;
            bool equipeed = false;
            if (handler is IRogueElementHandler rogueElementHandler &&
                Manager is MMgr manager &&
                Arg is MArg arg)
            {
                rogueElementHandler.GetRogueInfo(
                    element, manager, arg, out var nameObj, ref color, ref icon, ref iconColor, ref stack,
                    ref stars, ref infoText1, ref infoText2, ref equipeed);
                if (nameObj is RogueObj rogueObj)
                {
                    rogueObj.GetName(nameBuilder);
                    StandardRogueDeviceUtility.Localize(nameBuilder);
                    _nameText.text = nameBuilder.ToString();
                }
                else if (nameObj is ISkill skill)
                {
                    rogueObj = arg.Arg.TargetObj ?? arg.Self;
                    SkillNameEffectStateInfo.GetEffectedName(nameBuilder, rogueObj, skill);
                    StandardRogueDeviceUtility.Localize(nameBuilder);
                    _nameText.text = nameBuilder.ToString();
                }
                else
                {
                    _nameText.text = Manager.Localize(nameObj.ToString());
                }
            }
            else
            {
                var baseText = handler.GetName(element, Manager, Arg);
                _nameText.text = Manager.Localize(baseText);
            }
            _nameText.color = color;
            SetIcon(icon, iconColor);
            if (stack != null) { _stackText.SetText("x{0}", stack.Value); }
            else { _stackText.text = ""; }
            _info1Text.text = infoText1;
            _info2Text.text = infoText2;
            _equipText.enabled = equipeed;



            if (animator != null)
            {
                var style = handler.GetStyle(element, Manager, Arg);
                if (style == null) { style = _defaultStyle; }
                for (int i = 0; i < animator.layerCount; i++)
                {
                    var weight = animator.GetLayerName(i) == style ? 1f : 0f;
                    animator.SetLayerWeight(i, weight);
                }
            }
        }

        private void SetIcon(Sprite icon, Color iconColor)
        {
            if (icon == null)
            {
                _icon.enabled = false;
                return;
            }

            _icon.sprite = icon;
            _icon.color = iconColor;
            _icon.enabled = true;
            _icon.SetNativeSize();

            // ƒAƒCƒRƒ“‚ð˜g“à‚ÉŽû‚ß‚é
            var rectTransform = _icon.rectTransform;
            var rectWidth = Mathf.Max(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
            rectTransform.sizeDelta *= iconWidth / Mathf.Max(RoguegardSettings.PixelsPerUnit, rectWidth);
        }
    }
}
