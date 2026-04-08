using System.Collections.Generic;

namespace Lysionium
{
    internal class ListuiScreenStack<TMgr>
        where TMgr : StandardListuiManager<TMgr>
    {
        private readonly Stack<StackItem> stack = new();

        public int Count => stack.Count;

        private StackItem Push(StackItem stackItem)
        {
            // 非増分画面を追加するとき、直近の連続した増分画面をすべて削除する
            if (!stackItem.Screen.IsIncremental)
            {
                for (int i = stack.Count - 1; i >= 0; i--)
                {
                    if (!stack.Peek().Screen.IsIncremental) break;

                    stack.Pop();
                }
            }

            stack.Push(stackItem);
            return stackItem;
        }

        public StackItem Push(IListuiScreen<TMgr> screen) => Push(new StackItemImpl(screen));
        public StackItem Push<TArg>(IListuiScreen<TMgr, TArg> screen, TArg arg) => Push(new StackItemImpl<TArg>(screen, arg));

        public StackItem Pop() => stack.Pop();
        public void Clear() => stack.Clear();
        public bool TryPeek(out StackItem stackItem) => stack.TryPeek(out stackItem);

        public abstract class StackItem
        {
            internal abstract IListuiScreen Screen { get; }

            public abstract void OpenScreen(TMgr manager);
            public abstract void CloseScreenView(TMgr manager, bool back);
        }

        private class StackItemImpl : StackItem
        {
            private readonly IListuiScreen<TMgr> _screen;
            internal override IListuiScreen Screen => _screen;

            public StackItemImpl(IListuiScreen<TMgr> screen)
            {
                _screen = screen;
            }

            public override void OpenScreen(TMgr manager) => _screen.OpenScreen(manager);
            public override void CloseScreenView(TMgr manager, bool back) => _screen.CloseScreenView(manager, back);
        }

        private class StackItemImpl<TArg> : StackItem
        {
            private readonly IListuiScreen<TMgr, TArg> _screen;
            internal override IListuiScreen Screen => _screen;
            private readonly TArg arg;

            public StackItemImpl(IListuiScreen<TMgr, TArg> screen, TArg arg)
            {
                _screen = screen;
                this.arg = arg;
            }

            public override void OpenScreen(TMgr manager) => _screen.OpenScreen(manager, arg);
            public override void CloseScreenView(TMgr manager, bool back) => _screen.CloseScreenView(manager, back, arg);
        }
    }
}
