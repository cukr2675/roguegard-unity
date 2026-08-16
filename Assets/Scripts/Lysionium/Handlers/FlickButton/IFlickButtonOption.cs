namespace Lysionium
{
    public interface IFlickButtonOption<in TMgr> : ISelectOption<TMgr>
    {
        void Press(TMgr manager);
        void Expand(TMgr manager);
        void Release(TMgr manager);
    }
}
