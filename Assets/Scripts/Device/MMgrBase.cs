using Lysionium;

namespace Roguegard.Device
{
    public abstract class MMgrBase : StandardListMenuManager<MMgrBase, MArg>
    {
        /// <summary>
        /// メニュー画面をすべて閉じる
        /// </summary>
        public abstract void Done();

        public abstract void ResetDone();

        public abstract void AddInt(IKeyword keyword, int integer);
        public abstract void AddFloat(IKeyword keyword, float number);
        public abstract void AddObject(IKeyword keyword, object obj);
    }
}
