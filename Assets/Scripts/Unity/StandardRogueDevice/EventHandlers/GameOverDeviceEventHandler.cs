using Lysionium;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

namespace RoguegardUnity
{
    internal class GameOverDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;

        private static readonly GameOverScreen gameOverScreen = new();

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
                componentManager.EventManager.AddScreen(gameOverScreen, leaderCharacter, null, new(targetObj: dungeon));
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
        private class GameOverScreen : RogueListuiScreen
        {
            private readonly MainMenuViewData<MMgr> view = new()
            {
                PrimaryCommandSubviewSelector = null,
                BackAnchorSubviewSelector = m => m.ForwardAnchor,
            };

            public GameOverScreen()
            {
                view.BackAnchorList = new(_ => _.Option("OK", new NextScreen(), () => Arg));

                OnOpenScreen += (manager) =>
                {
                    // ログ表示
                    view.Show(manager)
                    ?
                    .Build();

                    manager.LongMessage.Show();
                };
            }

            private class NextScreen : RogueListuiScreen
            {
                public NextScreen()
                {
                    OnOpenScreen += (manager) =>
                    {
                        // リザルト表示 → ロビーへ帰還
                        var player = Arg.Self;
                        manager.Summary.SetGameOver(player, Arg.Arg.TargetObj, manager);
                        manager.Summary.Show();
                    };
                }
            }
        }
    }
}
