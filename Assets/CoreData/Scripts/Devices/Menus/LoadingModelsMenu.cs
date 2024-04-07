using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class LoadingModelsMenu : IModelsMenu
    {
        private readonly object[] models;

        public LoadingModelsMenu(string text, string buttonText, ModelsMenuAction buttonAction, ModelsMenuAction updateAction = null)
        {
            models = new object[]
            {
                new ActionModelsMenuChoice(text, updateAction ?? Wait),
                new ActionModelsMenuChoice(buttonText, buttonAction)
            };
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuLoading).OpenView(ChoiceListPresenter.Instance, models, root, self, user, arg);
        }

        private static void Wait(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
        }
    }
}
