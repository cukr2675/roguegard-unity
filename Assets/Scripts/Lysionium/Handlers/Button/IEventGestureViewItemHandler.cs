using System.Collections.Generic;

namespace Lysionium
{
    public interface IEventGestureViewItemHandler : IViewItemHandler
    {
        static IReadOnlyList<string> ClickOnlyEventGestureNames { get; }
            = new List<string> { "Click" }.AsReadOnly();

        /// <summary>
        /// 判定するクリック名を取得する
        /// </summary>
        IReadOnlyList<string> GetCandidateEventGestureNames(object item, IListuiManager manager);

        // 設計メモ:
        // - item をクリックするので item が第一引数
        // - manager はほとんどのハンドラメソッドに存在し利用頻度が高いので第二引数
        // - その他付加要素なので eventGestureName は第三引数
        // - ISelectOption<,> のような追加引数 arg は第四引数
        void EventGestureConfirmed(object item, IListuiManager manager, string eventGestureName);

        // 設計メモ:
        // F2 キーで名前変更などは不可視の Subview を用いて通常のショートカットキーとして実装するため
        // このインターフェースではサポートしない
    }
}
