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
            if (keyword == DeviceKw.GameOver && obj is RogueObj leaderCharacter && leaderCharacter == componentManager.Subject)
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

            componentManager.EventManager.AddMenu(gameOverMenu, leaderCharacter, null, new(targetObj: dungeon));
            RogueDevice.Add(DeviceKw.EnqueueViewDequeueState, 0);
        }

        /// <summary>
        /// ���O�\�� �� ���U���g�\�� �� ���r�[�֋A��
        /// </summary>
        private class GameOverMenu : IListMenu
        {
            private static readonly object[] selectOptions = new[] { new Next() };

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                // ���O�\��
                manager.GetView(DeviceKw.MenuLog).OpenView(SelectOptionPresenter.Instance, selectOptions, manager, self, user, arg);
            }

            private class Next : BaseListMenuSelectOption
            {
                public override string Name => null;

                private static readonly NextMenu nextMenu = new();

                public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    manager.OpenMenu(nextMenu, self, user, arg);
                }
            }

            private class NextMenu : IListMenu
            {
                public void OpenMenu(IListMenuManager manager, RogueObj player, RogueObj user, in RogueMethodArgument arg)
                {
                    // ���U���g�\�� �� ���r�[�֋A��
                    var summary = (IResultMenuView)manager.GetView(DeviceKw.MenuSummary);
                    summary.OpenView(SelectOptionPresenter.Instance, Spanning<object>.Empty, manager, player, user, arg);
                    summary.SetGameOver(player, arg.TargetObj);
                }
            }
        }
    }
}
