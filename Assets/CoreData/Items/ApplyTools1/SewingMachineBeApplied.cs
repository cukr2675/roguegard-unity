using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using ListingMF;
using SDSSprite;
using RuntimeDotter;
using Roguegard.Device;

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
            private readonly List<RogueObj> elms = new();

            private readonly ScrollViewTemplate<RogueObj, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var spaceObjs = arg.Self.Space.Objs;
                elms.Clear();
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    var obj = spaceObjs[i];
                    if (obj == null) continue;

                    elms.Add(obj);
                }
                elms.Add(null);

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .ElementNameFrom((item, manager, arg) =>
                    {
                        if (item == null) return "+ �V�������";
                        else return item.GetName();
                    })

                    .VariableOnce(out var nextScreen, new SewingScreen())
                    .OnClickElement((item, manager, arg) =>
                    {
                        if (item == null)
                        {
                            // �����i��V�K�쐬����ꍇ�̓f�[�^�N���X�𐶐�����
                            var data = new SewedEquipmentData();
                            for (int i = 0; i < RoguegardSettings.DefaultPalette.Count; i++)
                            {
                                data.BoneSprites.SetPalette(i, RoguegardSettings.DefaultPalette[i]);
                            }
                            data.BoneSprites.MainColor = Color.white;
                            manager.PushMenuScreen(nextScreen, arg.Self, other: data, targetObj: null);
                        }
                        else if (item.Main.BaseInfoSet is SewedEquipmentInfoSet infoSet)
                        {
                            // �ۑ������I���ł���悤�ɕ�������
                            var data = infoSet.GetDataClone();
                            manager.PushMenuScreen(nextScreen, arg.Self, other: data, targetObj: item);
                        }
                    })

                    .Build();
            }
        }

        private class SewingScreen : RogueMenuScreen
        {
            private static List<object> elms;

            private readonly ScrollViewTemplate<object, MMgr, MArg> view = new()
            {
                ScrollSubViewName = StandardSubViewTable.WidgetsName,
                BackAnchorList = new()
                {
                    SelectOption.Create<MMgr, MArg>(":Back", ChoicesMenuScreen.SaveBackDialog(Save)),
                },
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                if (elms == null)
                {
                    var colorPicker = new ColorPickerMenuScreen<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            var data = (SewedEquipmentData)arg.Arg.Other;
                            return data.BoneSprites.MainColor;
                        },
                        (manager, arg, color) =>
                        {
                            var data = (SewedEquipmentData)arg.Arg.Other;
                            data.BoneSprites.MainColor = color;
                        });

                    elms = new List<object>()
                    {
                        new object[]
                        {
                            "���O",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) =>
                                {
                                    var data = (SewedEquipmentData)arg.Arg.Other;
                                    return data.Name;
                                },
                                (manager, arg, value) =>
                                {
                                    var data = (SewedEquipmentData)arg.Arg.Other;
                                    return data.Name = value;
                                }),
                        },

                        SelectOption.Create<MMgr, MArg>(
                            (manager, arg) =>
                            {
                                var data = (SewedEquipmentData)arg.Arg.Other;
                                return $"<#{ColorUtility.ToHtmlStringRGBA(data.BoneSprites.MainColor)}>���C���J���[";
                            }, colorPicker),

                        SelectOption.Create<MMgr, MArg>("��������", new EquipPartsScreen()),

                        new object[]
                        {
                            "����",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
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
                                TMP_InputField.ContentType.DecimalNumber),
                        },
                    };
                }

                var data = (SewedEquipmentData)arg.Arg.Other;
                var equipment = arg.Arg.TargetObj;

                elms.RemoveRange(4, elms.Count - 4);
                for (int i = 0; i < data.BoneSprites.Items.Count; i++)
                {
                    var item = data.BoneSprites.Items[i];
                    elms.Add(item);
                }
                elms.Add(null);

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .ElementNameFrom((element, manager, arg) =>
                    {
                        if (element is ISelectOption selectOption) return selectOption.GetName(manager, arg);
                        else if (element is PaintBoneSprite item) return item.Bone.Name;
                        else return "+ �ǉ�";
                    })

                    .VariableOnce(out var nextScreen, new PaintBoneSpriteMenu())
                    .OnClickElement((element, manager, arg) =>
                    {
                        if (element is ISelectOption selectOption)
                        {
                            selectOption.HandleClick(manager, arg);
                        }
                        else if (element is PaintBoneSprite boneSprite)
                        {
                            // ���ʕҏW
                            var data = (SewedEquipmentData)arg.Arg.Other;
                            manager.PushMenuScreen(nextScreen, arg.Self, other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite));
                        }
                        else
                        {
                            // ���ʒǉ�
                            var data = (SewedEquipmentData)arg.Arg.Other;
                            boneSprite = new PaintBoneSprite();
                            boneSprite.NormalFront = boneSprite.BackRear = new DotterBoard(new Vector2Int(32, 32), 16);
                            boneSprite.NormalRear = boneSprite.BackFront = new DotterBoard(new Vector2Int(32, 32), 16);
                            boneSprite.Bone = BoneKeyword.Body;
                            boneSprite.Mirroring = true;
                            data.BoneSprites.Add(boneSprite);
                            manager.PushMenuScreen(nextScreen, arg.Self, other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite));
                        }
                    })

                    .Build();
            }

            private static void Save(MMgr manager, MArg arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // �ҏW��ʂ���߂����Ƃ��A���̑����i���X�V����
                var data = (SewedEquipmentData)arg.Arg.Other;
                var equipment = arg.Arg.TargetObj;
                if (equipment != null)
                {
                    // �����i�X�V
                    equipment.Main.SetBaseInfoSet(equipment, new SewedEquipmentInfoSet(data));
                }
                else
                {
                    // �V�K�����i
                    new SewedEquipmentInfoSet(data).CreateObj(arg.Self, Vector2Int.zero, RogueRandom.Primary);
                }

                manager.Back();
                manager.Back();
            }
        }

        private class EquipPartsScreen : RogueMenuScreen
        {
            private ISerializableKeyword[] elms;

            private readonly ScrollViewTemplate<ISerializableKeyword, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                if (elms == null)
                {
                    elms = new ISerializableKeyword[]
                    {
                        //EquipKw.Shield,
                        //EquipKw.Weapon,
                        //EquipKw.Ammo,
                        EquipKw.Headwear,
                        EquipKw.Cloak,
                        //EquipKw.Accessory,
                        //EquipKw.BodyArmor,
                        EquipKw.Tops,
                        EquipKw.Boots,
                        EquipKw.Bottoms,
                        EquipKw.Lenses,
                        EquipKw.FaceMask,
                        EquipKw.Gloves,
                        EquipKw.Socks,
                        EquipKw.Innerwear,
                        null
                    };
                }

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .ElementNameFrom((part, manager, arg) =>
                    {
                        if (part == null) return "���̑�";
                        return part.Name;
                    })

                    .OnClickElement((part, manager, arg) =>
                    {
                        var data = (SewedEquipmentData)arg.Arg.Other;
                        if (part == null)
                        {
                            data.SetEquipParts(Spanning<ISerializableKeyword>.Empty);
                        }
                        else
                        {
                            data.SetEquipParts(new[] { part });

                            if (part is EquipKeywordData keyword)
                            {
                                data.BoneSpriteEffectOrder = keyword.Order;
                            }
                        }

                        manager.Back();
                    })

                    .Build();
            }
        }
    }
}
