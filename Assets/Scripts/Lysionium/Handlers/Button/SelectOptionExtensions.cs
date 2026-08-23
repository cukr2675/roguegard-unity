namespace Lysionium
{
    public static class SelectOptionExtensions
    {
        public static void Click<TMgr>(this ISelectOption<TMgr> selectOption, TMgr manager)
            => selectOption.EventGestureConfirmed(manager, "Click");

        public static void Click<TMgr, TArg>(this ISelectOption<TMgr, TArg> selectOption, TMgr manager, TArg arg)
            => selectOption.EventGestureConfirmed(manager, "Click", arg);
    }
}
