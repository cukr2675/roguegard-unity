using System.Collections.Generic;

namespace Lysionium
{
    // 設計メモ: Lysionium.Modeler のハードメニューが IReadOnlyList<ISelectOption> だと TMgr, TArg の型がわからないので型引数をつける
    // ItemEventGestureConfirmedHandler に型引数があって ISelectOption に無いのは変
    // また、複数の IListuiManager で共通のメニュー（クイックメニューなど）を作りたい場合、反変性があると便利なので付与する

    /// <summary>
    /// <see cref="SelectOptionViewItemHandler{TMgr}"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface ISelectOption<in TMgr>
    {
        string GetName(TMgr manager);

        string GetStyle(TMgr manager);

        IReadOnlyList<string> GetCandidateEventGestureNames(TMgr manager);

        void EventGestureConfirmed(TMgr manager, string eventGestureName);
    }

    public interface ISelectOption<in TMgr, in TArg>
    {
        string GetName(TMgr manager, TArg arg);

        string GetStyle(TMgr manager, TArg arg);

        IReadOnlyList<string> GetCandidateEventGestureNames(TMgr manager, TArg arg);

        void EventGestureConfirmed(TMgr manager, string eventGestureName, TArg arg);
    }
}
