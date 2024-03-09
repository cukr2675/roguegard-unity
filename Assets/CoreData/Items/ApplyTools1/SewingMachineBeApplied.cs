using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;
using Roguegard.Device;

namespace Roguegard
{
    public class SewingMachineBeApplied : BaseApplyRogueMethod
    {
        private static readonly Menu menu = new Menu();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            RogueDevice.Primary.AddMenu(menu, user, null, RogueMethodArgument.Identity);
            return false;
        }

        private class Menu : IModelsMenu, IModelsMenuItemController
        {
            private readonly List<object> models = new List<object>();
            private static readonly SewingMenu nextMenu = new SewingMenu();

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var spaceObjs = self.Space.Objs;
                models.Clear();
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    var obj = spaceObjs[i];
                    if (obj == null) continue;

                    models.Add(obj);
                }
                models.Add(null);

                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, models, root, self, null, RogueMethodArgument.Identity);
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return "+ �V�������";
                else return ((RogueObj)model).GetName();
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (model == null)
                {
                    // �����i��V�K�쐬����ꍇ�̓f�[�^�N���X�𐶐�����
                    var data = new SewedEquipmentData();
                    for (int i = 0; i < RoguegardSettings.DefaultPalette.Count; i++)
                    {
                        data.BoneSprites.SetPalette(i, RoguegardSettings.DefaultPalette[i]);
                    }
                    data.BoneSprites.MainColor = Color.white;
                    root.OpenMenu(nextMenu, self, null, new(other: data, targetObj: null), RogueMethodArgument.Identity);
                }
                else if (model is RogueObj equipment && equipment.Main.BaseInfoSet is SewedEquipmentInfoSet infoSet)
                {
                    // �ۑ������I���ł���悤�ɕ�������
                    var data = infoSet.GetDataClone();
                    root.OpenMenu(nextMenu, self, null, new(other: data, targetObj: equipment), RogueMethodArgument.Identity);
                }
            }
        }

        private class SewingMenu : IModelsMenu, IModelsMenuItemController
        {
            private static List<object> models;
            private static readonly PaintBoneSpriteMenu nextMenu = new PaintBoneSpriteMenu();
            private static readonly EquipPartsMenu equipPartsMenu = new EquipPartsMenu();
            private static readonly ExitDialog exitDialog = new ExitDialog();

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (models == null)
                {
                    models = new List<object>()
                    {
                        new NameOption(),
                        new ColorPicker(),
                        new ActionModelsMenuChoice("��������", EquipParts),
                        new OrderOption(),
                    };
                }

                var data = (SewedEquipmentData)arg.Other;
                var equipment = arg.TargetObj;

                models.RemoveRange(4, models.Count - 4);
                for (int i = 0; i < data.BoneSprites.Items.Count; i++)
                {
                    var item = data.BoneSprites.Items[i];
                    models.Add(item);
                }
                models.Add(null);

                var options = (IScrollModelsMenuView)root.Get(DeviceKw.MenuOptions);
                options.OpenView(this, models, root, self, null, new(other: data, targetObj: equipment));
                options.ShowExitButton(new ActionModelsMenuChoice("<", Exit));
            }

            private static void EquipParts(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var data = (SewedEquipmentData)arg.Other;
                root.OpenMenu(equipPartsMenu, self, null, new(other: data), arg);
            }

            private static void Exit(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.AddInt(DeviceKw.StartTalk, 0);
                root.AddObject(DeviceKw.AppendText, "�ҏW���e��ۑ����܂����H");
                root.AddInt(DeviceKw.WaitEndOfTalk, 0);
                root.OpenMenuAsDialog(exitDialog, self, user, arg, arg);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is IModelsMenuChoice choice) return choice.GetName(root, self, user, arg);
                else if (model is PaintBoneSprite item) return item.Bone.Name;
                else return "+ �ǉ�";
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is IModelsMenuChoice choice)
                {
                    choice.Activate(root, self, user, arg);
                }
                else if (model is PaintBoneSprite boneSprite)
                {
                    // ���ʕҏW
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var data = (SewedEquipmentData)arg.Other;
                    root.OpenMenu(nextMenu, self, null, new(other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite)), arg);
                }
                else
                {
                    // ���ʒǉ�
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var data = (SewedEquipmentData)arg.Other;
                    boneSprite = new PaintBoneSprite();
                    boneSprite.NormalFront = boneSprite.BackRear = new DotterBoard(new Vector2Int(32, 32), 16);
                    boneSprite.NormalRear = boneSprite.BackFront = new DotterBoard(new Vector2Int(32, 32), 16);
                    boneSprite.Bone = BoneKw.Body;
                    boneSprite.Mirroring = true;
                    data.BoneSprites.Add(boneSprite);
                    root.OpenMenu(nextMenu, self, null, new(other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite)), arg);
                }
            }

            private class NameOption : IModelsMenuOptionText
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "���O";
                }

                public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var data = (SewedEquipmentData)arg.Other;
                    return data.Name;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
                {
                    var data = (SewedEquipmentData)arg.Other;
                    data.Name = value;
                }
            }

            private class ExitDialog : IModelsMenu
            {
                private readonly object[] models = new object[]
                {
                    new ActionModelsMenuChoice("�㏑���ۑ�", Save),
                    new ActionModelsMenuChoice("�ۑ����Ȃ�", NotSave),
                    ExitModelsMenuChoice.Instance
                };

                public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.Get(DeviceKw.MenuTalkChoices).OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, user, arg);
                }

                private static void Save(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    // �ҏW��ʂ���߂����Ƃ��A���̑����i���X�V����
                    var data = (SewedEquipmentData)arg.Other;
                    var equipment = arg.TargetObj;
                    if (equipment != null)
                    {
                        // �����i�X�V
                        equipment.Main.SetBaseInfoSet(equipment, new SewedEquipmentInfoSet(data));
                    }
                    else
                    {
                        // �V�K�����i
                        new SewedEquipmentInfoSet(data).CreateObj(player, Vector2Int.zero, RogueRandom.Primary);
                    }

                    root.Back();
                    root.Back();
                }

                private static void NotSave(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    // ������������
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    root.Back();
                    root.Back();
                }
            }
        }

        private class ColorPicker : IModelsMenuOptionColor
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var data = (SewedEquipmentData)arg.Other;
                return $"<#{ColorUtility.ToHtmlStringRGBA(data.BoneSprites.MainColor)}>���C���J���[";
            }

            public Color GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var data = (SewedEquipmentData)arg.Other;
                return data.BoneSprites.MainColor;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, Color value)
            {
                var data = (SewedEquipmentData)arg.Other;
                data.BoneSprites.MainColor = value;
            }
        }

        private class EquipPartsMenu : IModelsMenu, IModelsMenuItemController
        {
            private object[] models;

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (models == null)
                {
                    models = new ISerializableKeyword[]
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

                var scroll = root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, models, root, self, null, arg);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return "���̑�";
                return ((IKeyword)model).Name;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var data = (SewedEquipmentData)arg.Other;
                if (model == null)
                {
                    data.SetEquipParts(Spanning<ISerializableKeyword>.Empty);
                }
                else
                {
                    var part = (ISerializableKeyword)model;
                    data.SetEquipParts(new[] { part });

                    if (part is EquipKeywordData keyword)
                    {
                        data.BoneSpriteEffectOrder = keyword.Order;
                    }
                }

                root.Back();
            }
        }

        private class OrderOption : IModelsMenuOptionText
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "����";
            }

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var data = (SewedEquipmentData)arg.Other;
                return data.BoneSpriteEffectOrder.ToString();
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var data = (SewedEquipmentData)arg.Other;
                if (float.TryParse(value, out var order))
                {
                    data.BoneSpriteEffectOrder = order;
                }
            }
        }
    }
}
