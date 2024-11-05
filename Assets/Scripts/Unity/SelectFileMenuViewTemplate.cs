using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class SelectFileMenuViewTemplate : ListViewTemplate<object, MMgr, MArg>
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

        public Builder ShowTemplate(
            IReadOnlyList<FileInfo> list, MMgr manager, MArg arg, object viewStateHolder = null)
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

            if (BackAnchorList != null)
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
            if (BackAnchorList != null) { manager.GetSubView(BackAnchorSubViewName).Hide(back); }
        }

        private class ElementHandler : ButtonElementHandler<object, MMgr, MArg>, IRogueElementHandler
        {
            public ElementHandler()
            {
                GetName = (element, manager, arg) =>
                {
                    Color color = default;
                    Sprite icon = default;
                    Color iconColor = default;
                    int? stack = default;
                    float? stars = default;
                    string infoText1 = default;
                    string infoText2 = default;
                    GetRogueInfo(element, manager, arg, out var name, ref color, ref icon, ref iconColor, ref stack, ref stars, ref infoText1, ref infoText2);
                    return name;
                };
            }

            public void GetRogueInfo(
                object element, MMgr manager, MArg arg,
                out string name, ref Color color, ref Sprite icon, ref Color iconColor,
                ref int? stack, ref float? stars, ref string infoText1, ref string infoText2)
            {
                if (element is FileInfo fileInfo)
                {
                    name = fileInfo.Name;
                    infoText2 = fileInfo.LastWriteTime.ToString();
                }
                else if (element is ISelectOption option)
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

            public Builder(SelectFileMenuViewTemplate parent, MMgr manager, MArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder OnClickElement(HandleClickElement<object, MMgr, MArg> method)
            {
                AssertNotBuilded();

                parent.scrollSubViewHandler.HandleClick = method;
                return this;
            }
        }
    }
}
