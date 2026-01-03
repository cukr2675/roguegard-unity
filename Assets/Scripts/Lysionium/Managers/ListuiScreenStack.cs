using System.Collections.Generic;

namespace Lysionium
{
    internal class ListuiScreenStack<TMgr, TArg>
        where TMgr : StandardListuiManager<TMgr, TArg>
        where TArg : IListuiArg
    {
        private readonly Stack<StackItem> stack = new();

        public IListuiScreen<TMgr, TArg> Peek => stack.TryPeek(out var item) ? item.Screen : null;

        public int Count => stack.Count;

        public ListuiScreenStack()
        {
            Clear();
        }

        public StackItem Push(IListuiScreen<TMgr, TArg> screen, TArg arg)
        {
            // 非増分画面を追加するとき、直近の連続した増分画面をすべて削除する
            if (!screen.IsIncremental)
            {
                for (int i = stack.Count - 1; i >= 0; i--)
                {
                    if (!stack.Peek().Screen.IsIncremental) break;

                    stack.Pop();
                }
            }

            var stackItem = new StackItem(screen, arg);
            stack.Push(stackItem);
            return stackItem;
        }

        public StackItem Pop() => stack.Pop();
        public void Clear() => stack.Clear();
        public bool TryPeek(out StackItem stackItem) => stack.TryPeek(out stackItem);

        public class StackItem
        {
            public IListuiScreen<TMgr, TArg> Screen { get; }
            public TArg Arg { get; }

            public StackItem(IListuiScreen<TMgr, TArg> screen, TArg arg)
            {
                Screen = screen;
                Arg = arg;
            }
        }
    }
}
