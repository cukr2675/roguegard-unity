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
    public class ObjCommandMenu : IListMenu
    {
        private readonly List<IObjCommand> commands;
        private readonly List<object> selectOptions;

        public IListMenuSelectOption Summary { get; }
        public IListMenuSelectOption Details { get; }

        public ObjCommandMenu()
        {
            commands = new List<IObjCommand>();
            selectOptions = new List<object>();

            Summary = new SummarySelectOption();
            Details = new DetailsSelectOption();
        }

        void IListMenu.OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var tool = arg.Tool;
            RoguegardSettings.ObjCommandTable.GetCommands(self, tool, commands);
            selectOptions.Clear();
            foreach (var command in commands)
            {
                selectOptions.Add(command.SelectOption);
            }
            selectOptions.Add(Details);
            selectOptions.Add(ExitListMenuSelectOption.Instance);
            manager.GetView(DeviceKw.MenuCommand).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, arg);

            var caption = manager.GetView(DeviceKw.MenuCaption);
            caption.OpenView(null, Spanning<object>.Empty, null, null, null, new(other: tool.Main.InfoSet));
        }

        private class SummarySelectOption : BaseListMenuSelectOption
        {
            public override string Name => "つよさ";

            private Menu menu = new();

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var openArg = new RogueMethodArgument(targetObj: arg.TargetObj, other: arg.Other);
                manager.OpenMenu(menu, self, null, openArg);
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }

            private class Menu : IListMenu
            {
                public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var summary = (SummaryMenuView)manager.GetView(DeviceKw.MenuSummary);
                    summary.OpenView(SelectOptionPresenter.Instance, Spanning<object>.Empty, manager, self, user, arg);
                    ExitListMenuSelectOption.OpenLeftAnchorExit(manager);

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

        private class DetailsSelectOption : IListMenuSelectOption
        {
            private static readonly Menu nextMenu = new Menu();

            string IListMenuSelectOption.GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Tool?.Main.InfoSet.Details == null) return "<#808080>説明";
                else return "説明";
            }

            void IListMenuSelectOption.Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Tool?.Main.InfoSet.Details == null)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.OpenMenu(nextMenu, self, user, arg);
            }

            private class Menu : IListMenu
            {
                private static readonly object[] elms = new object[0];

                public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var details = (DetailsMenuView)manager.GetView(DeviceKw.MenuDetails);
                    details.OpenView(SelectOptionPresenter.Instance, elms, manager, self, user, arg);
                    Debug.Log(arg.TargetObj + " , " + arg.Tool);
                    details.SetObj(arg.Tool ?? arg.TargetObj);
                    ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
                }
            }
        }
    }
}
