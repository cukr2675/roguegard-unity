using Lysionium;
using UnityEngine;

namespace Roguegard.Device
{
    public abstract class MMgr : MMgrBase, IMenuScreenListMenuManager<MMgr, MArg>
    {
        public abstract IListHandlerSubview Face { get; }
        public abstract ISummaryElementsSubview Summary { get; }
        public abstract ITextEditorElementsSubview TextEditor { get; }
        public abstract IPaintElementsSubview Paint { get; }
        public abstract IListHandlerSubview Dopesheet { get; }
        public abstract ICharacterCreationElementsSubview CharacterCreation { get; }
        public abstract IListHandlerSubview TitleMenu { get; }

        public abstract void PushMenuScreen(
            IMenuScreen<MMgrBase, MArg> menuScreen,
            RogueObj self = null,
            RogueObj user = null,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            EffectableValue value = null,
            RogueObj tool = null,
            object other = null);

        /// <summary>
        /// <see cref="MMgrBase"/> の実装は <see cref="MMgr"/> を必ず継承することを想定するため、
        /// <see cref="RogueMenuScreen"/> と合わせて安全ではないキャストを許容する
        /// </summary>
        public void PushMenuScreen(IMenuScreen<MMgr, MArg> menuScreen, MArg arg)
        {
            base.PushMenuScreen((IMenuScreen<MMgrBase, MArg>)menuScreen, arg);
        }
    }
}
