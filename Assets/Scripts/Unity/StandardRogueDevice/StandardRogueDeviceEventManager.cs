using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class StandardRogueDeviceEventManager
    {
        private readonly TouchController touchController;
        private readonly CharacterRenderSystem characterRenderSystem;
        private readonly IStandardRogueDeviceEventHandler[] eventHandlers;
        private readonly MessageWorkQueue messageWorkQueue;

        public bool HasSynchronizedWork { get; private set; }

        public bool IsOpen { get; set; }

        public bool Any => messageWorkQueue.Count >= 1;
        public System.Diagnostics.StackTrace PeekStackTrace => messageWorkQueue.PeekStackTrace;

        public StandardRogueDeviceEventManager(
            TouchController touchController, CharacterRenderSystem characterRenderSystem, IStandardRogueDeviceEventHandler[] eventHandlers)
        {
            this.touchController = touchController;
            this.characterRenderSystem = characterRenderSystem;
            this.eventHandlers = eventHandlers;
            messageWorkQueue = new MessageWorkQueue();
        }

        public void Dequeue(RogueObj player, RogueObj subject, bool fastForward)
        {
            while (messageWorkQueue.Count >= 1)
            {
                messageWorkQueue.Dequeue(out var other, out var work, out var integer, out var number, out var stackTrace);

                // キャラクターアニメーション
                if (other == DeviceKw.EnqueueWork)
                {
                    characterRenderSystem.Work(work, player, fastForward);
                    if (!work.Continues) break;
                }

                // メッセージボックス表示
                else if (other == DeviceKw.EnqueueInteger)
                {
                    touchController.EventManager.Add(DeviceKw.AppendText, integer: integer);
                }
                else if (other == DeviceKw.EnqueueNumber)
                {
                    touchController.EventManager.Add(DeviceKw.AppendText, number: number);
                }
                else if (other == DeviceKw.EnqueueMenu)
                {
                    messageWorkQueue.DequeueMenu(out var menu, out var self, out var user, out var arg);
                    touchController.OpenMenu(subject, menu, self, user, arg);
                    break;
                }
                else if (other == DeviceKw.EnqueueSE)
                {
                    messageWorkQueue.Dequeue(out var seName, out _, out _, out _, out _);
                    touchController.EventManager.Add(DeviceKw.EnqueueSE, obj: seName);
                }
                else if (other == DeviceKw.HorizontalRule)
                {
                    touchController.EventManager.AppendTextObj(player, other, stackTrace);
                }

                // 効果音再生
                else if (other == DeviceKw.EnqueueSEAndWait)
                {
                    messageWorkQueue.Dequeue(out var seName, out _, out _, out _, out _);
                    touchController.EventManager.Add(DeviceKw.EnqueueSEAndWait, obj: seName);
                    break;
                }
                else if (other == DeviceKw.EnqueueWaitSeconds)
                {
                    messageWorkQueue.Dequeue(out _, out _, out _, out var waitSeconds, out _);
                    touchController.EventManager.Add(DeviceKw.EnqueueWaitSeconds, number: waitSeconds);
                    break;
                }

                // 視界制御
                else if (other == DeviceKw.EnqueueViewDequeueState)
                {
                    var view = ViewInfo.Get(player);
                    view.DequeueState();
                    view.ReadyView(player.Location);
                    view.AddView(player);
                }

                // その他オブジェクトはメッセージボックス表示へ
                else
                {
                    other = StandardRogueDeviceUtility.LocalizeMessage(other, player, messageWorkQueue);
                    touchController.EventManager.AppendTextObj(player, other, stackTrace);
                }
            }
        }

        public void Add(IKeyword keyword, int integer = 0, float number = 0f, object obj = null)
        {
            ////////////////////////////////////////////////////////////////////////
            // ゲーム内日時取得・待機・その他イベント
            ////////////////////////////////////////////////////////////////////////

            if (keyword == DeviceKw.WaitForInput)
            {
                touchController.WaitsForInput = true;
                HasSynchronizedWork = true;
                return;
            }
            foreach (var eventHandler in eventHandlers)
            {
                if (eventHandler.TryHandle(keyword, integer, number, obj)) return;
            }



            if (!IsOpen) return;

            ////////////////////////////////////////////////////////////////////////
            // メッセージ表示・効果音再生
            ////////////////////////////////////////////////////////////////////////

            if (keyword == DeviceKw.AppendText)
            {
                if (obj != null) { messageWorkQueue.EnqueueOther(obj); }
                else if (number == 0f) { messageWorkQueue.EnqueueInteger(integer); }
                else { messageWorkQueue.EnqueueNumber(number); }
                HasSynchronizedWork = true;
                return;
            }
            if (keyword == DeviceKw.EnqueueSE)
            {
                messageWorkQueue.EnqueueOther(DeviceKw.EnqueueSE);
                messageWorkQueue.EnqueueOther(obj);
                HasSynchronizedWork = true;
                return;
            }
            if (keyword == DeviceKw.EnqueueSEAndWait)
            {
                messageWorkQueue.EnqueueOther(DeviceKw.EnqueueSEAndWait);
                messageWorkQueue.EnqueueOther(obj);
                HasSynchronizedWork = true;
                return;
            }
            if (keyword == DeviceKw.EnqueueWaitSeconds)
            {
                messageWorkQueue.EnqueueOther(DeviceKw.EnqueueWaitSeconds);
                if (number == 0f) { messageWorkQueue.EnqueueNumber(integer); }
                else { messageWorkQueue.EnqueueNumber(number); }
                HasSynchronizedWork = true;
                return;
            }



            ////////////////////////////////////////////////////////////////////////
            // キャラを隠す
            ////////////////////////////////////////////////////////////////////////

            if (keyword == DeviceKw.InsertHideCharacterWork && obj is RogueObj hideObj)
            {
                messageWorkQueue.InsertHideCharacterWork(hideObj);
                return;
            }



            ////////////////////////////////////////////////////////////////////////
            // 視界更新遅延
            ////////////////////////////////////////////////////////////////////////
            
            if (keyword == DeviceKw.EnqueueViewDequeueState)
            {
                messageWorkQueue.EnqueueOther(DeviceKw.EnqueueViewDequeueState);
                return;
            }



            Debug.LogError($"{keyword?.Name ?? "null"} に対応するキーワードが見つかりません。（obj: {obj}）");
        }

        public void AddWork(RogueObj player, IKeyword keyword, in RogueCharacterWork work, bool fastForward)
        {
            if (!IsOpen) return;

            if (keyword == DeviceKw.EnqueueWork)
            {
                if (messageWorkQueue.Count == 0 && work.Continues)
                {
                    // 移動モーションなどを複数のオブジェクトで同時再生する。
                    characterRenderSystem.Work(work, player, fastForward);
                }
                else
                {
                    // 攻撃モーションなどの再生を待機する。
                    messageWorkQueue.EnqueueWork(work);
                    HasSynchronizedWork = true;
                }
            }
        }

        public void AddMenu(RogueMenuScreen menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            messageWorkQueue.EnqueueMenu(menu, self, user, arg);
        }

        public void Clear()
        {
            messageWorkQueue.Clear();
        }

        public void ResetCalledSynchronizedView()
        {
            HasSynchronizedWork = false;
        }
    }
}
