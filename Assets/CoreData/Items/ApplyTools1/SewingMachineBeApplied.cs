using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using SDSSprite;
using RuntimeDotter;
using Roguegard.Device;

namespace Roguegard
{
    public class SewingMachineBeApplied : BaseApplyRogueMethod
    {
        //private static readonly Menu menu = new Menu();

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            //RogueDevice.Primary.AddMenu(menu, user, null, RogueMethodArgument.Identity);
            return false;
        }

        //private class Menu : IListMenu, IElementPresenter
        //{
        //    private readonly List<object> elms = new List<object>();
        //    private static readonly SewingMenu nextMenu = new SewingMenu();

        //    public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var spaceObjs = self.Space.Objs;
        //        elms.Clear();
        //        for (int i = 0; i < spaceObjs.Count; i++)
        //        {
        //            var obj = spaceObjs[i];
        //            if (obj == null) continue;

        //            elms.Add(obj);
        //        }
        //        elms.Add(null);

        //        var scroll = manager.GetView(DeviceKw.MenuScroll);
        //        scroll.OpenView(this, elms, manager, self, null, RogueMethodArgument.Identity);
        //        ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
        //    }

        //    public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        if (element == null) return "+ �V�������";
        //        else return ((RogueObj)element).GetName();
        //    }

        //    public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //        if (element == null)
        //        {
        //            // �����i��V�K�쐬����ꍇ�̓f�[�^�N���X�𐶐�����
        //            var data = new SewedEquipmentData();
        //            for (int i = 0; i < RoguegardSettings.DefaultPalette.Count; i++)
        //            {
        //                data.BoneSprites.SetPalette(i, RoguegardSettings.DefaultPalette[i]);
        //            }
        //            data.BoneSprites.MainColor = Color.white;
        //            manager.OpenMenu(nextMenu, self, null, new(other: data, targetObj: null));
        //        }
        //        else if (element is RogueObj equipment && equipment.Main.BaseInfoSet is SewedEquipmentInfoSet infoSet)
        //        {
        //            // �ۑ������I���ł���悤�ɕ�������
        //            var data = infoSet.GetDataClone();
        //            manager.OpenMenu(nextMenu, self, null, new(other: data, targetObj: equipment));
        //        }
        //    }
        //}

        //private class SewingMenu : IListMenu, IElementPresenter
        //{
        //    private static List<object> elms;
        //    private static object[] leftAnchorList = new[] { DialogListMenuSelectOption.CreateExit(Save) };
        //    private static readonly PaintBoneSpriteMenu nextMenu = new PaintBoneSpriteMenu();
        //    private static readonly EquipPartsMenu equipPartsMenu = new EquipPartsMenu();

        //    public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        if (elms == null)
        //        {
        //            elms = new List<object>()
        //            {
        //                new NameOption(),
        //                new ColorPicker(),
        //                new ActionListMenuSelectOption("��������", EquipParts),
        //                new OrderOption(),
        //            };
        //        }

        //        var data = (SewedEquipmentData)arg.Other;
        //        var equipment = arg.TargetObj;

        //        elms.RemoveRange(4, elms.Count - 4);
        //        for (int i = 0; i < data.BoneSprites.Items.Count; i++)
        //        {
        //            var item = data.BoneSprites.Items[i];
        //            elms.Add(item);
        //        }
        //        elms.Add(null);

        //        var options = manager.GetView(DeviceKw.MenuOptions);
        //        options.OpenView(this, elms, manager, self, null, new(other: data, targetObj: equipment));
        //        var leftAnchor = manager.GetView(DeviceKw.MenuLeftAnchor);
        //        leftAnchor.OpenView(SelectOptionPresenter.Instance, leftAnchorList, manager, self, null, new(other: data, targetObj: equipment));
        //    }

        //    private static void EquipParts(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //        var data = (SewedEquipmentData)arg.Other;
        //        manager.OpenMenu(equipPartsMenu, self, null, new(other: data));
        //    }

        //    public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        if (element is IListMenuSelectOption selectOption) return selectOption.GetName(manager, self, user, arg);
        //        else if (element is PaintBoneSprite item) return item.Bone.Name;
        //        else return "+ �ǉ�";
        //    }

        //    public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        if (element is IListMenuSelectOption selectOption)
        //        {
        //            selectOption.Activate(manager, self, user, arg);
        //        }
        //        else if (element is PaintBoneSprite boneSprite)
        //        {
        //            // ���ʕҏW
        //            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //            var data = (SewedEquipmentData)arg.Other;
        //            manager.OpenMenu(nextMenu, self, null, new(other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite)));
        //        }
        //        else
        //        {
        //            // ���ʒǉ�
        //            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //            var data = (SewedEquipmentData)arg.Other;
        //            boneSprite = new PaintBoneSprite();
        //            boneSprite.NormalFront = boneSprite.BackRear = new DotterBoard(new Vector2Int(32, 32), 16);
        //            boneSprite.NormalRear = boneSprite.BackFront = new DotterBoard(new Vector2Int(32, 32), 16);
        //            boneSprite.Bone = BoneKeyword.Body;
        //            boneSprite.Mirroring = true;
        //            data.BoneSprites.Add(boneSprite);
        //            manager.OpenMenu(nextMenu, self, null, new(other: data.BoneSprites, count: data.BoneSprites.IndexOf(boneSprite)));
        //        }
        //    }

        //    private static void Save(IListMenuManager manager, RogueObj player, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //        // �ҏW��ʂ���߂����Ƃ��A���̑����i���X�V����
        //        var data = (SewedEquipmentData)arg.Other;
        //        var equipment = arg.TargetObj;
        //        if (equipment != null)
        //        {
        //            // �����i�X�V
        //            equipment.Main.SetBaseInfoSet(equipment, new SewedEquipmentInfoSet(data));
        //        }
        //        else
        //        {
        //            // �V�K�����i
        //            new SewedEquipmentInfoSet(data).CreateObj(player, Vector2Int.zero, RogueRandom.Primary);
        //        }

        //        manager.Back();
        //        manager.Back();
        //    }

        //    private class NameOption : IOptionsMenuText
        //    {
        //        public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

        //        public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        {
        //            return "���O";
        //        }

        //        public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        {
        //            var data = (SewedEquipmentData)arg.Other;
        //            return data.Name;
        //        }

        //        public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
        //        {
        //            var data = (SewedEquipmentData)arg.Other;
        //            data.Name = value;
        //        }
        //    }
        //}

        //private class ColorPicker : IOptionsMenuColor
        //{
        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var data = (SewedEquipmentData)arg.Other;
        //        return $"<#{ColorUtility.ToHtmlStringRGBA(data.BoneSprites.MainColor)}>���C���J���[";
        //    }

        //    public Color GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var data = (SewedEquipmentData)arg.Other;
        //        return data.BoneSprites.MainColor;
        //    }

        //    public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, Color value)
        //    {
        //        var data = (SewedEquipmentData)arg.Other;
        //        data.BoneSprites.MainColor = value;
        //    }
        //}

        //private class EquipPartsMenu : IListMenu, IElementPresenter
        //{
        //    private object[] elms;

        //    public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        if (elms == null)
        //        {
        //            elms = new ISerializableKeyword[]
        //            {
        //                //EquipKw.Shield,
        //                //EquipKw.Weapon,
        //                //EquipKw.Ammo,
        //                EquipKw.Headwear,
        //                EquipKw.Cloak,
        //                //EquipKw.Accessory,
        //                //EquipKw.BodyArmor,
        //                EquipKw.Tops,
        //                EquipKw.Boots,
        //                EquipKw.Bottoms,
        //                EquipKw.Lenses,
        //                EquipKw.FaceMask,
        //                EquipKw.Gloves,
        //                EquipKw.Socks,
        //                EquipKw.Innerwear,
        //                null
        //            };
        //        }

        //        var scroll = manager.GetView(DeviceKw.MenuScroll);
        //        scroll.OpenView(this, elms, manager, self, null, arg);
        //    }

        //    public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        if (element == null) return "���̑�";
        //        return ((IKeyword)element).Name;
        //    }

        //    public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

        //        var data = (SewedEquipmentData)arg.Other;
        //        if (element == null)
        //        {
        //            data.SetEquipParts(Spanning<ISerializableKeyword>.Empty);
        //        }
        //        else
        //        {
        //            var part = (ISerializableKeyword)element;
        //            data.SetEquipParts(new[] { part });

        //            if (part is EquipKeywordData keyword)
        //            {
        //                data.BoneSpriteEffectOrder = keyword.Order;
        //            }
        //        }

        //        manager.Back();
        //    }
        //}

        //private class OrderOption : IOptionsMenuText
        //{
        //    public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.DecimalNumber;

        //    public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        return "����";
        //    }

        //    public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var data = (SewedEquipmentData)arg.Other;
        //        return data.BoneSpriteEffectOrder.ToString();
        //    }

        //    public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
        //    {
        //        var data = (SewedEquipmentData)arg.Other;
        //        if (float.TryParse(value, out var order))
        //        {
        //            data.BoneSpriteEffectOrder = order;
        //        }
        //    }
        //}
    }
}
