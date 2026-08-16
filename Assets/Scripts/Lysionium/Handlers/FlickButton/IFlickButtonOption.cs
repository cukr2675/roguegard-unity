namespace Lysionium
{
    public interface IFlickButtonOption<in TMgr>
    {
        string GetName(TMgr manager);

        string GetStyle(TMgr manager);

        void KeyDown(TMgr manager);
        void Expand(TMgr manager);
        void KeyUp(TMgr manager);
    }
}
