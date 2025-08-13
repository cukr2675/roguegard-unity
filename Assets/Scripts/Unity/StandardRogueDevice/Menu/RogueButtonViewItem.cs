using Lysionium;
using Roguegard;
using Roguegard.Device;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoguegardUnity
{
    public class RogueButtonViewItem : ViewItem
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
        private Animator animator;

        private IButtonViewItemHandler handler;
        private object item;

        private static readonly RogueNameBuilder nameBuilder = new();

        private void Awake()
        {
            iconWidth = _icon.rectTransform.rect.width;

            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                handler.Click(item, Manager, Arg);
            });

            TryGetComponent(out animator);
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            this.handler = handler as IButtonViewItemHandler;
            this.item = item;

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
                    item, manager, arg, out var nameObj, ref color, ref icon, ref iconColor, ref stack,
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
                var baseText = handler.GetName(item, Manager, Arg);
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
                var style = handler.GetStyle(item, Manager, Arg);
                if (style == null) { style = _defaultStyle; }
                for (int i = 0; i < animator.layerCount; i++)
                {
                    var weight = animator.GetLayerName(i) == style ? 1f : 0f;
                    animator.SetLayerWeight(i, weight);
                }
            }
        }

        protected override void UnbindCore(object item, IViewItemHandler handler)
        {
            _nameText.text = null;
            _icon.enabled = false;
            _icon.sprite = null;
            _stackText.text = null;
            _info1Text.text = null;
            _info2Text.text = null;
            _equipText.enabled = false;
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

            // アイコンを枠内に収める
            var rectTransform = _icon.rectTransform;
            var rectWidth = Mathf.Max(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
            rectTransform.sizeDelta *= iconWidth / Mathf.Max(RoguegardSettings.PixelsPerUnit, rectWidth);
        }
    }
}
