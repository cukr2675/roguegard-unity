namespace Lysionium
{
    public interface IFlickableOption<in TMgr>
    {
        string GetName(TMgr manager);

        string GetStyle(TMgr manager);

        void KeyDown(TMgr manager);
        void Expand(TMgr manager);
        void KeyUp(TMgr manager);
    }
}
