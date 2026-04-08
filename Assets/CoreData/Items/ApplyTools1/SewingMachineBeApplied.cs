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
        private static readonly SewingMachineScreen sewingMachineScreen = new();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            RogueDevice.Primary.AddScreen(sewingMachineScreen, user, null, RogueMethodArgument.Identity);
            return false;
        }

        private class SewingMachineScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<RogueObj, MMgr> view = new()
            {
            };

            public SewingMachineScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(Arg.Self.Space.Objs, manager)
                    ?
                    .Filter(obj => obj != null)

                    .NameFrom((item, manager) =>
                    {
                        if (item == null) return "+ 新しく作る";
                        else return item.GetName();
                    })

                    .VarOnce(out var nextScreen, new SewingScreen())
                    .OnClick((item, manager) =>
                    {
                        if (item.Main.BaseInfoSet is SewedEquipmentInfoSet infoSet)
                        {
                            // 保存せず終了できるように複製する
                            var data = infoSet.GetDataClone();
                            manager.PushScreen(nextScreen, Arg.Self, other: data, targetObj: item);
                        }
                    })

                    .Tail.Option("+ 新しく作る", (manager) =>
                    {
                        // 装備品を新規作成する場合はデータクラスを生成する
                        var data = new SewedEquipmentData();
                        for (int i = 0; i < RoguegardSettings.DefaultPalette.Length; i++)
                        {
                            data.BoneSprites.SetPalette(i, RoguegardSettings.DefaultPalette[i]);
                        }
                        data.BoneSprites.MainColor = Color.white;
                        manager.PushScreen(nextScreen, Arg.Self, other: data, targetObj: null);
                    })

                    .Build();
                };
            }
        }

        private class SewingScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<IPaintBoneSprite, MMgr> view = new()
            {
                ScrollSubviewSelector = m => m.Widgets,
            };

            public SewingScreen()
            {
                view.BackAnchorList = new(_ => _.Option(":Back", ChoicesScreen.SaveBackDialog(Save), () => Arg));

                OnOpenScreen += (manager) =>
                {
                    var data = (SewedEquipmentData)Arg.Arg.Other;

                    view.Show(data.BoneSprites.Items, manager)
                    ?
                    .VarOnce(out var colorPicker, new ColorPickerScreen<MMgr>(
                        (manager) =>
                        {
                            var data = (SewedEquipmentData)Arg.Arg.Other;
                            return data.BoneSprites.MainColor;
                        },
                        (color, manager) =>
                        {
                            var data = (SewedEquipmentData)Arg.Arg.Other;
                            data.BoneSprites.MainColor = color;
                        }))
                    .Head.Append(StackWidgetOption.Create(
                        ("1*", "名前"),
                        ("1*", InputFieldWidgetOption.Create<MMgr>(
                            _ =>
                            {
                                var data = (SewedEquipmentData)Arg.Arg.Other;
                                return data.Name;
                            },
                            value =>
                            {
                                var data = (SewedEquipmentData)Arg.Arg.Other;
                                return data.Name = value;
                            }))))

                    .Head.Append(SelectOption.Create<MMgr>(
                        getName: (manager) =>
                        {
                            var data = (SewedEquipmentData)Arg.Arg.Other;
                            return $"<#{ColorUtility.ToHtmlStringRGBA(data.BoneSprites.MainColor)}>メインカラー";
                        },
                        onClick: (manager) => manager.PushScreen(colorPicker)))

                    .VarOnce(out var equipmentSlotsScreen, new EquipmentSlotsScreen())
                    .Head.Option("装備部位", equipmentSlotsScreen, () => Arg)

                    .Head.Append(StackWidgetOption.Create(
                        ("1*", "順序"),
                        ("1*", InputFieldWidgetOption.Create<MMgr>(
                            _ =>
                            {
                                var data = (SewedEquipmentData)Arg.Arg.Other;
                                return data.BoneSpriteEffectOrder.ToString();
                            },
                            value =>
                            {
                                if (!float.TryParse(value, out var order)) { order = 0f; }

                                var data = (SewedEquipmentData)Arg.Arg.Other;
                                data.BoneSpriteEffectOrder = order;
                                return order.ToString();
                            },
                            TMP_InputField.ContentType.DecimalNumber))))

                    .NameFrom((item, manager) =>
                    {
                        if (item is PaintBoneSprite boneSprite) return boneSprite.Bone.Name;
                        else return string.Empty;
                    })

                    .VarOnce(out var nextScreen, new PaintBoneSpriteMenuScreen())
                    .OnClick((boneSprite, manager) =>
                    {
                        // 部位編集
                        var data = (SewedEquipmentData)Arg.Arg.Other;
                        manager.PushScreen(nextScreen, Arg.Self, other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite));
                    })

                    .Tail.Option("+ 追加", (manager) =>
                    {
                        // 部位追加
                        var data = (SewedEquipmentData)Arg.Arg.Other;
                        var boneSprite = new PaintBoneSprite();
                        boneSprite.NormalFront = boneSprite.BackRear = new DotterBoard(new Vector2Int(32, 32), 16);
                        boneSprite.NormalRear = boneSprite.BackFront = new DotterBoard(new Vector2Int(32, 32), 16);
                        boneSprite.Bone = BoneKeyword.Body;
                        boneSprite.Mirroring = true;
                        data.BoneSprites.Add(boneSprite);
                        manager.PushScreen(nextScreen, Arg.Self, other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite));
                    })

                    .Build();
                };
            }

            private void Save(MMgr manager)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // 編集画面から戻ったとき、その装備品を更新する
                var data = (SewedEquipmentData)Arg.Arg.Other;
                var equipment = Arg.Arg.TargetObj;
                if (equipment != null)
                {
                    // 装備品更新
                    equipment.Main.SetBaseInfoSet(equipment, new SewedEquipmentInfoSet(data));
                }
                else
                {
                    // 新規装備品
                    new SewedEquipmentInfoSet(data).CreateObj(Arg.Self, Vector2Int.zero);
                }

                manager.PopScreen(2);
            }
        }

        private class EquipmentSlotsScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<ISerializableKeyword, MMgr> view = new()
            {
            };

            public EquipmentSlotsScreen()
            {
                ISerializableKeyword[] keywords = null;

                OnOpenScreen += (manager) =>
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

                    view.Show(keywords, manager)
                    ?
                    .NameFrom((slot, manager) =>
                    {
                        if (slot == null) return "その他";
                        return slot.Name;
                    })

                    .OnClick((slot, manager) =>
                    {
                        var data = (SewedEquipmentData)Arg.Arg.Other;
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

                        manager.PopScreen();
                    })

                    .Build();
                };
            }
        }
    }
}
