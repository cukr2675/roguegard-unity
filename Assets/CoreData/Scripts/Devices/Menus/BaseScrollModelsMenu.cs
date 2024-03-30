using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public abstract class BaseScrollModelsMenu<T> : IModelsMenu, IModelsMenuItemController
    {
        protected virtual string MenuName => null;
        protected virtual IKeyword ViewKeyword => DeviceKw.MenuScroll;

        protected ModelsMenuViewPosition ViewPosition { get; }

        private readonly ExitModelsMenuChoice exitChoice;
        private readonly List<object> leftAnchorModels;

        protected BaseScrollModelsMenu()
        {
            ViewPosition = new ModelsMenuViewPosition(ViewKeyword);
            exitChoice = new ExitModelsMenuChoice(Exit);
            leftAnchorModels = new List<object>();
        }

        protected abstract Spanning<T> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var models = GetModels(root, self, user, arg);
            var scroll = root.Get(ViewKeyword);
            scroll.OpenView(this, models, root, self, user, arg);

            var holder = GetViewPositionHolder(root, self, user, arg);
            if (!ViewPosition.Load(root, holder))
            {
                var defaultPosition = GetDefaultViewPosition(root, self, user, arg);
                ViewPosition.Set(root, holder, defaultPosition);
            }

            var caption = root.Get(DeviceKw.MenuCaption);
            caption.OpenView(null, Spanning<object>.Empty, null, null, null, new(other: MenuName));

            leftAnchorModels.Clear();
            leftAnchorModels.Add(exitChoice);
            GetLeftAnchorModels(root, self, user, arg, leftAnchorModels);
            var leftAnchor = root.Get(DeviceKw.MenuLeftAnchor);
            leftAnchor.OpenView(ChoicesModelsMenuItemController.Instance, leftAnchorModels, root, self, user, arg);
        }

        protected virtual object GetViewPositionHolder(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return self;
        }

        protected virtual float GetDefaultViewPosition(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return 0f;
        }

        protected virtual void GetLeftAnchorModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, List<object> models)
        {
        }

        protected abstract string GetItemName(T model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);
        protected abstract void ItemActivate(T model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        private void Exit(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            // スクロール位置を記憶する
            var holder = GetViewPositionHolder(root, self, user, arg);
            ViewPosition.Save(root, holder);
        }



        string IModelsMenuItemController.GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (model == null || !(model is T t)) return null;

            // 個別処理を実行
            return GetItemName(t, root, self, user, arg);
        }

        void IModelsMenuItemController.Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            // スクロール位置を記憶する
            var holder = GetViewPositionHolder(root, self, user, arg);
            ViewPosition.Save(root, holder);

            if (model == null || !(model is T t))
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                return;
            }
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

            // 個別処理を実行
            ItemActivate(t, root, self, user, arg);
        }
    }
}
