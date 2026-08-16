namespace Lysionium
{
    public interface IViewItemHandlerBuilder<TItem, TMgr, TBuilder>
    {
        TBuilder Init(System.Func<System.IDisposable> func);

        // 命名メモ:
        // List-UI のビルダーはコンテンツの可読性を重視してメソッドの長さを抑えたい
        // - NameFrom(x => x.Name) : 文章のように読める
        // - WithNameSelector(x => x.Name) : 具体的だが長すぎてデリゲートの記述がメソッド名に埋もれる
        // - Name(x => x.Name) : 短すぎて他の構文に埋もれる？
        TBuilder NameFrom(System.Func<TItem, TMgr, string> selector);

        TBuilder StyleFrom(System.Func<TItem, TMgr, string> selector);
    }
}
