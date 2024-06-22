using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public IListMenu Items { get; }

        /// <summary>
        /// <see cref="RogueMethodArgument.TargetObj"/> の足元のアイテム一覧を開く
        /// </summary>
        public IListMenu Ground { get; }

        public IListMenu PutIntoChest { get; }

        public IListMenu TakeOutFromChest { get; }

        public ObjsMenu(ObjCommandMenu commandMenu, PutIntoChestCommandMenu putInCommandMenu, TakeOutFromChestCommandMenu takeOutCommandMenu)
        {
            Close = new CloseSelectOption();
            Items = new ItemsMenu() { commandMenu = commandMenu };
            Ground = new GroundMenu() { commandMenu = commandMenu };
            PutIntoChest = new PutIntoChestMenu() { commandMenu = putInCommandMenu };
            TakeOutFromChest = new TakeOutFromChestMenu() { commandMenu = takeOutCommandMenu };
        }

        private class CloseSelectOption : BaseListMenuSelectOption
        {
            public override string Name => ":Close";

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.Done();
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
            }
        }

        private abstract class ScrollMenu : BaseScrollListMenu<RogueObj>
        {
            protected virtual bool Skip0WeightObjs => false;
            protected virtual bool SortIsEnabled => false;

            public IListMenu commandMenu;

            private readonly ActionListMenuSelectOption sortSelectOption;

            private static CategorizedSortTable sortTable;

            protected ScrollMenu()
            {
                sortSelectOption = new ActionListMenuSelectOption(":Sort", Sort);
            }

            protected override object GetViewPositionHolder(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => arg.TargetObj;

            protected sealed override Spanning<RogueObj> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => GetObjs(self, arg.TargetObj);

            /// <summary>
            /// <paramref name="targetObj"/> のインベントリを開く
            /// </summary>
            protected abstract Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj);

            protected override float GetDefaultViewPosition(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (!Skip0WeightObjs) return 0f;

                // 重さがゼロではないアイテムまで自動スクロール
                var objs = GetObjs(self, arg.TargetObj);
                for (int i = 0; i < objs.Count; i++)
                {
                    var weight = WeightCalculator.Get(objs[i]);
                    if (weight.TotalWeight > 0f) return i;
                }
                return 0f;
            }

            protected override void GetLeftAnchorList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, List<object> list)
            {
                if (!SortIsEnabled) return;

                // プレイヤーパーティのキャラのインベントリまたは倉庫であればソート可能
                if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(arg.TargetObj) ||
                    ChestInfo.GetStorage(arg.TargetObj) != null)
                {
                    list.Insert(0, sortSelectOption);
                }
            }

            protected override string GetItemName(RogueObj obj, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return obj.GetName();
            }

            protected override void ActivateItem(RogueObj obj, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // 選択したアイテムの情報と選択肢を表示する
                manager.OpenMenuAsDialog(commandMenu, self, null, new(targetObj: arg.TargetObj, tool: obj));
            }

            private void Sort(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                ViewPosition.Save(manager, arg.TargetObj);
                manager.AddObject(DeviceKw.EnqueueSE, StdKw.Sort);

                if (sortTable == null)
                {
                    sortTable = new CategorizedSortTable(RoguegardSettings.ObjCommandTable.Categories);
                }

                // ソートしたあと開きなおす
                var storageObjs = ChestInfo.GetStorage(arg.TargetObj);
                if (storageObjs != null) { sortTable.Sort(arg.TargetObj); }
                else { sortTable.Sort(arg.TargetObj); }
                manager.Reopen();
            }
        }

        private class ItemsMenu : ScrollMenu
        {
            protected override string MenuName => ":Inventory";
            protected override bool Skip0WeightObjs => true;
            protected override bool SortIsEnabled => true;

            private readonly List<RogueObj> objs = new List<RogueObj>();

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
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
            protected override string MenuName => ":Ground";

            private readonly List<RogueObj> objs = new List<RogueObj>();

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
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
            protected override string MenuName => ":Put in what?";

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                return self.Space.Objs;
            }
        }

        private class TakeOutFromChestMenu : ScrollMenu
        {
            protected override string MenuName => ":Take out what?";
            protected override bool SortIsEnabled => true;

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj chest)
            {
                var storageObjs = ChestInfo.GetStorage(chest);
                if (storageObjs != null) return storageObjs;
                else return chest.Space.Objs;
            }
        }
    }
}
