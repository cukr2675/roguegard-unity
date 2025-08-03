using Lysionium;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class RogueScrollViewData<T> : ListViewData<T, MMgr, MArg>
        where T : class
    {
        public string ScrollSubviewName { get; set; } = StandardSubviewTable.ScrollName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public string BackAnchorSubviewName { get; set; } = StandardSubviewTable.BackAnchorName;
        public List<ISelectOption> BackAnchorList { get; set; } = new() { BackSelectOption.Instance };

        private object prevViewStateHolder;
        private ISubviewStateProvider scrollSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private ISubviewStateProvider backAnchorSubviewStateProvider;

        private readonly ElementHandler scrollSubviewHandler = new();

        public delegate object GetInfo(T element, MMgr manager, MArg arg);
        public delegate (object, T1) GetInfo<T1>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2) GetInfo<T1, T2>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2, T3) GetInfo<T1, T2, T3>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2, T3, T4, T5, T6) GetInfo<T1, T2, T3, T4, T5, T6>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2, T3, T4, T5, T6, T7) GetInfo<T1, T2, T3, T4, T5, T6, T7>(T element, MMgr manager, MArg arg);
        public delegate (object, T1, T2, T3, T4, T5, T6, T7, T8) GetInfo<T1, T2, T3, T4, T5, T6, T7, T8>(T element, MMgr manager, MArg arg);

        public Builder Show(IReadOnlyList<T> list, MMgr manager, MArg arg, object viewStateHolder = null)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                scrollSubviewStateProvider?.Reset();
                captionBoxSubviewStateProvider?.Reset();
                backAnchorSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // スクロールのビューを表示
            OriginalList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                OriginalList.Add(list[i]);
            }

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubviews(MMgr manager, MArg arg)
        {
            manager
                .GetSubview(ScrollSubviewName)
                .Show(List, scrollSubviewHandler, manager, arg, ref scrollSubviewStateProvider);

            if (Title != null)
            {
                manager
                    .GetSubview(CaptionBoxSubviewName)
                    .Show(TitleSingle, ToStringViewItemHandler.Instance, manager, arg, ref captionBoxSubviewStateProvider);
            }

            if (BackAnchorSubviewName != null)
            {
                manager
                    .GetSubview(BackAnchorSubviewName)
                    .Show(BackAnchorList, SelectOptionViewItemHandler.Instance, manager, arg, ref backAnchorSubviewStateProvider);
            }
        }

        public void Hide(MMgr manager, bool back)
        {
            manager.GetSubview(ScrollSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
            if (BackAnchorSubviewName != null) { manager.GetSubview(BackAnchorSubviewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly RogueScrollViewData<T> parent;

            public Builder(RogueScrollViewData<T> parent, MMgr manager, MArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder InfoFrom(GetInfo method)
            {
                AssertNotBuilt();

                parent.scrollSubviewHandler.GetInfo = (element, manager, arg) =>
                {
                    var nameObj = method(element, manager, arg);
                    return (nameObj, null, null, null, null, null, null, null, false);
                };
                return this;
            }

            public Builder InfoFrom(GetInfo<string, string> method)
            {
                AssertNotBuilt();

                parent.scrollSubviewHandler.GetInfo = (element, manager, arg) =>
                {
                    var info = method(element, manager, arg);
                    return (info.Item1, null, null, null, null, null, info.Item2, info.Item3, false);
                };
                return this;
            }

            public Builder InfoFrom(GetInfo<Sprite, Color, int?, float?, string, string, bool> method)
            {
                AssertNotBuilt();

                parent.scrollSubviewHandler.GetInfo = (element, manager, arg) =>
                {
                    var info = method(element, manager, arg);
                    return (info.Item1, null, info.Item2, info.Item3, info.Item4, info.Item5, info.Item6, info.Item7, info.Item8);
                };
                return this;
            }

            public Builder OnClick(ClickItemHandler<T, MMgr, MArg> method)
            {
                AssertNotBuilt();

                parent.scrollSubviewHandler.Click = method;
                return this;
            }
        }

        private class ElementHandler : IRogueElementHandler, IButtonViewItemHandler
        {
            public GetInfo<Color?, Sprite, Color?, int?, float?, string, string, bool> GetInfo { get; set; }
            public ClickItemHandler<T, MMgr, MArg> Click { get; set; }

            public string GetName(object elementObj, IListMenuManager manager, IListMenuArg arg)
            {
                var element = (T)elementObj;
                var info = GetInfo(element, (MMgr)manager, (MArg)arg);
                return info.Item1.ToString();
            }

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

            void IButtonViewItemHandler.Click(object elementObj, IListMenuManager iManager, IListMenuArg iArg)
            {
                var element = (T)elementObj;
                var manager = (MMgr)iManager;
                var arg = (MArg)iArg;

                // 選択したスキルの情報と選択肢を表示する
                Click(element, manager, arg);
            }
        }
    }
}
