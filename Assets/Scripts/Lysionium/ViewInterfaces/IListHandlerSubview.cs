using System.Collections.Generic;

namespace Lysionium
{
    public interface IListHandlerSubview : ISubview
    {
        // リストとハンドラを Subpresenter でカプセル化すべきかもしれないが、
        // リストのコピーとハンドラのダウンキャストを考えると複雑になるためそのまま渡す
        void SetParameters(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider);
    }
}
