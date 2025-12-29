using System.Collections.Generic;

namespace Lysionium
{
    // 設計メモ: ITreeOption<out TItem, in TMgr, in TArg> にしても TItem に ITreeOption が入らないと再帰できないのであまり意味がない

    public interface ITreeOption<in TMgr, in TArg>
    {
        string GetName(TMgr manager, TArg arg);

        string GetStyle(TMgr manager, TArg arg);

        IReadOnlyList<object> GetChildren(TMgr manager, TArg arg);
    }
}
