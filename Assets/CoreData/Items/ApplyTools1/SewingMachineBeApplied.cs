using Lysionium;
using OchalikeSprites;
using Roguegard.Device;
using RuntimeDotter;
using TMPro;
using UnityEngine;

namespace Roguegard
{
    public class SewingMachineBeApplied : BaseApplyRogueMethod
    {
        private static readonly Menu menu = new();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            RogueDevice.Primary.AddMenu(menu, user, null, RogueMethodArgument.Identity);
            return false;
        }

        private class Menu : RogueMenuScreen
        {
            private readonly ScrollMenuViewData<RogueObj, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show(arg.Self.Space.Objs, manager, arg)
                    ?
                    .Filter(obj => obj != null)

                    .NameFrom((item, manager, arg) =>
                    {
                        if (item == null) return "+ 新しく作る";
                        else return item.GetName();
                    })

                    .VarOnce(out var nextScreen, new SewingScreen())
                    .OnClick((item, manager, arg) =>
                    {
                        if (item.Main.BaseInfoSet is SewedEquipmentInfoSet infoSet)
                        {
                            // 保存せず終了できるように複製する
                            var data = infoSet.GetDataClone();
                            manager.PushMenuScreen(nextScreen, arg.Self, other: data, targetObj: item);
                        }
                    })

                    .Tail.Option("+ 新しく作る", (manager, arg) =>
                    {
                        // 装備品を新規作成する場合はデータクラスを生成する
                        var data = new SewedEquipmentData();
                        for (int i = 0; i < RoguegardSettings.DefaultPalette.Length; i++)
                        {
                            data.BoneSprites.SetPalette(i, RoguegardSettings.DefaultPalette[i]);
                        }
                        data.BoneSprites.MainColor = Color.white;
                        manager.PushMenuScreen(nextScreen, arg.Self, other: data, targetObj: null);
                    })

                    .Build();
            }
        }

        private class SewingScreen : RogueMenuScreen
        {
            private readonly ScrollMenuViewData<IPaintBoneSprite, MMgr, MArg> view = new()
            {
                ScrollSubviewSelector = m => m.Widgets,
                BackAnchorList = new(_ => _.Option(":Back", ChoicesMenuScreen.SaveBackDialog(Save))),
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var data = (SewedEquipmentData)arg.Arg.Other;

                view.Show(data.BoneSprites.Items, manager, arg)
                    ?
                    .VarOnce(out var colorPicker, new ColorPickerMenuScreen<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            var data = (SewedEquipmentData)arg.Arg.Other;
                            return data.BoneSprites.MainColor;
                        },
                        (manager, arg, color) =>
                        {
                            var data = (SewedEquipmentData)arg.Arg.Other;
                            data.BoneSprites.MainColor = color;
                        }))
                    .Head.Append(StackWidgetOption.Create(
                        ("1*", "名前"),
                        ("1*", InputFieldWidgetOption.Create<MMgr, MArg>(
                            (manager, arg) =>
                            {
                                var data = (SewedEquipmentData)arg.Arg.Other;
                                return data.Name;
                            },
                            (manager, arg, value) =>
                            {
                                var data = (SewedEquipmentData)arg.Arg.Other;
                                return data.Name = value;
                            }))))

                    .Head.Append(SelectOption.Create<MMgr, MArg>(
                        getName: (manager, arg) =>
                        {
                            var data = (SewedEquipmentData)arg.Arg.Other;
                            return $"<#{ColorUtility.ToHtmlStringRGBA(data.BoneSprites.MainColor)}>メインカラー";
                        },
                        onClick: (manager, arg) => manager.PushMenuScreen(colorPicker, arg)))

                    .VarOnce(out var equipmentSlotsScreen, new EquipmentSlotsScreen())
                    .Head.Option("装備部位", equipmentSlotsScreen)

                    .Head.Append(StackWidgetOption.Create(
                        ("1*", "順序"),
                        ("1*", InputFieldWidgetOption.Create<MMgr, MArg>(
                            (manager, arg) =>
                            {
                                var data = (SewedEquipmentData)arg.Arg.Other;
                                return data.BoneSpriteEffectOrder.ToString();
                            },
                            (manager, arg, value) =>
                            {
                                if (!float.TryParse(value, out var order)) { order = 0f; }

                                var data = (SewedEquipmentData)arg.Arg.Other;
                                data.BoneSpriteEffectOrder = order;
                                return order.ToString();
                            },
                            TMP_InputField.ContentType.DecimalNumber))))

                    .NameFrom((item, manager, arg) =>
                    {
                        if (item is PaintBoneSprite boneSprite) return boneSprite.Bone.Name;
                        else return string.Empty;
                    })

                    .VarOnce(out var nextScreen, new PaintBoneSpriteMenu())
                    .OnClick((boneSprite, manager, arg) =>
                    {
                        // 部位編集
                        var data = (SewedEquipmentData)arg.Arg.Other;
                        manager.PushMenuScreen(nextScreen, arg.Self, other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite));
                    })

                    .Tail.Option("+ 追加", (manager, arg) =>
                    {
                        // 部位追加
                        var data = (SewedEquipmentData)arg.Arg.Other;
                        var boneSprite = new PaintBoneSprite();
                        boneSprite.NormalFront = boneSprite.BackRear = new DotterBoard(new Vector2Int(32, 32), 16);
                        boneSprite.NormalRear = boneSprite.BackFront = new DotterBoard(new Vector2Int(32, 32), 16);
                        boneSprite.Bone = BoneKeyword.Body;
                        boneSprite.Mirroring = true;
                        data.BoneSprites.Add(boneSprite);
                        manager.PushMenuScreen(nextScreen, arg.Self, other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite));
                    })

                    .Build();
            }

            private static void Save(MMgr manager, MArg arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // 編集画面から戻ったとき、その装備品を更新する
                var data = (SewedEquipmentData)arg.Arg.Other;
                var equipment = arg.Arg.TargetObj;
                if (equipment != null)
                {
                    // 装備品更新
                    equipment.Main.SetBaseInfoSet(equipment, new SewedEquipmentInfoSet(data));
                }
                else
                {
                    // 新規装備品
                    new SewedEquipmentInfoSet(data).CreateObj(arg.Self, Vector2Int.zero);
                }

                manager.PopMenuScreen(2);
            }
        }

        private class EquipmentSlotsScreen : RogueMenuScreen
        {
            private ISerializableKeyword[] keywords;

            private readonly ScrollMenuViewData<ISerializableKeyword, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                keywords ??= new ISerializableKeyword[]
                {
                    //EquipmentSlotKw.Shield,
                    //EquipmentSlotKw.Weapon,
                    //EquipmentSlotKw.Ammo,
                    EquipmentSlotKw.Headwear,
                    EquipmentSlotKw.Cloak,
                    //EquipmentSlotKw.Accessory,
                    //EquipmentSlotKw.BodyArmor,
                    EquipmentSlotKw.Tops,
                    EquipmentSlotKw.Boots,
                    EquipmentSlotKw.Bottoms,
                    EquipmentSlotKw.Glasses,
                    EquipmentSlotKw.FaceMask,
                    EquipmentSlotKw.Gloves,
                    EquipmentSlotKw.Socks,
                    EquipmentSlotKw.Innerwear,
                    null
                };

                view.Show(keywords, manager, arg)
                    ?
                    .NameFrom((slot, manager, arg) =>
                    {
                        if (slot == null) return "その他";
                        return slot.Name;
                    })

                    .OnClick((slot, manager, arg) =>
                    {
                        var data = (SewedEquipmentData)arg.Arg.Other;
                        if (slot == null)
                        {
                            data.SetEquipmentSlots(Spanning<ISerializableKeyword>.Empty);
                        }
                        else
                        {
                            data.SetEquipmentSlots(new[] { slot });

                            if (slot is EquipmentSlotKeywordAsset keyword)
                            {
                                data.BoneSpriteEffectOrder = keyword.Order;
                            }
                        }

                        manager.PopMenuScreen();
                    })

                    .Build();
            }
        }
    }
}
