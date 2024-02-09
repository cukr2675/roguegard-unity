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

        [SerializeField] private ScriptableStartingItem[] _items = null;

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

        private class RogueMenu : IModelsMenu, IModelsMenuItemController
        {
            public LobbyMerchantBeApplied parent;

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, parent._items, root, self, user, arg);
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ((ScriptableStartingItem)model).Name;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.AddObject(DeviceKw.AppendText, DeviceKw.StartTalk);
                root.AddObject(DeviceKw.AppendText, model);
                root.AddObject(DeviceKw.AppendText, "ÇéËÇ…ì¸ÇÍÇΩ\n");
                root.AddObject(DeviceKw.AppendText, DeviceKw.EndTalk);

                var startingItem = (ScriptableStartingItem)model;
                startingItem.Option.CreateObj(startingItem, self, Vector2Int.zero, RogueRandom.Primary);
            }
        }
    }
}
