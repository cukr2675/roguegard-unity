using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public class RogueScrollViewTemplate<T> : ListViewTemplate<T, MMgr, MArg>
        where T : class
    {
        public string ScrollSubViewName { get; set; } = StandardSubViewTable.ScrollName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;
        public string BackAnchorSubViewName { get; set; } = StandardSubViewTable.BackAnchorName;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider scrollSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;
        private IElementsSubViewStateProvider backAnchorSubViewStateProvider;

        private readonly ElementHandler scrollSubViewHandler = new();

        public delegate object GetInfo(T element, MMgr manager, MArg arg);
        public delegate (object, T1) GetInfo<T1>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2) GetInfo<T1, T2>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2, T3) GetInfo<T1, T2, T3>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2, T3, T4, T5, T6) GetInfo<T1, T2, T3, T4, T5, T6>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2, T3, T4, T5, T6, T7) GetInfo<T1, T2, T3, T4, T5, T6, T7>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2, T3, T4, T5, T6, T7, T8) GetInfo<T1, T2, T3, T4, T5, T6, T7, T8>(T element, MMgr manager, MArg arg);

        public Builder ShowTemplate(
            IReadOnlyList<T> list, MMgr manager, MArg arg, object viewStateHolder = null)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                scrollSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
                backAnchorSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // スクロールのビューを表示
            OriginalList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                OriginalList.Add(list[i]);
            }

            if (TryShowSubViews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubViews(MMgr manager, MArg arg)
        {
            manager
                .GetSubView(ScrollSubViewName)
                .Show(List, scrollSubViewHandler, manager, arg, ref scrollSubViewStateProvider);

            if (Title != null)
            {
                manager
                    .GetSubView(CaptionBoxSubViewName)
                    .Show(TitleSingle, ElementToStringHandler.Instance, manager, arg, ref captionBoxSubViewStateProvider);
            }

            if (BackAnchorSubViewName != null)
            {
                manager
                    .GetSubView(BackAnchorSubViewName)
                    .Show(BackAnchorList, SelectOptionHandler.Instance, manager, arg, ref backAnchorSubViewStateProvider);
            }
        }

        public void HideTemplate(MMgr manager, bool back)
        {
            manager.GetSubView(ScrollSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
            if (BackAnchorSubViewName != null) { manager.GetSubView(BackAnchorSubViewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly RogueScrollViewTemplate<T> parent;

            public Builder(RogueScrollViewTemplate<T> parent, MMgr manager, MArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder ElementInfoFrom(GetInfo method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.GetInfo = (element, manager, arg) =>
                {
                    var nameObj = method(element, manager, arg);
                    return (nameObj, null, null, null, null, null, null, null, false);
                };
                return this;
            }

            public Builder ElementInfoFrom(GetInfo<string, string> method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.GetInfo = (element, manager, arg) =>
                {
                    var info = method(element, manager, arg);
                    return (info.Item1, null, null, null, null, null, info.Item2, info.Item3, false);
                };
                return this;
            }

            public Builder ElementInfoFrom(GetInfo<Sprite, Color, int?, float?, string, string, bool> method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.GetInfo = (element, manager, arg) =>
                {
                    var info = method(element, manager, arg);
                    return (info.Item1, null, info.Item2, info.Item3, info.Item4, info.Item5, info.Item6, info.Item7, info.Item8);
                };
                return this;
            }

            public Builder OnClickElement(HandleClickElement<T, MMgr, MArg> method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.HandleClick = method;
                return this;
            }
        }

        private class ElementHandler : IRogueElementHandler, IButtonElementHandler
        {
            public GetInfo<Color?, Sprite, Color?, int?, float?, string, string, bool> GetInfo { get; set; }
            public HandleClickElement<T, MMgr, MArg> HandleClick { get; set; }

            public Sprite GetIcon(object element, IListMenuManager manager, IListMenuArg arg) => null;

            public string GetName(object element, IListMenuManager manager, IListMenuArg arg) => "Skill";

            public void GetRogueInfo(
                object elementObj, MMgr manager, MArg arg,
                out object nameObj, ref Color color, ref Sprite icon, ref Color iconColor, ref int? stack,
                ref float? stars, ref string infoText1, ref string infoText2, ref bool equipeed)
            {
                var element = (T)elementObj;
                var info = GetInfo(element, manager, arg);
                (nameObj, _, icon, _, stack, stars, infoText1, infoText2, equipeed) = info;
                if (info.Item2.HasValue) { color = info.Item2.Value; }
                if (info.Item4.HasValue) { iconColor = info.Item4.Value; }
            }

            public string GetStyle(object element, IListMenuManager manager, IListMenuArg arg) => null;

            void IButtonElementHandler.HandleClick(object elementObj, IListMenuManager iManager, IListMenuArg iArg)
            {
                var element = (T)elementObj;
                var manager = (MMgr)iManager;
                var arg = (MArg)iArg;

                // 選択したスキルの情報と選択肢を表示する
                HandleClick(element, manager, arg);
            }
        }
    }
}
