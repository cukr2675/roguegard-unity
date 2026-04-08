using System.Collections.Generic;

namespace Lysionium
{
    // 設計メモ: ITreeOption<out TItem, in TMgr> にしても TItem に ITreeOption が入らないと再帰できないのであまり意味がない

    public interface ITreeOption<in TMgr>
    {
        string GetName(TMgr manager);

        string GetStyle(TMgr manager);

        IReadOnlyList<object> GetChildren(TMgr manager);
    }
}
