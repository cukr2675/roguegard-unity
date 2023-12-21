using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

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
        /// �w��� <see cref="RogueObj"/> �����[���h�����ֈړ�������B���ڒ��̃L�����̏ꍇ�̓��U���g��\������B
        /// </summary>
        public void AfterGameOver(RogueObj leaderCharacter)
        {
            // ���łŃQ�[���I�[�o�[�ɂȂ����Ƃ��̂��߂ɃX�^�b�N����ݒ肷��
            leaderCharacter.TrySetStack(1);

            // �ꎞ�I�Ƀ��[���h�����ֈړ�
            var dungeon = leaderCharacter.Location;
            SpaceUtility.TryLocate(leaderCharacter, componentManager.World);

            if (leaderCharacter == componentManager.TargetObj)
            {
                componentManager.EventManager.AddMenu(gameOverMenu, leaderCharacter, null, new(targetObj: dungeon));
                RogueDevice.Add(DeviceKw.EnqueueViewDequeueState, 0);
            }
        }

        /// <summary>
        /// ���O�\�� �� ���U���g�\�� �� ���r�[�֋A��
        /// </summary>
        private class GameOverMenu : IModelsMenu
        {
            private static readonly object[] choices = new[] { new Next() };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // ���O�\��
                root.Get(DeviceKw.MenuLog).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, self, user, arg);
            }

            private class Next : IModelsMenuChoice
            {
                private static readonly NextMenu nextMenu = new NextMenu();

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) => null;

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.OpenMenu(nextMenu, self, user, arg, arg);
                }
            }

            private class NextMenu : IModelsMenu
            {
                public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
                {
                    // ���U���g�\�� �� ���r�[�֋A��
                    var summary = (IResultMenuView)root.Get(DeviceKw.MenuSummary);
                    summary.OpenView(ChoicesModelsMenuItemController.Instance, Spanning<object>.Empty, root, player, user, arg);
                    summary.SetGameOver(player, arg.TargetObj);
                }
            }
        }
    }
}
