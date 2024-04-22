using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard
{
    // �Q�[��������^�悷����@�̔�r
    // �E�A�j���[�V������ۑ�
    // �@�@�\�ʏサ���Č��ł��Ȃ�
    // �@�@�A�j���[�V�������V���A�����\�ɂ���K�v����i������ɂ��惆�[�U��`�A�j���[�V�����ŕK�v�j
    // �E���[���h�S�̂ƃv���C���[�����ۑ�
    // �@�@�Z�[�u�f�[�^����剻����
    // �@�@�S�R�}���h���V���A�����\�ɂ���K�v����
    // �@�@�o�[�W���j���O���K�v�@�����A�b�v�f�[�g��Web�A�v���Ƃ͑���������
    // �E�^�撆�̓_���W�����P�ʂŐ؂藣��
    // �@�@�o�O�𐶂݂₷���i�_���W�����O�����犱����Ă����m�ł��Ȃ��j
    // �@�@���R�x��������i�^�撆�̃_���W�����ɂ͊O�����犱�s�j
    // �@�@RogueRandom ��؂藣����悤�ɐ݌v�ύX���K�v
    // �@�@�o�[�W���j���O���K�v�@�����A�b�v�f�[�g��Web�A�v���Ƃ͑���������
    // �A�j���[�V�����ۑ��ɂ���

    public class DungeonRecorder
    {
        public DungeonQuest Quest { get; }
        public int FloorLv { get; }
        public MessageWorkList List { get; }

        private PlayCommand playCommand;

        public DungeonRecorder(DungeonQuest quest, int floorLv)
        {
            Quest = quest;
            FloorLv = floorLv;
            List = new MessageWorkList();
            //var obj = new RogueObj();
            //obj.Main = new MainRogueObjInfo();
            //obj.Main.SetBaseInfoSet(obj, RogueDevice.Primary.Player.Main.InfoSet);
            //List.Add(RogueCharacterWork.CreateWalk(obj, Vector2Int.zero, RogueDirection.Down, KeywordSpriteMotion.Attack, false));
            //List.Add(RogueCharacterWork.CreateWalk(obj, Vector2Int.one*100, 1f, RogueDirection.Down, KeywordSpriteMotion.Attack, false));
            playCommand = new PlayCommand() { recorder = this };
        }

        public void Add(IKeyword keyword, int integer)
        {
            if (keyword == DeviceKw.AppendText)
            {
                List.Add(integer.ToString());
            }
        }

        public void Add(IKeyword keyword, float number)
        {
            if (keyword == DeviceKw.AppendText)
            {
                List.Add(number.ToString());
            }
        }

        public void Add(IKeyword keyword, object other)
        {
            if (keyword == DeviceKw.AppendText)
            {
                List.Add(other);
            }
            else if (keyword == DeviceKw.EnqueueSE)
            {
                List.Add(DeviceKw.EnqueueSE);
                List.Add(other);
            }
        }

        public void AddWork(IKeyword keyword, in RogueCharacterWork work)
        {
            if (keyword == DeviceKw.EnqueueWork)
            {
                List.Add(work);
            }
        }

        public void Play(RogueObj player)
        {
            // Seed ���w�肵�ĊK�w����
            var random = new RogueRandom(Quest.Seed);
            var world = RogueWorldInfo.GetWorld(player);
            var dungeon = Quest.Dungeon.CreateObj(world, Vector2Int.zero, random);
            var dungeonSeed = random.Next(int.MinValue, int.MaxValue);
            DungeonInfo.SetSeedTo(dungeon, dungeonSeed);
            var dungeonInfo = DungeonInfo.Get(dungeon);
            if (!dungeonInfo.TryGetRandom(FloorLv, out var floorRandom)) throw new RogueException();
            if (!dungeonInfo.TryGetLevel(FloorLv, out var level)) throw new RogueException();

            var floor = Quest.Dungeon.CreateObj(dungeon, Vector2Int.zero, floorRandom);
            level.GenerateFloor(player, floor, floorRandom);

            var view = ViewInfo.Get(player);
            //player.TryLocate(floor, Vector2Int.zero, false, false, false, false, StackOption.Default);
            view.ReadyView(player.Location);
            default(IActiveRogueMethodCaller).Affect(player, 0f, VisionStatusEffect.Callback);

            var device = RogueDeviceEffect.Get(player);
            device.SetDeviceCommand(playCommand, null, RogueMethodArgument.Identity);
        }

        private class PlayCommand : IDeviceCommandAction
        {
            public DungeonRecorder recorder;
            public int index;

            public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                while (index < recorder.List.Count)
                {
                    recorder.List.Get(index, out var other, out var work);
                    index++;
                    if (other == DeviceKw.EnqueueWork)
                    {
                        RogueDevice.AddWork(DeviceKw.EnqueueWork, work);
                    }
                    else if (other == DeviceKw.EnqueueSE)
                    {
                        recorder.List.Get(index, out var seName, out _);
                        index++;
                        RogueDevice.Add(DeviceKw.EnqueueSE, seName);
                    }
                    else
                    {
                        RogueDevice.Add(DeviceKw.AppendText, other);
                    }
                }
                return true;
            }
        }
    }
}
