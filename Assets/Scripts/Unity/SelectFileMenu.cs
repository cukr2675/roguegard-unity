using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    public class SelectFileMenu : IModelsMenu
    {
        private readonly ScrollModelsMenuView scrollMenuView;

        private readonly List<object> objs = new List<object>();

        private static readonly ItemController itemController = new ItemController();

        public delegate void SelectCallback(IModelsMenuRoot root, string path);
        public delegate void AddCallback(IModelsMenuRoot root);

        public SelectFileMenu(ScrollModelsMenuView scrollMenuView)
        {
            this.scrollMenuView = scrollMenuView;
        }

        public void Open(SelectCallback selectCallback, AddCallback addCallback = null)
        {
            SetCallback(selectCallback, addCallback);
            OpenMenu(null, null, null, RogueMethodArgument.Identity);
        }

        public void SetCallback(SelectCallback selectCallback, AddCallback addCallback = null)
        {
            itemController.selectCallback = selectCallback;
            itemController.addCallback = addCallback;
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (!Directory.Exists(StandardRogueDeviceSave.RootDirectory))
            {
                Directory.CreateDirectory(StandardRogueDeviceSave.RootDirectory);
            }

            var filesArg = arg;
            StandardRogueDeviceSave.GetFiles(files =>
            {
                objs.Clear();
                if (itemController.addCallback != null) { objs.Add(null); }
                objs.AddRange(files);
                scrollMenuView.OpenView(itemController, objs, root, self, user, filesArg);
                scrollMenuView.ShowExitButton(ExitModelsMenuChoice.Instance);
                scrollMenuView.SetPosition(1f);
            });
        }

        private class ItemController : IModelsMenuItemController
        {
            public SelectCallback selectCallback;
            public AddCallback addCallback;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return "+ New File";
                return (string)model;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) { addCallback(root); }
                else { selectCallback(root, (string)model); }
            }
        }
    }
}
