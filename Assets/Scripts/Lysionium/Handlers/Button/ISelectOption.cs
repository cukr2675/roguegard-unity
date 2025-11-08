namespace Lysionium
{
    /// <summary>
    /// <see cref="SelectOptionViewItemHandler"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface ISelectOption
    {
        string GetName(IListMenuManager manager, IListMenuArg arg);

        string GetStyle(IListMenuManager manager, IListMenuArg arg);

        void Click(IListMenuManager manager, IListMenuArg arg);
    }
}
