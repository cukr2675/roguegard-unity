using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface IModelsMenuRoot
    {
        IModelListView Get(IKeyword keyword);

        void OpenMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void OpenMenuAsDialog(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void Reopen(RogueObj self, RogueObj user, in RogueMethodArgument arg);

        void Done();

        void Back();

        void AddInt(IKeyword keyword, int integer);
        void AddFloat(IKeyword keyword, float number);
        void AddWork(IKeyword keyword, in RogueCharacterWork work);
        void AddObject(IKeyword keyword, object obj);
    }
}
