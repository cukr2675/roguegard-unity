using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using TMPro;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class FloorMenuView : ModelsMenuView, IPointerClickHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private TMP_Text _text = null;

        private IModelListPresenter presenter;

        private readonly List<object> modelList = new List<object>();

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private float count;

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
            _text.text = presenter.GetItemName(modelList[0], Root, Self, User, Arg);
            MenuController.Show(_canvasGroup, true);
            count = 2f;
        }

        public override float GetPosition() => 0f;
        public override void SetPosition(float position) { }

        private void Update()
        {
            if (count <= 0f) return;

            count -= Time.deltaTime;
            if (count > 0f) return;

            presenter.ActivateItem(modelList[0], Root, Self, User, Arg);
            MenuController.Show(_canvasGroup, false);
            Root.Done();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (count <= 0f) return;

            count = 0f;
            presenter.ActivateItem(modelList[0], Root, Self, User, Arg);
            MenuController.Show(_canvasGroup, false);
            Root.Done();
        }
    }
}
