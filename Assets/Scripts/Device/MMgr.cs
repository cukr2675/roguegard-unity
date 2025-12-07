using Lysionium;
using UnityEngine;

namespace Roguegard.Device
{
    public abstract class MMgr : StandardListMenuManager<MMgr, MArg>
    {
        public abstract void PushMenuScreen(
            IMenuScreen<MMgr, MArg> menuScreen,
            RogueObj self = null,
            RogueObj user = null,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            EffectableValue value = null,
            RogueObj tool = null,
            object other = null);

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
