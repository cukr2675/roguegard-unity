using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.Device;

namespace Roguegard
{
    /// <summary>
    /// ダンジョン名と階層を表示してそこに移動させる <see cref="IRogueMethod"/>
    /// </summary>
    public abstract class FloorMenuAfterLoadRogueMethod : BaseApplyRogueMethod, ISelectOption
    {
        public RogueMenuScreen MenuScreen { get; }

        protected FloorMenuAfterLoadRogueMethod()
        {
            MenuScreen = new Screen()
            {
                selectOptions = new ISelectOption[] { this }
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

        protected abstract string GetName(RogueMenuManager manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg);

        protected abstract void Activate(RogueMenuManager manager, RogueObj player, RogueObj empty, in RogueMethodArgument arg);

        string ISelectOption.GetName(IListMenuManager manager, IListMenuArg arg)
        {
            var args = (ReadOnlyMenuArg)arg;
            return GetName((RogueMenuManager)manager, args.Self, args.User, args.Arg);
        }

        string ISelectOption.GetStyle(IListMenuManager manager, IListMenuArg arg) => null;

        void ISelectOption.HandleClick(IListMenuManager manager, IListMenuArg arg)
        {
            var args = (ReadOnlyMenuArg)arg;
            Activate((RogueMenuManager)manager, args.Self, args.User, args.Arg);
        }

        private class Screen : RogueMenuScreen
        {
            public ISelectOption[] selectOptions;

            private readonly FadeOutInViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            private IElementsSubViewStateProvider stateProvider;

            public override bool IsIncremental => true;

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
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

                        selectOptions[0].HandleClick(manager, arg);
                        manager.GetSubView(StandardSubViewTable.OverlayName).Show(
                            new[] {
                                $"<align=\"center\"><size=+32>{arg.Self.Location.GetName()} {levelText}"
                            }, ElementToStringHandler.Instance, manager, arg, ref stateProvider);
                        manager.StartCoroutine(Wait2sDone(manager));
                    })

                    .Build();
            }

            public override void CloseScreen(RogueMenuManager manager, bool back)
            {
            }

            private IEnumerator Wait2sDone(RogueMenuManager manager)
            {
                yield return new WaitForSeconds(2f);

                manager.GetSubView(StandardSubViewTable.OverlayName).Hide(false, (manager, arg) =>
                {
                    ((RogueMenuManager)manager).Done();
                });
            }
        }
    }
}
