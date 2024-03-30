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
        public IModelsMenuChoice Close { get; }

        /// <summary>
        /// <see cref="RogueMethodArgument.TargetObj"/> のインベントリを開く
        /// </summary>
        public IModelsMenu Items { get; }

        /// <summary>
        /// <see cref="RogueMethodArgument.TargetObj"/> の足元のアイテム一覧を開く
        /// </summary>
        public IModelsMenu Ground { get; }

        public IModelsMenu PutIntoChest { get; }

        public IModelsMenu TakeOutFromChest { get; }

        public ObjsMenu(ObjCommandMenu commandMenu, PutIntoChestCommandMenu putInCommandMenu, TakeOutFromChestCommandMenu takeOutCommandMenu)
        {
            Close = new CloseChoice();
            Items = new ItemsMenu() { commandMenu = commandMenu };
            Ground = new GroundMenu() { commandMenu = commandMenu };
            PutIntoChest = new PutIntoChestMenu() { commandMenu = putInCommandMenu };
            TakeOutFromChest = new TakeOutFromChestMenu() { commandMenu = takeOutCommandMenu };
        }

        private class CloseChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Close";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Done();
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
            }
        }

        private abstract class ScrollMenu : BaseScrollModelsMenu<RogueObj>
        {
            protected virtual bool Skip0WeightObjs => false;
            protected virtual bool SortIsEnabled => false;

            public IModelsMenu commandMenu;

            private readonly ActionModelsMenuChoice sortChoice;

            private static CategorizedSortTable sortTable;

            protected ScrollMenu()
            {
                sortChoice = new ActionModelsMenuChoice(":Sort", Sort);
            }

            protected override object GetViewPositionHolder(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => arg.TargetObj;

            protected sealed override Spanning<RogueObj> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => GetObjs(self, arg.TargetObj);

            /// <summary>
            /// <paramref name="targetObj"/> のインベントリを開く
            /// </summary>
            protected abstract Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj);

            protected override float GetDefaultViewPosition(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (!Skip0WeightObjs) return 0f;

                // 重さがゼロではないアイテムまで自動スクロール
                var models = GetObjs(self, arg.TargetObj);
                for (int i = 0; i < models.Count; i++)
                {
                    var weight = WeightCalculator.Get(models[i]);
                    if (weight.TotalWeight > 0f) return i;
                }
                return 0f;
            }

            protected override void GetLeftAnchorModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, List<object> models)
            {
                if (!SortIsEnabled) return;

                // プレイヤーパーティのキャラのインベントリまたは倉庫であればソート可能
                if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(arg.TargetObj) ||
                    ChestInfo.GetStorage(arg.TargetObj) != null)
                {
                    models.Insert(0, sortChoice);
                }
            }

            protected override string GetItemName(RogueObj obj, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return obj.GetName();
            }

            protected override void ItemActivate(RogueObj obj, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // 選択したアイテムの情報と選択肢を表示する
                root.OpenMenuAsDialog(commandMenu, self, null, new(targetObj: arg.TargetObj, tool: obj), arg);
            }

            private void Sort(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                ViewPosition.Save(root, arg.TargetObj);
                root.AddObject(DeviceKw.EnqueueSE, StdKw.Sort);

                if (sortTable == null)
                {
                    sortTable = new CategorizedSortTable(RoguegardSettings.ObjCommandTable.Categories);
                }

                // ソートしたあと開きなおす
                var storageObjs = ChestInfo.GetStorage(arg.TargetObj);
                if (storageObjs != null) { sortTable.Sort(arg.TargetObj); }
                else { sortTable.Sort(arg.TargetObj); }
                root.Reopen(self, user, arg, arg);
                root.Back();
            }
        }

        private class ItemsMenu : ScrollMenu
        {
            protected override string MenuName => ":Inventory";
            protected override bool Skip0WeightObjs => true;
            protected override bool SortIsEnabled => true;

            private readonly List<RogueObj> models = new List<RogueObj>();

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                // お金を取り除いたリストを生成
                models.Clear();
                var spaceObjs = targetObj.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    if (spaceObjs[i] == null || spaceObjs[i].Main.InfoSet == RoguegardSettings.MoneyInfoSet) continue;

                    models.Add(spaceObjs[i]);
                }
                return models;
            }
        }

        private class GroundMenu : ScrollMenu
        {
            protected override string MenuName => ":Ground";

            private readonly List<RogueObj> models = new List<RogueObj>();

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                var locationObjs = targetObj.Location.Space.Objs;
                var targetPosition = targetObj.Position;
                models.Clear();
                for (int i = 0; i < locationObjs.Count; i++)
                {
                    var obj = locationObjs[i];
                    if (obj == null || obj == targetObj || obj.Position != targetPosition) continue;

                    models.Add(obj);
                }
                return models;
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
