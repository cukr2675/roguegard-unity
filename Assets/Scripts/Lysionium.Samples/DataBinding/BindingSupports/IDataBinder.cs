namespace Lysionium.Samples
{
    /// <summary>
    /// ビュー (<see cref="ViewItem"/> 等) とモデル間のデータバインディングの橋渡しをするインターフェース。
    /// このインターフェースがあることでビューがモデルのイベントに依存しない実装ができる。
    /// </summary>
    public interface IDataBinder
    {
        void Bind(object data);

        void Unbind(object data);
    }
}
