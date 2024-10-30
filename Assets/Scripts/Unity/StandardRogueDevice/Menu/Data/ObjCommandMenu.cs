using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// <see cref="RogueObj"/> に対するコマンド（食べる・投げるなど）を選択するメニュー。
    /// </summary>
    public class ObjCommandMenu : RogueMenuScreen
    {
        private readonly List<IObjCommand> commands;
        private readonly List<IListMenuSelectOption> selectOptions;
        private readonly SummaryMenuScreen summaryMenuScreen = new();
        private readonly DetailsMenuScreen detailsMenuScreen = new();

        public IListMenuSelectOption Summary { get; }
        public IListMenuSelectOption Details { get; }

        private readonly CommandListViewTemplate<IListMenuSelectOption, RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public ObjCommandMenu()
        {
            commands = new List<IObjCommand>();
            selectOptions = new List<IListMenuSelectOption>();

            Summary = ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("つよさ", (manager, arg) =>
            {
                manager.PushMenuScreen(summaryMenuScreen, arg.Self, targetObj: arg.Arg.TargetObj, other: arg.Arg.Other);
            });
            Details = ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>("説明", (manager, arg) =>
            {
                manager.PushMenuScreen(detailsMenuScreen, arg);
            });
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            var tool = arg.Arg.Tool;
            RoguegardSettings.ObjCommandTable.GetCommands(arg.Self, tool, commands);
            selectOptions.Clear();
            foreach (var command in commands)
            {
                selectOptions.Add(command.SelectOption);
            }
            selectOptions.Add(Details);
            selectOptions.Add(ExitListMenuSelectOption.Instance);
            view.ShowTemplate(selectOptions, manager, arg)
                ?
                .ElementNameFrom((selectOption, manager, arg) =>
                {
                    return selectOption.GetName(manager, arg);
                })

                .OnClickElement((selectOption, manager, arg) =>
                {
                    selectOption.HandleClick(manager, arg);
                })

                .Build();
        }

        private class SummaryMenuScreen : RogueMenuScreen
        {
            private readonly DialogViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                object target;
                if (arg.Arg.TargetObj != null)
                {
                    target = arg.Arg.TargetObj;
                }
                else if (arg.Arg.Other is IRogueTile tile)
                {
                    target = tile;
                }
                else
                {
                    target = arg;
                }

                view.ShowTemplate(target?.ToString(), manager, arg)
                    ?.Build();
            }
        }

        private class DetailsMenuScreen : RogueMenuScreen
        {
            private readonly DialogViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var details = (arg.Arg.Tool ?? arg.Arg.TargetObj).Main.InfoSet.Details?.ToString();

                view.ShowTemplate(details, manager, arg)
                    ?.Build();
            }
        }
    }
}
