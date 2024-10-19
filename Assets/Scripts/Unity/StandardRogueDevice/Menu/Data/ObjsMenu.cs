using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// アイテム一覧を表示するメニュー。
    /// </summary>
    public class ObjsMenu
    {
        public IListMenuSelectOption Close { get; }

        /// <summary>
        /// <see cref="RogueMethodArgument.TargetObj"/> のインベントリを開く
        /// </summary>
        public RogueMenuScreen Items { get; }

        /// <summary>
        /// <see cref="RogueMethodArgument.TargetObj"/> の足元のアイテム一覧を開く
        /// </summary>
        public RogueMenuScreen Ground { get; }

        public RogueMenuScreen PutIntoChest { get; }

        public RogueMenuScreen TakeOutFromChest { get; }

        public ObjsMenu(ObjCommandMenu commandMenu, PutIntoChestCommandMenu putInCommandMenu, TakeOutFromChestCommandMenu takeOutCommandMenu)
        {
            Close = ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Close", (manager, arg) => manager.Done());
            Items = new ItemsMenu() { commandMenu = commandMenu };
            Ground = new GroundMenu() { commandMenu = commandMenu };
            PutIntoChest = new PutIntoChestMenu() { commandMenu = putInCommandMenu };
            TakeOutFromChest = new TakeOutFromChestMenu() { commandMenu = takeOutCommandMenu };
        }

        private abstract class ScrollMenu : RogueMenuScreen
        {
            protected abstract string Title { get; }
            protected virtual bool Skip0WeightObjs => false;
            protected virtual bool SortIsEnabled => false;

            public RogueMenuScreen commandMenu;

            private readonly ActionListMenuSelectOption sortSelectOption;

            private static CategorizedSortTable sortTable;

            private readonly ScrollViewTemplate<RogueObj, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };


            protected ScrollMenu()
            {
                view.Title = Title;
                if (SortIsEnabled)
                {
                    view.BackAnchorList = new IListMenuSelectOption[]
                    {
                        ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(":Sort", Sort),
                        ExitListMenuSelectOption.Instance
                    };
                }
            }

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var list = GetObjs(arg.Self, arg.Arg.TargetObj);
                var viewStateHolder = GetViewStateHolder(manager, arg);

                view.Show(list, manager, arg, viewStateHolder)
                    ?.ElementNameGetter((obj, manager, arg) =>
                    {
                        return obj.GetName();
                    })
                    .OnClickElement((obj, manager, arg) =>
                    {
                        // 選択したアイテムの情報と選択肢を表示する
                        manager.PushMenuScreen(commandMenu, arg.Self, null, targetObj: arg.Arg.TargetObj, tool: obj);
                    })
                    .Build();
            }

            protected virtual object GetViewStateHolder(RogueMenuManager manager, ReadOnlyMenuArg arg)
                => arg.Arg.TargetObj;

            protected abstract List<RogueObj> GetObjs(RogueObj self, RogueObj targetObj);

            protected virtual float GetDefaultViewPosition(RogueMenuManager manager, ReadOnlyMenuArg arg)
            {
                if (!Skip0WeightObjs) return 0f;

                // 重さがゼロではないアイテムまで自動スクロール
                var objs = GetObjs(arg.Self, arg.Arg.TargetObj);
                for (int i = 0; i < objs.Count; i++)
                {
                    var weight = WeightCalculator.Get(objs[i]);
                    if (weight.TotalWeight > 0f) return i;
                }
                return 0f;
            }

            private void Sort(RogueMenuManager manager, ReadOnlyMenuArg arg)
            {
                //manager.AddObject(DeviceKw.EnqueueSE, StdKw.Sort);

                if (sortTable == null)
                {
                    sortTable = new CategorizedSortTable(RoguegardSettings.ObjCommandTable.Categories);
                }

                // ソートしたあと開きなおす
                var storageObjs = ChestInfo.GetStorage(arg.Arg.TargetObj);
                if (storageObjs != null) { sortTable.Sort(arg.Arg.TargetObj); }
                else { sortTable.Sort(arg.Arg.TargetObj); }
                manager.Reopen();
            }
        }

        private class ItemsMenu : ScrollMenu
        {
            protected override string Title => ":Inventory";
            protected override bool Skip0WeightObjs => true;
            protected override bool SortIsEnabled => true;

            private readonly List<RogueObj> objs = new();

            protected override List<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                // お金を取り除いたリストを生成
                objs.Clear();
                var spaceObjs = targetObj.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    if (spaceObjs[i] == null || spaceObjs[i].Main.InfoSet.Equals(RoguegardSettings.MoneyInfoSet)) continue;

                    objs.Add(spaceObjs[i]);
                }
                return objs;
            }
        }

        private class GroundMenu : ScrollMenu
        {
            protected override string Title => ":Ground";

            private readonly List<RogueObj> objs = new();

            protected override List<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                var locationObjs = targetObj.Location.Space.Objs;
                var targetPosition = targetObj.Position;
                objs.Clear();
                for (int i = 0; i < locationObjs.Count; i++)
                {
                    var obj = locationObjs[i];
                    if (obj == null || obj == targetObj || obj.Position != targetPosition) continue;

                    objs.Add(obj);
                }
                return objs;
            }
        }

        private class PutIntoChestMenu : ScrollMenu
        {
            protected override string Title => ":Put in what?";

            private readonly List<RogueObj> objs = new();

            protected override List<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                objs.Clear();
                var spaceObjs = self.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    objs.Add(spaceObjs[i]);
                }
                return objs;
            }
        }

        private class TakeOutFromChestMenu : ScrollMenu
        {
            protected override string Title => ":Take out what?";
            protected override bool SortIsEnabled => true;

            private readonly List<RogueObj> objs = new();

            protected override List<RogueObj> GetObjs(RogueObj self, RogueObj chest)
            {
                var storageObjs = ChestInfo.GetStorage(chest);
                if (storageObjs != null)
                {
                    objs.Clear();
                    for (int i = 0; i < storageObjs.Count; i++)
                    {
                        objs.Add(storageObjs[i]);
                    }
                    return objs;
                }
                else
                {
                    objs.Clear();
                    for (int i = 0; i < chest.Space.Objs.Count; i++)
                    {
                        objs.Add(chest.Space.Objs[i]);
                    }
                    return objs;
                }
            }
        }
    }
}
