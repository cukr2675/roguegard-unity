using Lysionium;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class RogueScrollMenuViewData<T> : ListViewData<T, MMgr, MArg>
        where T : class
    {
        public System.Func<MMgr, IListHandlerSubview> ScrollSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Scroll;
        public System.Func<MMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<MMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.BackAnchor;
        public SelectOptionList<MMgr, MArg> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        private object prevViewStateHolder;
        private ISubviewStateProvider scrollSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        private readonly ElementHandler scrollSubviewHandler = new();

        public Builder Show(T[] list, MMgr manager, MArg arg, object viewStateHolder = null)
        {
            SetOriginalList(list, manager, arg);
            return Show(manager, arg, viewStateHolder);
        }

        public Builder Show(IReadOnlyList<T> list, MMgr manager, MArg arg, object viewStateHolder = null)
        {
            SetOriginalList(list, manager, arg);
            return Show(manager, arg, viewStateHolder);
        }

        public Builder Show(System.ReadOnlySpan<T> list, MMgr manager, MArg arg, object viewStateHolder = null)
        {
            SetOriginalList(list, manager, arg);
            return Show(manager, arg, viewStateHolder);
        }

        public Builder Show(MMgr manager, MArg arg, object viewStateHolder)
        {
            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                scrollSubviewStateProvider?.Reset();
                captionBoxSubviewStateProvider?.Reset();
                backAnchorSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubviews(MMgr manager, MArg arg)
        {
            ScrollSubviewSelector?.Invoke(manager)?.Show(
                List, scrollSubviewHandler, manager, arg, ref scrollSubviewStateProvider);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    Title, manager, arg, ref captionBoxSubviewStateProvider);
            }

            BackAnchorSubviewSelector?.Invoke(manager)?.Show(
                BackAnchorList, manager, arg, ref backAnchorSubviewStateProvider);
        }

        public void Hide(MMgr manager, bool back)
        {
            ScrollSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<RogueScrollMenuViewData<T>, Builder>
        {
            public Builder(RogueScrollMenuViewData<T> parent, MMgr manager, MArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder InfoFrom(System.Func<T, MMgr, MArg, object> method)
            {
                AssertNotBuilt();

                Parent.scrollSubviewHandler.GetInfo = (item, manager, arg) =>
                {
                    var nameObj = method(item, manager, arg);
                    return (nameObj, null, null, null, null, null, null, null, false);
                };
                return this;
            }

            public Builder InfoFrom(System.Func<T, MMgr, MArg, (object, string, string)> method)
            {
                AssertNotBuilt();

                Parent.scrollSubviewHandler.GetInfo = (item, manager, arg) =>
                {
                    var info = method(item, manager, arg);
                    return (info.Item1, null, null, null, null, null, info.Item2, info.Item3, false);
                };
                return this;
            }

            public Builder InfoFrom(System.Func<T, MMgr, MArg, (object, Sprite, Color, int?, float?, string, string, bool)> method)
            {
                AssertNotBuilt();

                Parent.scrollSubviewHandler.GetInfo = (item, manager, arg) =>
                {
                    var info = method(item, manager, arg);
                    return (info.Item1, null, info.Item2, info.Item3, info.Item4, info.Item5, info.Item6, info.Item7, info.Item8);
                };
                return this;
            }

            public Builder OnClick(ClickItemHandler<T, MMgr, MArg> method)
            {
                AssertNotBuilt();

                Parent.scrollSubviewHandler.Click = method;
                return this;
            }
        }

        private class ElementHandler : IRogueElementHandler, IButtonViewItemHandler
        {
            public System.Func<T, MMgr, MArg, (object, Color?, Sprite, Color?, int?, float?, string, string, bool)> GetInfo { get; set; }
            public ClickItemHandler<T, MMgr, MArg> Click { get; set; }

            public string GetName(object itemObj, IListuiManager manager, IListuiArg arg)
            {
                var item = (T)itemObj;
                var info = GetInfo(item, (MMgr)manager, (MArg)arg);
                return info.Item1.ToString();
            }

            public void GetRogueInfo(
                object itemObj, MMgr manager, MArg arg,
                out object nameObj, ref Color color, ref Sprite icon, ref Color iconColor, ref int? stack,
                ref float? stars, ref string infoText1, ref string infoText2, ref bool equipeed)
            {
                var item = (T)itemObj;
                var info = GetInfo(item, manager, arg);
                (nameObj, _, icon, _, stack, stars, infoText1, infoText2, equipeed) = info;
                if (info.Item2.HasValue) { color = info.Item2.Value; }
                if (info.Item4.HasValue) { iconColor = info.Item4.Value; }
            }

            public string GetStyle(object item, IListuiManager manager, IListuiArg arg) => string.Empty;

            void IButtonViewItemHandler.Click(object itemObj, IListuiManager iManager, IListuiArg iArg)
            {
                var item = (T)itemObj;
                var manager = (MMgr)iManager;
                var arg = (MArg)iArg;

                // 選択したスキルの情報と選択肢を表示する
                Click(item, manager, arg);
            }
        }
    }
}
