using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public static class ModelsMenuViewExtension
    {
        public static void OpenView(
            this IModelsMenuView view, IModelsMenuItemController itemController, Spanning<object> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            view.OpenView(itemController, models, root, self, user, arg);
        }
    }
}
