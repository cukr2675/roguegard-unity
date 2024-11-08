using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    internal class GameOverDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;

        private static readonly GameOverMenu gameOverMenu = new GameOverMenu();

        public GameOverDeviceEventHandler(StandardRogueDeviceComponentManager componentManager)
        {
            this.componentManager = componentManager;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            if (keyword == DeviceKw.GameOver && obj is RogueObj leaderCharacter && leaderCharacter == componentManager.Subject)
            {
                AfterGameOver(leaderCharacter);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 指定の <see cref="RogueObj"/> をワールド直下へ移動させる。注目中のキャラの場合はリザルトを表示する。
        /// </summary>
        public void AfterGameOver(RogueObj leaderCharacter)
        {
            // 消滅でゲームオーバーになったときのためにスタック数を設定する
            leaderCharacter.TrySetStack(1);

            // 一時的にワールド直下へ移動
            var dungeon = leaderCharacter.Location;
            SpaceUtility.TryLocate(leaderCharacter, componentManager.World);

            if (leaderCharacter == componentManager.Subject)
            {
                componentManager.EventManager.AddMenu(gameOverMenu, leaderCharacter, null, new(targetObj: dungeon));
                RogueDevice.Add(DeviceKw.EnqueueViewDequeueState, 0);
            }
            else
            {
                default(IActiveRogueMethodCaller).LocateSavePoint(leaderCharacter, null, 0f, RogueWorldSavePointInfo.Instance, true);

                var memberInfo = LobbyMemberList.GetMemberInfo(leaderCharacter);
                memberInfo.SavePoint = RogueWorldSavePointInfo.Instance;
            }
        }

        /// <summary>
        /// ログ表示 → リザルト表示 → ロビーへ帰還
        /// </summary>
        private class GameOverMenu : RogueMenuScreen
        {
            private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
            {
                PrimaryCommandSubViewName = StandardSubViewTable.LongMessageName,
                BackAnchorSubViewName = StandardSubViewTable.ForwardAnchorName,
                BackAnchorList = new List<ISelectOption>() { SelectOption.Create<MMgr, MArg>("OK", new NextMenu()) },
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                // ログ表示
                view.ShowTemplate(manager, arg)
                    ?
                    .Build();
            }

            private class NextMenu : RogueMenuScreen
            {
                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    // リザルト表示 → ロビーへ帰還
                    var player = arg.Self;
                    var summary = RoguegardSubViews.GetSummary(manager);
                    summary.SetGameOver(player, arg.Arg.TargetObj, manager);
                    summary.Show();
                }
            }
        }
    }
}
