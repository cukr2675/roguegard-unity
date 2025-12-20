namespace Lysionium
{
    // Lysionium.MergeExtensions のように Observer 風に GetName や Click に分岐させればハンドラのダウンキャストを無くせるが
    // ・ViewItem が利用するハンドラをインターフェースとして保持できないため、依存関係が不明瞭になる
    // ・ハンドラの実装と呼び出しが直感的ではなくなる（参照やスタックトレースが追いづらくなる）
    // などデメリットがある

    /// <summary>
    /// <see cref="ISubview"/> に各要素をどのように扱わせるかを設定するインターフェース。
    /// 一つのメニュー画面が複数の <see cref="IViewItemHandler"/> を持つ可能性があるため分けて考える。
    /// </summary>
    public interface IViewItemHandler
    {
        // 期待する契約的には GetName(IReadOnlyList<object> list, int index, IListMenuManager manager, IListMenuArg arg) だが、
        // 回りくどく、かえって分かりづらいため却下

        string GetName(object item, IListMenuManager manager, IListMenuArg arg);

        // 右クリックなど種類が増えるとメソッド一つでは足りない
        // メソッドが増えると実装が面倒
        // 戻るボタンは常に esc キーをバインドするのであれば Style と統合したほうがスムーズ
        //InputAction GetKeybind(object item, IListMenuManager manager, IListMenuArg arg);

        // Roguegard の実装を見ると、アイコン以外の情報も同時に返すほうが効率的なため使用していない
        // 使用するとしてもキーバインドと同じように Style でキーワードを渡すべき？
        // GetName はデバッグにも使用できるが GetIcon は不向き
        //Sprite GetIcon(object item, IListMenuManager manager, IListMenuArg arg);

        // Name と Style はラベルをはじめとしたほとんどのコントロールで使うため実装する

        // Style は用途的には string[] のほうが近いが、コンマ区切りやデフォルト引数等を考えると冗長に見えるので string にする
        string GetStyle(object item, IListMenuManager manager, IListMenuArg arg);
    }
}
