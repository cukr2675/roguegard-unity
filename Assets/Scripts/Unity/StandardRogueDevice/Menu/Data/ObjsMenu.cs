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

        public IModelsMenu Items { get; }
        public IModelsMenu Ground { get; }
        public IModelsMenu OpenChest { get; }

        public ObjsMenu(
            CaptionWindow captionWindow, ObjCommandMenu commandMenu,
            PutIntoChestCommandMenu putInCommandMenu, TakeOutFromChestCommandMenu takeOutCommandMenu)
        {
            Close = new CloseChoice();
            {
                var items = new ItemsMenu();
                items.caption = captionWindow;
                items.commandMenu = commandMenu;
                items.action = (IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) =>
                {
                    // スクロール位置を記憶する。違うキャラのインベントリを開いたときリセットされる
                    items.selfIndex = root.Get(DeviceKw.MenuScroll).GetPosition();
                    items.prevSelf = self;
                };
                items.exitChoice = new ExitModelsMenuChoice(items.action);
                Items = items;
            }
            {
                var ground = new GroundMenu();
                ground.caption = captionWindow;
                ground.commandMenu = commandMenu;
                ground.action = (IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) =>
                {
                    // スクロール位置を記憶する。違うキャラのインベントリを開いたときリセットされる
                    ground.selfIndex = root.Get(DeviceKw.MenuScroll).GetPosition();
                    ground.prevSelf = self;
                };
                ground.exitChoice = new ExitModelsMenuChoice(ground.action);
                ground.closeChoice = Close;
                Ground = ground;
            }
            {
                var openChest = new OpenChestMenu();

                var putIn = new PutIntoChestMenu();
                putIn.caption = captionWindow;
                putIn.commandMenu = putInCommandMenu;
                putIn.action = (IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) =>
                {
                    // スクロール位置を記憶する。違うキャラのインベントリを開いたときリセットされる
                    openChest.selfIndex = putIn.selfIndex = root.Get(DeviceKw.MenuScroll).GetPosition();
                    openChest.prevSelf = self;
                };

                var takeOut = new PutIntoChestMenu();
                takeOut.caption = captionWindow;
                takeOut.commandMenu = takeOutCommandMenu;
                takeOut.action = (IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) =>
                {
                    // スクロール位置を記憶する。違うキャラのインベントリを開いたときリセットされる
                    openChest.selfIndex = takeOut.selfIndex = root.Get(DeviceKw.MenuScroll).GetPosition();
                    openChest.prevSelf = self;
                };

                openChest.putInMenu = putIn;
                openChest.takeOutMenu = takeOut;
                openChest.exitChoice = new ExitModelsMenuChoice(putIn.action);
                openChest.closeChoice = Close;
                OpenChest = openChest;
            }
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

        private class ItemsMenu : MenuItemController, IModelsMenu
        {
            private readonly List<RogueObj> models = new List<RogueObj>();
            public IModelsMenuChoice exitChoice;
            private SortChoice sortChoice;

            public RogueObj prevSelf;

            void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                models.Clear();
                var spaceObjs = arg.TargetObj.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    if (spaceObjs[i] == null || spaceObjs[i].Main.InfoSet == RoguegardSettings.MoneyInfoSet) continue;

                    models.Add(spaceObjs[i]);
                }

                float position;
                if (arg.TargetObj == prevSelf)
                {
                    position = selfIndex;
                }
                else
                {
                    // 重さがゼロではないアイテムまで自動スクロール
                    int index;
                    for (index = 0; index < models.Count; index++)
                    {
                        var weight = WeightCalculator.Get(models[index]);
                        if (weight.TotalWeight > 0f) break;
                    }
                    position = index;
                }
                var openArg = new RogueMethodArgument(targetObj: arg.TargetObj, vector: new Vector2(position, 0f));
                var scroll = (ScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, Spanning<RogueObj>.Create(models), root, self, user, openArg);
                scroll.ShowExitButton(exitChoice);

                // プレイヤーパーティのインベントリまたは箱であればソート可能
                if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(arg.TargetObj) || arg.TargetObj.Main.InfoSet.Category == CategoryKw.Chest)
                {
                    sortChoice ??= new SortChoice() { parent = this };
                    scroll.ShowSortButton(sortChoice);
                }
            }
        }

        private class GroundMenu : MenuItemController, IModelsMenu
        {
            public IModelsMenuChoice exitChoice;
            public IModelsMenuChoice closeChoice;

            public RogueObj prevSelf;

            private readonly List<object> groundObjs = new List<object>();

            void IModelsMenu.OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var locationObjs = arg.TargetObj.Location.Space.Objs;
                var targetPosition = arg.TargetObj.Position;
                groundObjs.Clear();
                for (int i = 0; i < locationObjs.Count; i++)
                {
                    var obj = locationObjs[i];
                    if (obj == null || obj == arg.TargetObj || obj.Position != targetPosition) continue;

                    groundObjs.Add(obj);
                }

                var position = arg.TargetObj == prevSelf ? selfIndex : 0f;
                var openArg = new RogueMethodArgument(targetObj: arg.TargetObj, vector: new Vector2(position, 0f));
                var scroll = (ScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, groundObjs, root, self, user, openArg);

                if (arg.Count == 1) { scroll.ShowExitButton(closeChoice); }
                else { scroll.ShowExitButton(exitChoice); }
            }
        }

        private class PutIntoChestMenu : MenuItemController
        {
        }

        private class OpenChestMenu : IModelsMenu
        {
            public PutIntoChestMenu putInMenu;
            public PutIntoChestMenu takeOutMenu;

            public IModelsMenuChoice exitChoice;
            public IModelsMenuChoice closeChoice;
            private SortChoice sortChoice;

            public RogueObj prevSelf;
            public float selfIndex;

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var chest = arg.TargetObj;
                var storageObjs = ChestInfo.GetStorage(chest);
                RogueObj targetObj;
                if (arg.Count == 1) { targetObj = self; }
                else { targetObj = chest; }

                var models = targetObj.Space.Objs;
                if (targetObj == chest && storageObjs != null) { models = storageObjs; }
                float position;
                if (targetObj == prevSelf)
                {
                    position = selfIndex;
                }
                else
                {
                    int index;
                    for (index = 0; index < models.Count; index++)
                    {
                        var weight = WeightCalculator.Get(models[index]);
                        if (weight.TotalWeight > 0f) break;
                    }
                    position = index;
                }
                var openArg = new RogueMethodArgument(targetObj: chest, vector: new Vector2(position, 0f));
                var scroll = (ScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                if (arg.Count == 1) { scroll.OpenView(putInMenu, models, root, self, user, openArg); }
                else { scroll.OpenView(takeOutMenu, models, root, self, user, openArg); }
                scroll.ShowExitButton(closeChoice);

                // プレイヤーパーティのインベントリまたは箱であればソート可能
                if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(targetObj) || targetObj.Main.InfoSet.Category == CategoryKw.Chest)
                {
                    sortChoice ??= new SortChoice() { parent = takeOutMenu };
                    scroll.ShowSortButton(sortChoice);
                }
            }
        }

        private class MenuItemController : IModelsMenuItemController
        {
            public IModelsMenu commandMenu;
            public ModelsMenuAction action;
            public CaptionWindow caption;
            public float selfIndex;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var obj = (RogueObj)model;
                return obj?.GetName();
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                action(root, self, user, arg);
                var obj = (RogueObj)model;
                if (obj == null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                caption.Log(obj.Main.InfoSet);
                var openArg = new RogueMethodArgument(targetObj: arg.TargetObj, tool: obj);
                var backArg = new RogueMethodArgument(targetObj: arg.TargetObj);
                root.OpenMenuAsDialog(commandMenu, self, null, openArg, backArg);
            }
        }

        private class SortChoice : IModelsMenuChoice
        {
            public MenuItemController parent;
            private static readonly OtherKeyword other = new OtherKeyword();
            private static readonly Comparer comparer = new Comparer();
            private Dictionary<IKeyword, RogueObjList[]> categorizedBufferTable;
            private static readonly RogueObjList mainBuffer = new RogueObjList();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ":Sort";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                Debug.LogError(root.Get(DeviceKw.MenuScroll).GetPosition());

                root.AddObject(DeviceKw.EnqueueSE, StdKw.Sort);

                if (categorizedBufferTable == null)
                {
                    categorizedBufferTable = new Dictionary<IKeyword, RogueObjList[]>();
                    var categories = RoguegardSettings.ObjCommandTable.Categories;
                    for (int i = 0; i < categories.Count; i++)
                    {
                        categorizedBufferTable.Add(categories[i], new[] { new RogueObjList(), new RogueObjList(), new RogueObjList(), new RogueObjList() });
                    }
                    categorizedBufferTable.Add(other, new[] { new RogueObjList(), new RogueObjList(), new RogueObjList(), new RogueObjList() });
                }

                foreach (var pair in categorizedBufferTable)
                {
                    foreach (var listItem in pair.Value)
                    {
                        listItem.Clear();
                    }
                }

                // アイテムを並べ替える
                var objs = arg.TargetObj.Space.Objs;
                for (int i = 0; i < objs.Count; i++)
                {
                    var obj = objs[i];
                    if (obj == null) continue;

                    if (obj.Main.InfoSet.Category == null || !categorizedBufferTable.TryGetValue(obj.Main.InfoSet.Category, out var categorizedBuffers))
                    {
                        categorizedBuffers = categorizedBufferTable[other];
                    }
                    var weight = WeightCalculator.Get(obj);
                    var equipmentInfo = obj.Main.GetEquipmentInfo(obj);
                    var vehicleInfo = VehicleInfo.Get(obj);
                    var equipped = equipmentInfo?.EquipIndex >= 0 || vehicleInfo?.Rider != null;

                    // 重さゼロのアイテムを上に並べる
                    if (weight.TotalWeight <= 0f)
                    {
                        // その中でも装備済みのアイテムを最も上に並べる
                        if (equipped) { categorizedBuffers[0].Add(obj); }
                        else { categorizedBuffers[1].Add(obj); }
                    }
                    else
                    {
                        // 装備済みのアイテムを上に並べる
                        if (equipped) { categorizedBuffers[2].Add(obj); }
                        else { categorizedBuffers[3].Add(obj); }
                    }
                }

                // 同じクラスのアイテムは名前順で並べる


                // バッファをもとに並べ替える
                mainBuffer.Clear();
                foreach (var pair in categorizedBufferTable)
                {
                    foreach (var listItem in pair.Value)
                    {
                        listItem.Sort(comparer);
                        for (int i = 0; i < listItem.Count; i++)
                        {
                            mainBuffer.Add(listItem[i]);
                        }
                    }
                }
                arg.TargetObj.Space.Sort(mainBuffer);

                // ビューを更新
                parent.action(root, self, user, arg);
                var openArg = new RogueMethodArgument(targetObj: arg.TargetObj, vector: new Vector2(parent.selfIndex, 0f));
                root.Reopen(self, user, arg, openArg);
                root.Back();
            }

            private class OtherKeyword : IKeyword
            {
                public string Name => null;
                public Sprite Icon => null;
                public Color Color => default;
                public string Caption => null;
                public IRogueDetails Details => null;
            }

            private class Comparer : IComparer<RogueObj>
            {
                public int Compare(RogueObj x, RogueObj y)
                {
                    if (x == null || y == null) return 0;

                    return string.CompareOrdinal(x.Main.InfoSet.Name, y.Main.InfoSet.Name);
                }
            }
        }
    }
}
