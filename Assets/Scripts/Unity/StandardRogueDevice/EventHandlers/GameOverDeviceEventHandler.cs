using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (keyword == DeviceKw.GameOver && obj is RogueObj leaderCharacter)
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

            if (leaderCharacter == componentManager.Subject)
            {
                // 一時的にワールド直下へ移動
                var dungeon = leaderCharacter.Location;
                SpaceUtility.TryLocate(leaderCharacter, componentManager.World);

                componentManager.EventManager.AddMenu(gameOverMenu, leaderCharacter, null, new(targetObj: dungeon));
                RogueDevice.Add(DeviceKw.EnqueueViewDequeueState, 0);
            }
            else
            {
                FadeCanvas.StartCanvasCoroutine(Coroutine());
            }

            IEnumerator Coroutine()
            {
                // RogueMethodAspectState の処理の完了を待つ
                yield return null;

                default(IActiveRogueMethodCaller).LocateSavePoint(leaderCharacter, null, 0f, RogueWorldSavePointInfo.Instance, true);

                var memberInfo = LobbyMemberList.GetMemberInfo(leaderCharacter);
                memberInfo.SavePoint = RogueWorldSavePointInfo.Instance;
            }
        }

        /// <summary>
        /// ログ表示 → リザルト表示 → ロビーへ帰還
        /// </summary>
        private class GameOverMenu : IModelsMenu
        {
            private static readonly object[] choices = new[] { new Next() };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // ログ表示
                root.Get(DeviceKw.MenuLog).OpenView(ChoiceListPresenter.Instance, choices, root, self, user, arg);
            }

            private class Next : BaseModelsMenuChoice
            {
                public override string Name => null;

                private static readonly NextMenu nextMenu = new();

                public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.OpenMenu(nextMenu, self, user, arg);
                }
            }

            private class NextMenu : IModelsMenu
            {
                public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
                {
                    // リザルト表示 → ロビーへ帰還
                    var summary = (IResultMenuView)root.Get(DeviceKw.MenuSummary);
                    summary.OpenView(ChoiceListPresenter.Instance, Spanning<object>.Empty, root, player, user, arg);
                    summary.SetGameOver(player, arg.TargetObj);
                }
            }
        }
    }
}
