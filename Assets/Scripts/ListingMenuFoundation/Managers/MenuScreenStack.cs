using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    internal class MenuScreenStack<TMgr, TArg>
        where TMgr : StandardListMenuManager<TMgr, TArg>
        where TArg : IListMenuArg
    {
        private readonly Stack<StackItem> stack = new();

        public int Count => stack.Count;

        public MenuScreenStack()
        {
            Clear();
        }

        public StackItem Push(MenuScreen<TMgr, TArg> menuScreen, TArg arg)
        {
            // �񑝕���ʂ�ǉ�����Ƃ��A���߂̘A������������ʂ����ׂč폜����
            if (!menuScreen.IsIncremental)
            {
                for (int i = stack.Count - 1; i >= 0; i--)
                {
                    if (!stack.Peek().MenuScreen.IsIncremental) break;

                    stack.Pop();
                }
            }

            var stackItem = new StackItem(menuScreen, arg);
            stack.Push(stackItem);
            return stackItem;
        }

        public StackItem Pop() => stack.Pop();
        public void Clear() => stack.Clear();
        public bool TryPeek(out StackItem stackItem) => stack.TryPeek(out stackItem);

        public class StackItem
        {
            public MenuScreen<TMgr, TArg> MenuScreen { get; }
            public TArg Arg { get; }

            public StackItem(MenuScreen<TMgr, TArg> menuScreen, TArg arg)
            {
                MenuScreen = menuScreen;
                Arg = arg;
            }
        }
    }
}
