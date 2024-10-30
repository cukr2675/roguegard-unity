using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class SelectFileMenuViewTemplate : ListViewTemplate<object, RogueMenuManager, ReadOnlyMenuArg>
    {
        public string ScrollSubViewName { get; set; } = StandardSubViewTable.ScrollName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;
        public string BackAnchorSubViewName { get; set; } = StandardSubViewTable.BackAnchorName;
        public List<IListMenuSelectOption> BackAnchorList { get; set; } = new() { ExitListMenuSelectOption.Instance };

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider scrollSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;
        private IElementsSubViewStateProvider backAnchorSubViewStateProvider;

        private readonly ElementHandler scrollSubViewHandler = new();

        public Builder ShowTemplate(
            IReadOnlyList<FileInfo> list, RogueMenuManager manager, ReadOnlyMenuArg arg, object viewStateHolder = null)
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

        protected override void ShowSubViews(RogueMenuManager manager, ReadOnlyMenuArg arg)
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

            if (BackAnchorList != null)
            {
                manager
                    .GetSubView(BackAnchorSubViewName)
                    .Show(BackAnchorList, SelectOptionHandler.Instance, manager, arg, ref backAnchorSubViewStateProvider);
            }
        }

        public void HideTemplate(RogueMenuManager manager, bool back)
        {
            manager.GetSubView(ScrollSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
            if (BackAnchorList != null) { manager.GetSubView(BackAnchorSubViewName).Hide(back); }
        }

        private class ElementHandler : ButtonElementHandler<object, RogueMenuManager, ReadOnlyMenuArg>, IRogueElementHandler
        {
            public void GetRogueInfo(
                object element, RogueMenuManager manager, ReadOnlyMenuArg arg,
                out string name, ref Color color, ref Sprite icon, ref Color iconColor,
                ref int? stack, ref float? stars, ref string infoText1, ref string infoText2)
            {
                if (element is FileInfo fileInfo)
                {
                    name = fileInfo.Name;
                    infoText2 = fileInfo.LastWriteTime.ToString();
                }
                else if (element is IListMenuSelectOption option)
                {
                    name = option.GetName(manager, arg);
                }
                else
                {
                    throw new System.InvalidOperationException();
                }
            }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly SelectFileMenuViewTemplate parent;

            public Builder(SelectFileMenuViewTemplate parent, RogueMenuManager manager, ReadOnlyMenuArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder OnClickElement(HandleClickElement<object, RogueMenuManager, ReadOnlyMenuArg> method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.HandleClick = method;
                return this;
            }
        }
    }
}
