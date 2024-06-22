using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class LobbyMerchantBeApplied : ReferableScript, IApplyRogueMethod
    {
        private LobbyMerchantBeApplied() { }

        [SerializeField, ElementDescription("_option")] private ScriptableStartingItem[] _items = null;

        IRogueMethodTarget ISkillDescription.Target => null;
        IRogueMethodRange ISkillDescription.Range => null;
        int ISkillDescription.RequiredMP => 0;
        Spanning<IKeyword> ISkillDescription.AmmoCategories => Spanning<IKeyword>.Empty;

        private RogueMenu rogueMenu;

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                rogueMenu ??= new RogueMenu() { parent = this };
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.StartTalk);
                RogueDevice.Add(DeviceKw.AppendText, "è§êlÅuÇÌÇΩÇµÇÕè§êlÇ≈Ç∑ Ç≈Ç‡Ç‹ÇæèÄîıíÜÇ≈Ç∑\n");
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);
                RogueDevice.Primary.AddMenu(rogueMenu, user, null, RogueMethodArgument.Identity);
                return true;
            }
            else
            {
                return false;
            }
        }

        int ISkillDescription.GetATK(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        private class RogueMenu : IListMenu, IElementPresenter
        {
            public LobbyMerchantBeApplied parent;

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var scroll = manager.GetView(DeviceKw.MenuScroll);
                scroll.OpenView(this, parent._items, manager, self, user, arg);
                ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
            }

            public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ((ScriptableStartingItem)element).Name;
            }

            public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.AddObject(DeviceKw.AppendText, element);
                manager.AddObject(DeviceKw.AppendText, "ÇéËÇ…ì¸ÇÍÇΩ\n");

                var startingItem = (ScriptableStartingItem)element;
                startingItem.Option.CreateObj(startingItem, self, Vector2Int.zero, RogueRandom.Primary);
            }
        }
    }
}
