using Lysionium;
using Roguegard.Device;
using System.Collections;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// ダンジョン名と階層を表示してそこに移動させる <see cref="IRogueMethod"/>
    /// </summary>
    public abstract class FloorScreenAfterLoadRogueMethod : BaseApplyRogueMethod
    {
        public RogueListuiScreen EnteredScreen { get; }

        protected FloorScreenAfterLoadRogueMethod()
        {
            EnteredScreen = new Screen(this);
        }

        /// <summary>
        /// <see cref="IApplyRogueMethod"/> のため  user 引数がプレイヤーとなる
        /// </summary>
        public sealed override bool Invoke(RogueObj self, RogueObj player, float activationDepth, in RogueMethodArgument arg)
        {
            if (LobbyMemberList.GetMemberInfo(player) == null)
            {
                // ダンジョン空間オブジェクトが永続的に残ることを避けるため、 LobbyMembers によって追跡できないキャラによる階層移動は禁止する。
                Debug.LogError($"{player} は {nameof(LobbyMemberInfo)} を持ちません。");
                return false;
            }

            if (player == RogueDevice.Primary.Player)
            {
                // ダンジョン名と階層を表示してそこに移動させる。
                RogueDevice.Primary.AddScreen(EnteredScreen, player, null, RogueMethodArgument.Identity);
            }
            else
            {
                Activate(null, player, null, RogueMethodArgument.Identity);
            }
            return true;
        }

        protected abstract string GetName(MMgr manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg);

        protected abstract void Activate(MMgr manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg);

        private class Screen : RogueListuiScreen
        {
            private readonly FadeOutInViewData<MMgr> view = new()
            {
            };

            public Screen(FloorScreenAfterLoadRogueMethod parent)
            {
                ISubviewStateProvider stateProvider = null;

                OnOpenScreen += (manager) =>
                {
                    view.FadeOut(manager)
                    ?
                    .OnFadeOutCompleted((manager) =>
                    {
                        var levelText = "";
                        if (DungeonInfo.TryGet(Arg.Self.Location, out var dungeonInfo))
                        {
                            levelText = dungeonInfo.GetLevelText(Arg.Self.Location);
                        }

                        parent.Activate(manager, Arg.Self, Arg.User, Arg.Arg);
                        manager.Overlay.Show(
                            new[] {
                                $"<align=\"center\"><size=+32>{Arg.Self.Location.GetName()} {levelText}"
                            }, SelectOptionViewItemHandler<IListuiManager>.Instance, manager, ref stateProvider);
                        manager.StartCoroutine(Wait2sDone(manager));
                    })

                    .Build();
                };

                OnCloseScreenView += (manager, back) => { };
            }

            private IEnumerator Wait2sDone(MMgr manager)
            {
                yield return new WaitForSeconds(2f);

                manager.Overlay.Hide(false, (manager) =>
                {
                    ((MMgr)manager).Done();
                });
            }
        }
    }
}
