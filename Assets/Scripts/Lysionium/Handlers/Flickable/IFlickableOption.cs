namespace Lysionium
{
    public interface IFlickableOption<in TMgr, in TArg>
    {
        string GetName(TMgr manager, TArg arg);

        string GetStyle(TMgr manager, TArg arg);

        void KeyDown(TMgr manager, TArg arg);
        void Expand(TMgr manager, TArg arg);
        void KeyUp(TMgr manager, TArg arg);
    }
}
