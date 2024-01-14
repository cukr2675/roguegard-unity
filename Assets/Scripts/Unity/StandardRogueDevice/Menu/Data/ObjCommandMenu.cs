using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// <see cref="RogueObj"/> に対するコマンド（食べる・投げるなど）を選択するメニュー。
    /// </summary>
    public class ObjCommandMenu : IModelsMenu
    {
        private readonly List<IObjCommand> commands;
        private readonly List<object> choices;

        public IModelsMenuChoice Summary { get; }
        public IModelsMenuChoice Details { get; }

        public ObjCommandMenu()
        {
            commands = new List<IObjCommand>();
            choices = new List<object>();

            Summary = new SummaryChoice();
            Details = new DetailsChoice();
        }

        void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var tool = arg.Tool;
            RoguegardSettings.ObjCommandTable.GetCommands(self, tool, commands);
            choices.Clear();
            foreach (var command in commands)
            {
                choices.Add(command.ModelsMenuChoice);
            }
            choices.Add(Details);
            choices.Add(ExitModelsMenuChoice.Instance);
            root.Get(DeviceKw.MenuCommand).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, arg);
        }

        private class SummaryChoice : IModelsMenuChoice
        {
            private Menu menu = new Menu();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "つよさ";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: arg.TargetObj, other: arg.Other);
                root.OpenMenu(menu, self, null, openArg, arg);
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }

            private class Menu : IModelsMenu
            {
                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var summary = (SummaryMenuView)root.Get(DeviceKw.MenuSummary);
                    summary.OpenView(ChoicesModelsMenuItemController.Instance, Spanning<object>.Empty, root, self, user, arg);
                    summary.ShowExitButton(ExitModelsMenuChoice.Instance);

                    if (arg.TargetObj != null)
                    {
                        summary.SetObj(arg.TargetObj);
                    }
                    else if (arg.Other is IRogueTile tile)
                    {
                        summary.SetTile(tile);
                    }
                    else
                    {
                        summary.SetOther(arg);
                    }
                }
            }
        }

        private class DetailsChoice : IModelsMenuChoice
        {
            private static readonly Menu nextMenu = new Menu();

            string IModelsMenuChoice.GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Tool?.Main.InfoSet.Details == null) return "<#808080>説明";
                else return "説明";
            }

            void IModelsMenuChoice.Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Tool?.Main.InfoSet.Details == null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, self, user, arg, arg);
            }

            private class Menu : IModelsMenu
            {
                private static readonly object[] models = new object[0];

                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var details = (DetailsMenuView)root.Get(DeviceKw.MenuDetails);
                    details.OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, user, arg);
                    Debug.Log(arg.TargetObj + " , " + arg.Tool);
                    details.SetObj(arg.Tool ?? arg.TargetObj);
                    details.ShowExitButton(ExitModelsMenuChoice.Instance);
                }
            }
        }
    }
}
