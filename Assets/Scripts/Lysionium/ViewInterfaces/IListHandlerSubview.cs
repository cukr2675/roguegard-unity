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

        // list 引数を object 型にすれば Subview が直接リスト要素を取得しないようにもできるが、
        // 逆に IViewItemHandler の契約（リスト要素を渡す）を満たさない・満たせない値も list 引数にできてしまう
    }
}
