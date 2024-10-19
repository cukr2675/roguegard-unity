using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class DialogViewTemplate<TMgr, TArg> : ViewTemplate<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string MenuName { get; set; }
        public string DialogSubViewName { get; set; } = StandardSubViewTable.DialogName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider dialogSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;

        private readonly List<object> list = new();
        private readonly List<object> afterList = new();
        private readonly ReadOnlyListConcat concatList;
        private readonly List<object> captionBoxList = new();

        public DialogViewTemplate()
        {
            concatList = new ReadOnlyListConcat(list, afterList);
        }

        public FluentBuilder Show(string message, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (message == null) throw new System.ArgumentNullException(nameof(message));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                dialogSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // スクロールのビューを表示
            list.Clear();
            list.Add(message);

            // メニュー名を表示
            captionBoxList.Clear();
            if (MenuName != null) { captionBoxList.Add(MenuName); }

            if (TryShowSubViews(manager, arg)) return null;
            else return new FluentBuilder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            manager
                .GetSubView(DialogSubViewName)
                .Show(concatList, SelectOptionHandler.Instance, manager, arg, ref dialogSubViewStateProvider);

            if (MenuName != null)
            {
                manager
                    .GetSubView(CaptionBoxSubViewName)
                    .Show(captionBoxList, ElementToStringHandler.Instance, manager, arg, ref captionBoxSubViewStateProvider);
            }
        }

        public void HideSubViews(TMgr manager, bool back)
        {
            manager.GetSubView(DialogSubViewName).Hide(back);
            if (MenuName != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
        }

        public class FluentBuilder : BaseFluentBuilder
        {
            private readonly DialogViewTemplate<TMgr, TArg> parent;

            public FluentBuilder(DialogViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public FluentBuilder Append(object element)
            {
                AssertNotBuilded();

                parent.afterList.Add(element);
                return this;
            }

            public FluentBuilder AppendSelectOption(string name, ListMenuSelectOption<TMgr, TArg>.HandleClickAction handleClick)
            {
                AssertNotBuilded();

                Append(ListMenuSelectOption.Create(name, handleClick));
                return this;
            }

            public FluentBuilder AppendSelectOptions(params (string, ListMenuSelectOption<TMgr, TArg>.HandleClickAction)[] selectOptions)
            {
                AssertNotBuilded();

                var stack = new List<object>();
                foreach (var selectOption in selectOptions)
                {
                    stack.Add(ListMenuSelectOption.Create(selectOption.Item1, selectOption.Item2));
                }
                Append(stack);
                return this;
            }
        }

        private class ReadOnlyListConcat : IReadOnlyList<object>
        {
            private readonly IReadOnlyList<object>[] lists;

            public object this[int index]
            {
                get
                {
                    foreach (var list in lists)
                    {
                        if (index < list.Count)
                        {
                            return list[index];
                        }
                        else
                        {
                            index -= list.Count;
                        }
                    }
                    throw new System.IndexOutOfRangeException();
                }
            }

            public int Count
            {
                get
                {
                    var sumCount = 0;
                    foreach (var list in lists)
                    {
                        sumCount += list.Count;
                    }
                    return sumCount;
                }
            }

            public ReadOnlyListConcat(params IReadOnlyList<object>[] lists)
            {
                this.lists = lists;
            }

            public IEnumerator<object> GetEnumerator()
            {
                foreach (var list in lists)
                {
                    foreach (var item in list)
                    {
                        yield return item;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
