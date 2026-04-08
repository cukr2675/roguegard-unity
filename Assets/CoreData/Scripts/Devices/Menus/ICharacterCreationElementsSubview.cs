using Lysionium;
using System.Collections.Generic;

namespace Roguegard.Device
{
    public interface ICharacterCreationElementsSubview : ISubview
    {
        ISelectOption<MMgr, MArg> LoadPresetOption { get; }

        void SetListHandler(
            IReadOnlyList<object> list, IViewItemHandler handler, MMgr manager, MArg arg, ref ISubviewStateProvider stateProvider);
    }
}
