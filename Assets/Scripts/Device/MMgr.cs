using Lysionium;
using UnityEngine;

namespace Roguegard.Device
{
    public abstract class MMgr : StandardListMenuManager<MMgr, MArg>
    {
        public abstract IListHandlerSubview CharacterCreation { get; }
        public abstract IListHandlerSubview TitleMenu { get; }

        public abstract void PushMenuScreen(
            MenuScreen<MMgr, MArg> menuScreen,
            RogueObj self = null,
            RogueObj user = null,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            EffectableValue value = null,
            RogueObj tool = null,
            object other = null);

        public abstract void AddInt(IKeyword keyword, int integer);
        public abstract void AddFloat(IKeyword keyword, float number);
        public abstract void AddObject(IKeyword keyword, object obj);
    }
}
