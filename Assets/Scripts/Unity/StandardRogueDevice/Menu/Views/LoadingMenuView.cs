using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class LoadingMenuView : ModelsMenuView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Image _progressCircle = null;
        [SerializeField] private ModelsMenuViewItemButton _interruptButton = null;
        [SerializeField] private TMP_Text _text = null;

        private IModelsMenuItemController itemController;

        private readonly List<object> models = new List<object>();

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public void Initialize()
        {
            _interruptButton.Initialize(this);
        }

        public override void OpenView<T>(
            IModelsMenuItemController itemController, Spanning<T> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            this.itemController = itemController;
            this.models.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                this.models.Add(models[i]);
            }
            SetArg(root, self, user, arg);

            _progressCircle.fillAmount = 0f;
            _interruptButton.SetItem(itemController, models[1]);
            _text.text = itemController.GetName(models[0], Root, Self, User, Arg);
            MenuController.Show(_canvasGroup, true);
        }

        public override float GetPosition() => _progressCircle.fillAmount;
        public override void SetPosition(float position) => _progressCircle.fillAmount = position;

        private void Update()
        {
            if (models.Count == 0) return;

            itemController.Activate(models[0], Root, Self, User, Arg);
            if (_progressCircle.fillAmount >= 1f)
            {
                MenuController.Show(_canvasGroup, false);
                models.Clear();
                Root.Done();
            }
        }
    }
}
