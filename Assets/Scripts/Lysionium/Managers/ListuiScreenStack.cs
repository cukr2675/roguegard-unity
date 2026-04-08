using System.Collections.Generic;

namespace Lysionium
{
    internal class ListuiScreenStack<TMgr, TArg>
        where TMgr : StandardListuiManager<TMgr, TArg>
        where TArg : IListuiArg
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

        public StackItem Push(IListuiScreen<TMgr, TArg> screen, TArg arg)
            => Push(new StackItemImpl(screen, arg));
        public StackItem Push<TCtx>(IListuiScreen<TMgr, TArg, TCtx> screen, TArg arg, TCtx context)
            => Push(new StackItemImpl<TCtx>(screen, arg, context));

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
            private readonly IListuiScreen<TMgr, TArg> _screen;
            internal override IListuiScreen Screen => _screen;
            private readonly TArg arg;

            public StackItemImpl(IListuiScreen<TMgr, TArg> screen, TArg arg)
            {
                _screen = screen;
                this.arg = arg;
            }

            public override void OpenScreen(TMgr manager) => _screen.OpenScreen(manager, arg);
            public override void CloseScreenView(TMgr manager, bool back) => _screen.CloseScreenView(manager, back);
        }

        private class StackItemImpl<TCtx> : StackItem
        {
            private readonly IListuiScreen<TMgr, TArg, TCtx> _screen;
            internal override IListuiScreen Screen => _screen;
            private readonly TArg arg;
            private readonly TCtx context;

            public StackItemImpl(IListuiScreen<TMgr, TArg, TCtx> screen, TArg arg, TCtx context)
            {
                _screen = screen;
                this.arg = arg;
                this.context = context;
            }

            public override void OpenScreen(TMgr manager) => _screen.OpenScreen(manager, arg, context);
            public override void CloseScreenView(TMgr manager, bool back) => _screen.CloseScreenView(manager, back, context);
        }
    }
}
