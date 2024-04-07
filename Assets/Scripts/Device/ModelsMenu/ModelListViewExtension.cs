using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public static class ModelListViewExtension
    {
        public static void OpenView(
            this IModelListView view, IModelListPresenter presenter, Spanning<object> models,
            IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            view.OpenView(presenter, models, root, self, user, arg);
        }
    }
}
