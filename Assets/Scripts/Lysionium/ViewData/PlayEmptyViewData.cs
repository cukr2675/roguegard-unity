using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// ビューを変更せずに LUI Play を再生するためのクラス
    /// </summary>
    public class PlayEmptyViewData
    {
        public string EmptySubviewName { get; set; } = StandardSubviewTable.ScrollName;

        public void Play(string value, IListMenuManager manager)
        {
            if (value == null) throw new System.ArgumentNullException(nameof(value));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            if (manager.GetSubview(EmptySubviewName) is not Subview subview) throw new System.InvalidOperationException(
                $"{EmptySubviewName} の Subview は {nameof(Subview)} ではありません。");

            subview.PlayString(value);
        }

        public void Play(Object value, IListMenuManager manager)
        {
            if (value == null) throw new System.ArgumentNullException(nameof(value));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            if (manager.GetSubview(EmptySubviewName) is not Subview subview) throw new System.InvalidOperationException(
                $"{EmptySubviewName} の Subview は {nameof(Subview)} ではありません。");

            subview.PlayObject(value);
        }
    }
}
