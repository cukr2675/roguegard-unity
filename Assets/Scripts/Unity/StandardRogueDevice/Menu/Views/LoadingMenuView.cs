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

        private IModelListPresenter presenter;

        private readonly List<object> modelList = new List<object>();

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public void Initialize()
        {
            _interruptButton.Initialize(this);
        }

        public override void OpenView<T>(
            IModelListPresenter presenter, Spanning<T> modelList, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            this.presenter = presenter;
            this.modelList.Clear();
            for (int i = 0; i < modelList.Count; i++)
            {
                this.modelList.Add(modelList[i]);
            }
            SetArg(root, self, user, arg);

            _progressCircle.fillAmount = 0f;
            _interruptButton.SetItem(presenter, modelList[1]);
            _text.text = presenter.GetItemName(modelList[0], Root, Self, User, Arg);
            MenuController.Show(_canvasGroup, true);
        }

        public override float GetPosition() => _progressCircle.fillAmount;
        public override void SetPosition(float position) => _progressCircle.fillAmount = position;

        private void Update()
        {
            if (modelList.Count == 0) return;

            presenter.ActivateItem(modelList[0], Root, Self, User, Arg);
            if (_progressCircle.fillAmount >= 1f)
            {
                MenuController.Show(_canvasGroup, false);
                modelList.Clear();
                Root.Done();
            }
        }
    }
}
