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
    public abstract class FloorMenuAfterLoadRogueMethod : BaseApplyRogueMethod, IListMenuSelectOption
    {
        public RogueMenuScreen MenuScreen { get; }

        protected FloorMenuAfterLoadRogueMethod()
        {
            MenuScreen = new Screen()
            {
                selectOptions = new IListMenuSelectOption[] { this }
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

        string IListMenuSelectOption.GetName(IListMenuManager manager, IListMenuArg arg)
        {
            var args = (ReadOnlyMenuArg)arg;
            return GetName((RogueMenuManager)manager, args.Self, args.User, args.Arg);
        }

        void IListMenuSelectOption.HandleClick(IListMenuManager manager, IListMenuArg arg)
        {
            var args = (ReadOnlyMenuArg)arg;
            Activate((RogueMenuManager)manager, args.Self, args.User, args.Arg);
        }

        private class Screen : RogueMenuScreen
        {
            public IListMenuSelectOption[] selectOptions;

            private readonly FadeOutInViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.FadeOut(manager, arg)
                    ?.OnFadeOutCompleted((manager, arg) =>
                    {
                        selectOptions[0].HandleClick(manager, arg);
                        manager.Done();
                    })
                    .Build();
            }
        }
    }
}
