using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using UnityEngine;

namespace Roguegard
{
    public class LobbyMerchantBeApplied : ReferableScript, IApplyRogueMethod
    {
        private LobbyMerchantBeApplied() { }

        [SerializeField, DescribeElement] private AssetStartingItem[] _items = null;

        IRogueMethodTarget ISkillDescribable.Target => null;
        IRogueMethodRange ISkillDescribable.Range => null;
        int ISkillDescribable.RequiredMp => 0;
        Spanning<IKeyword> ISkillDescribable.AmmoCategories => Spanning<IKeyword>.Empty;

        private SpeechScreen speechScreen;

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                speechScreen ??= new SpeechScreen() { parent = this };
                RogueDevice.Primary.AddScreen(speechScreen, user, null, RogueMethodArgument.Identity);
                return true;
            }
            else
            {
                return false;
            }
        }

        int ISkillDescribable.GetAtk(RogueObj self, out bool additionalEffect)
        {
            additionalEffect = false;
            return 0;
        }

        private class SpeechScreen : RogueListuiScreen
        {
            public LobbyMerchantBeApplied parent;

            private readonly SpeechBoxViewData<MMgr> view = new()
            {
            };

            public SpeechScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show($"商人「わたしは商人です でもまだ準備中です{{v}}", manager)
                    ?
                    .VarOnce(out var nextScreen, new ShopScreen() { parent = parent })

                    .OnCompleted((manager) =>
                    {
                        manager.PushScreen(nextScreen, Arg.Self);
                    })

                    .Build();
                };

                // IsIncremental を true にする
                OnCloseScreenView += (_, _) => { };
            }
        }

        private class ShopScreen : RogueListuiScreen
        {
            public LobbyMerchantBeApplied parent;

            private readonly ScrollMenuViewData<AssetStartingItem, MMgr> view = new()
            {
            };

            public ShopScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(parent._items, manager)
                    ?
                    .NameFrom((item, manager) =>
                    {
                        return item.Name;
                    })

                    .OnClick((item, manager) =>
                    {
                        manager.AddObject(DeviceKw.AppendText, item);
                        manager.AddObject(DeviceKw.AppendText, "を手に入れた\n");

                        item.Option.CreateObj(item, Arg.Self, Vector2Int.zero, RogueRandom.Primary);
                    })

                    .Build();
                };
            }
        }
    }
}
