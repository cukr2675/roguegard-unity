using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
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

        private class RogueMenu : RogueMenuScreen
        {
            public LobbyMerchantBeApplied parent;

            private readonly ScrollViewTemplate<ScriptableStartingItem, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.Show(parent._items, manager, arg)
                    ?
                    .ElementNameFrom((item, manager, arg) =>
                    {
                        return item.Name;
                    })

                    .OnClickElement((item, manager, arg) =>
                    {
                        manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                        manager.AddObject(DeviceKw.AppendText, item);
                        manager.AddObject(DeviceKw.AppendText, "ÇéËÇ…ì¸ÇÍÇΩ\n");

                        item.Option.CreateObj(item, arg.Self, Vector2Int.zero, RogueRandom.Primary);
                    })

                    .Build();
            }
        }
    }
}
