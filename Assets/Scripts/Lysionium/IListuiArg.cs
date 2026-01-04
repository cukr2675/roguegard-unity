namespace Lysionium
{
    // 命名メモ: IListuiParameter, IListuiParam にして TParam にすると型引数名が長すぎる。 TParm, TPrm は parameter の略として一般的ではない
    // 予約語の 'params' と被りやすいのもデメリット

    /// <summary>
    /// <see cref="IListuiManager"/> の各画面に渡す引数となるインターフェース
    /// </summary>
    public interface IListuiArg
    {
        void CopyTo(ref IListuiArg dest);
    }
}
