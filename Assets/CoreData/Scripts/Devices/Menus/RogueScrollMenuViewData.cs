using Lysionium;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class RogueScrollMenuViewData<T> : ListViewData<T, MMgr>
        where T : class
    {
        public System.Func<MMgr, IListHandlerSubview> ScrollSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Scroll;
        public System.Func<MMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public System.Func<MMgr, IListHandlerSubview> BackAnchorSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.BackAnchor;
        public SelectOptionList<MMgr> BackAnchorList { get; set; } = new(_ => _.BackIfReflectable());

        private object prevViewStateHolder;
        private ISubviewStateProvider scrollSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        private readonly ElementHandler scrollSubviewHandler = new();

        public Builder Show(T[] list, MMgr manager, object viewStateHolder = null)
        {
            SetOriginalList(list, manager);
            return Show(manager, viewStateHolder);
        }

        public Builder Show(IReadOnlyList<T> list, MMgr manager, object viewStateHolder = null)
        {
            SetOriginalList(list, manager);
            return Show(manager, viewStateHolder);
        }

        public Builder Show(System.ReadOnlySpan<T> list, MMgr manager, object viewStateHolder = null)
        {
            SetOriginalList(list, manager);
            return Show(manager, viewStateHolder);
        }

        public Builder Show(MMgr manager, object viewStateHolder)
        {
            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                scrollSubviewStateProvider?.Reset();
                captionBoxSubviewStateProvider?.Reset();
                backAnchorSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            if (TryShowSubviews(manager)) return null;
            else return new Builder(this, manager);
        }

        protected override void ShowSubviews(MMgr manager)
        {
            ScrollSubviewSelector?.Invoke(manager)?.Show(
                List, scrollSubviewHandler, manager, ref scrollSubviewStateProvider);

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    Title, manager, ref captionBoxSubviewStateProvider);
            }

            BackAnchorSubviewSelector?.Invoke(manager)?.Show(
                BackAnchorList, manager, ref backAnchorSubviewStateProvider);
        }

        public void Hide(MMgr manager, bool back)
        {
            ScrollSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
            BackAnchorSubviewSelector?.Invoke(manager)?.Hide(back);
        }

        public class Builder : BaseListBuilder<RogueScrollMenuViewData<T>, Builder>
        {
            public Builder(RogueScrollMenuViewData<T> parent, MMgr manager)
                : base(parent, manager)
            {
            }

            public Builder InfoFrom(System.Func<T, MMgr, object> method)
            {
                AssertNotBuilt();

                Parent.scrollSubviewHandler.GetInfo = (item, manager) =>
                {
                    var nameObj = method(item, manager);
                    return (nameObj, null, null, null, null, null, null, null, false);
                };
                return this;
            }

            public Builder InfoFrom(System.Func<T, MMgr, (object, string, string)> method)
            {
                AssertNotBuilt();

                Parent.scrollSubviewHandler.GetInfo = (item, manager) =>
                {
                    var info = method(item, manager);
                    return (info.Item1, null, null, null, null, null, info.Item2, info.Item3, false);
                };
                return this;
            }

            public Builder InfoFrom(System.Func<T, MMgr, (object, Sprite, Color, int?, float?, string, string, bool)> method)
            {
                AssertNotBuilt();

                Parent.scrollSubviewHandler.GetInfo = (item, manager) =>
                {
                    var info = method(item, manager);
                    return (info.Item1, null, info.Item2, info.Item3, info.Item4, info.Item5, info.Item6, info.Item7, info.Item8);
                };
                return this;
            }

            public Builder OnClick(System.Action<T, MMgr> method)
            {
                AssertNotBuilt();

                Parent.scrollSubviewHandler.Click = method;
                return this;
            }
        }

        private class ElementHandler : IRogueElementHandler, IButtonViewItemHandler
        {
            public System.Func<T, MMgr, (object, Color?, Sprite, Color?, int?, float?, string, string, bool)> GetInfo { get; set; }
            public System.Action<T, MMgr> Click { get; set; }

            private static readonly IReadOnlyList<string> clickSingle = new List<string> { "Click" };

            public string GetName(object itemObj, IListuiManager manager)
            {
                var item = (T)itemObj;
                var info = GetInfo(item, (MMgr)manager);
                return info.Item1.ToString();
            }

            public void GetRogueInfo(
                object itemObj, MMgr manager,
                out object nameObj, ref Color color, ref Sprite icon, ref Color iconColor, ref int? stack,
                ref float? stars, ref string infoText1, ref string infoText2, ref bool equipeed)
            {
                var item = (T)itemObj;
                var info = GetInfo(item, manager);
                (nameObj, _, icon, _, stack, stars, infoText1, infoText2, equipeed) = info;
                if (info.Item2.HasValue) { color = info.Item2.Value; }
                if (info.Item4.HasValue) { iconColor = info.Item4.Value; }
            }

            public string GetStyle(object item, IListuiManager manager) => string.Empty;

            IReadOnlyList<string> IButtonViewItemHandler.GetCandidateClickNames(object item, IListuiManager manager)
            {
                return clickSingle;
            }

            void IButtonViewItemHandler.Click(object itemObj, IListuiManager iManager, string clickName)
            {
                var item = (T)itemObj;
                var manager = (MMgr)iManager;

                // 選択したスキルの情報と選択肢を表示する
                Click(item, manager);
            }
        }
    }
}
