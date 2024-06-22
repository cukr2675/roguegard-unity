using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class LoadingMenuView : ElementsView
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Image _progressCircle = null;
        [SerializeField] private ViewElementButton _interruptButton = null;
        [SerializeField] private TMP_Text _text = null;

        private IElementPresenter presenter;

        private readonly List<object> list = new List<object>();

        public override CanvasGroup CanvasGroup => _canvasGroup;

        public void Initialize()
        {
            _interruptButton.Initialize(this);
        }

        public override void OpenView<T>(
            IElementPresenter presenter, Spanning<T> list, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            this.presenter = presenter;
            this.list.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                this.list.Add(list[i]);
            }
            SetArg(manager, self, user, arg);

            _progressCircle.fillAmount = 0f;
            _interruptButton.SetItem(presenter, list[1]);
            _text.text = presenter.GetItemName(list[0], Root, Self, User, Arg);
            MenuController.Show(_canvasGroup, true);
        }

        public override float GetPosition() => _progressCircle.fillAmount;
        public override void SetPosition(float position) => _progressCircle.fillAmount = position;

        private void Update()
        {
            if (list.Count == 0) return;

            presenter.ActivateItem(list[0], Root, Self, User, Arg);
            if (_progressCircle.fillAmount >= 1f)
            {
                MenuController.Show(_canvasGroup, false);
                list.Clear();
                Root.Done();
            }
        }
    }
}
