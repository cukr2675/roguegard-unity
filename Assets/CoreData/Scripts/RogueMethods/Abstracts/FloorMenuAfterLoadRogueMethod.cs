using Lysionium;
using Roguegard.Device;
using System.Collections;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// ダンジョン名と階層を表示してそこに移動させる <see cref="IRogueMethod"/>
    /// </summary>
    public abstract class FloorMenuAfterLoadRogueMethod : BaseApplyRogueMethod, ISelectOption<MMgr, MArg>
    {
        public RogueMenuScreen MenuScreen { get; }

        protected FloorMenuAfterLoadRogueMethod()
        {
            MenuScreen = new Screen()
            {
                selectOptions = new ISelectOption<MMgr, MArg>[] { this }
            };
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
                RogueDevice.Primary.AddMenu(MenuScreen, player, null, RogueMethodArgument.Identity);
            }
            else
            {
                Activate(null, player, null, RogueMethodArgument.Identity);
            }
            return true;
        }

        protected abstract string GetName(MMgr manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg);

        protected abstract void Activate(MMgr manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg);

        string ISelectOption<MMgr, MArg>.GetName(MMgr manager, MArg arg)
        {
            var args = (MArg)arg;
            return GetName((MMgr)manager, args.Self, args.User, args.Arg);
        }

        string ISelectOption<MMgr, MArg>.GetStyle(MMgr manager, MArg arg) => null;

        void ISelectOption<MMgr, MArg>.Click(MMgr manager, MArg arg)
        {
            var args = (MArg)arg;
            Activate((MMgr)manager, args.Self, args.User, args.Arg);
        }

        private class Screen : RogueMenuScreen
        {
            public ISelectOption<MMgr, MArg>[] selectOptions;

            private readonly FadeOutInViewData<MMgr, MArg> view = new()
            {
            };

            private ISubviewStateProvider stateProvider;

            public override bool IsIncremental => true;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                view.FadeOut(manager, arg)
                    ?
                    .OnFadeOutCompleted((manager, arg) =>
                    {
                        var levelText = "";
                        if (DungeonInfo.TryGet(arg.Self.Location, out var dungeonInfo))
                        {
                            levelText = dungeonInfo.GetLevelText(arg.Self.Location);
                        }

                        selectOptions[0].Click(manager, arg);
                        manager.Overlay.Show(
                            new[] {
                                $"<align=\"center\"><size=+32>{arg.Self.Location.GetName()} {levelText}"
                            }, ToStringViewItemHandler.Instance, manager, arg, ref stateProvider);
                        manager.StartCoroutine(Wait2sDone(manager));
                    })

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
            }

            private IEnumerator Wait2sDone(MMgr manager)
            {
                yield return new WaitForSeconds(2f);

                manager.Overlay.Hide(false, (manager, arg) =>
                {
                    ((MMgr)manager).Done();
                });
            }
        }
    }
}
