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

        private IModelsMenuItemController itemController;

        private readonly List<object> models = new List<object>();

        public override CanvasGroup CanvasGroup => _canvasGroup;

        private float count;

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
            _text.text = itemController.GetName(models[0], Root, Self, User, Arg);
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

            itemController.Activate(models[0], Root, Self, User, Arg);
            MenuController.Show(_canvasGroup, false);
            Root.Done();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (count <= 0f) return;

            count = 0f;
            itemController.Activate(models[0], Root, Self, User, Arg);
            MenuController.Show(_canvasGroup, false);
            Root.Done();
        }
    }
}
