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
        /// �w��� <see cref="RogueObj"/> �����[���h�����ֈړ�������B���ڒ��̃L�����̏ꍇ�̓��U���g��\������B
        /// </summary>
        public void AfterGameOver(RogueObj leaderCharacter)
        {
            // ���łŃQ�[���I�[�o�[�ɂȂ����Ƃ��̂��߂ɃX�^�b�N����ݒ肷��
            leaderCharacter.TrySetStack(1);

            // �ꎞ�I�Ƀ��[���h�����ֈړ�
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
        /// ���O�\�� �� ���U���g�\�� �� ���r�[�֋A��
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
                // ���O�\��
                view.ShowTemplate(manager, arg)
                    ?
                    .Build();
            }

            private class NextMenu : RogueMenuScreen
            {
                public override void OpenScreen(in MMgr manager, in MArg arg)
                {
                    // ���U���g�\�� �� ���r�[�֋A��
                    var player = arg.Self;
                    var summary = RoguegardSubViews.GetSummary(manager);
                    summary.SetGameOver(player, arg.Arg.TargetObj, manager);
                    summary.Show();
                }
            }
        }
    }
}
