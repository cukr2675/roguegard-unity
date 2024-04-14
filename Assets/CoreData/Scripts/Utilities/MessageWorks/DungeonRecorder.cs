using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    // ゲーム処理を録画する方法の比較
    // ・アニメーションを保存
    // 　　表面上しか再現できない
    // 　　アニメーションをシリアル化可能にする必要あり（いずれにせよユーザ定義アニメーションで必要）
    // ・ワールド全体とプレイヤー操作を保存
    // 　　セーブデータが肥大化する
    // 　　全コマンドをシリアル化可能にする必要あり
    // 　　バージョニングが必要　強制アップデートのWebアプリとは相性が悪い
    // ・録画中はダンジョン単位で切り離す
    // 　　バグを生みやすい（ダンジョン外部から干渉されても検知できない）
    // 　　自由度が下がる（録画中のダンジョンには外部から干渉不可）
    // 　　RogueRandom を切り離せるように設計変更が必要
    // 　　バージョニングが必要　強制アップデートのWebアプリとは相性が悪い
    // アニメーション保存にする

    public class DungeonRecorder
    {
        public DungeonQuest Quest { get; }
        public int Floor { get; }
        public MessageWorkList List { get; }

        private PlayCommand playCommand;

        public DungeonRecorder(DungeonQuest quest, int floor)
        {
            Quest = quest;
            Floor = floor;
            List = new MessageWorkList();
            var obj = new RogueObj();
            obj.Main = new MainRogueObjInfo();
            obj.Main.SetBaseInfoSet(obj, RogueDevice.Primary.Player.Main.InfoSet);
            List.Add(RogueCharacterWork.CreateWalk(obj, Vector2Int.zero, RogueDirection.Down, KeywordSpriteMotion.Attack, false));
            List.Add(RogueCharacterWork.CreateWalk(obj, Vector2Int.one*100, 1f, RogueDirection.Down, KeywordSpriteMotion.Attack, false));
            playCommand = new PlayCommand() { recorder = this };
        }

        public void Add(IKeyword keyword, int integer)
        {
            if (keyword == DeviceKw.AppendText)
            {
                List.Add(integer);
            }
        }

        public void Add(IKeyword keyword, float number)
        {
            if (keyword == DeviceKw.AppendText)
            {
                List.Add(number);
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
            // Seed を指定して階層生成

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
