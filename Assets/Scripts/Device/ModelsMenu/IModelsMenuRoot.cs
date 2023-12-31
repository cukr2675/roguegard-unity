using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IModelsMenuRoot
    {
        IModelsMenuView Get(IKeyword keyword);

        void OpenMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg, in RogueMethodArgument backArg);

        void OpenMenuAsDialog(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg, in RogueMethodArgument backArg);

        void Reopen(RogueObj self, RogueObj user, in RogueMethodArgument arg, in RogueMethodArgument backArg);

        void Done();

        void Back();

        void AddInt(IKeyword keyword, int integer);
        void AddFloat(IKeyword keyword, float number);
        void AddWork(IKeyword keyword, in RogueCharacterWork work);
        void AddObject(IKeyword keyword, object obj);
    }
}
