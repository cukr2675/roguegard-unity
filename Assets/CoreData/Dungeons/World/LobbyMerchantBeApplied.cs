using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using UnityEngine;

namespace Roguegard
{
    public class LobbyMerchantBeApplied : ReferableScript, IApplyRogueMethod
    {
        private LobbyMerchantBeApplied() { }

        [SerializeField, ElementDescription("_option")] private AssetStartingItem[] _items = null;

        IRogueMethodTarget ISkillDescription.Target => null;
        IRogueMethodRange ISkillDescription.Range => null;
        int ISkillDescription.RequiredMp => 0;
        Spanning<IKeyword> ISkillDescription.AmmoCategories => Spanning<IKeyword>.Empty;

        private SpeechScreen rogueMenu;

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                rogueMenu ??= new SpeechScreen() { parent = this };
                RogueDevice.Primary.AddMenu(rogueMenu, user, null, RogueMethodArgument.Identity);
                return true;
            }
            else
            {
                return false;
            }
        }

        int ISkillDescription.GetAtk(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        private class SpeechScreen : RogueMenuScreen
        {
            public LobbyMerchantBeApplied parent;

            private readonly SpeechBoxViewTemplate<MMgr, MArg> view = new()
            {
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate($"商人「わたしは商人です でもまだ準備中です{{v}}", manager, arg)
                    ?
                    .VarOnce(out var nextScreen, new RogueMenu() { parent = parent })

                    .OnCompleted((manager, arg) =>
                    {
                        manager.PushMenuScreen(nextScreen, arg.Self);
                    })

                    .Build();
            }
        }

        private class RogueMenu : RogueMenuScreen
        {
            public LobbyMerchantBeApplied parent;

            private readonly ScrollViewTemplate<AssetStartingItem, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate(parent._items, manager, arg)
                    ?
                    .NameFrom((item, manager, arg) =>
                    {
                        return item.Name;
                    })

                    .OnClick((item, manager, arg) =>
                    {
                        manager.AddObject(DeviceKw.AppendText, item);
                        manager.AddObject(DeviceKw.AppendText, "を手に入れた\n");

                        item.Option.CreateObj(item, arg.Self, Vector2Int.zero, RogueRandom.Primary);
                    })

                    .Build();
            }
        }
    }
}
