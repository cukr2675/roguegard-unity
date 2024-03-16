using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using SkeletalSprite;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class MessageWorkQueue
    {
        private readonly Queue<RogueCharacterWork> works;
        private readonly Queue<int> integers;
        private readonly Queue<float> numbers;

        private readonly Queue<object> others;

        private readonly ModelsMenuQueue menus;
        private readonly Stack<RogueObj> hideCharacters;

#if DEBUG
        private readonly Queue<StackTrace> stackTraces = new Queue<StackTrace>();
#endif

        public int Count => others.Count;

        private static readonly HideMotion hideMotion = new HideMotion();

        public MessageWorkQueue()
        {
            works = new Queue<RogueCharacterWork>();
            integers = new Queue<int>();
            numbers = new Queue<float>();
            others = new Queue<object>();
            menus = new ModelsMenuQueue();
            hideCharacters = new Stack<RogueObj>();
            Clear();
        }

        public void EnqueueWork(in RogueCharacterWork work)
        {
            works.Enqueue(work);
            others.Enqueue(DeviceKw.EnqueueWork);
            EnqueueStackTrace();
        }

        public void EnqueueInteger(int integer)
        {
            integers.Enqueue(integer);
            others.Enqueue(DeviceKw.EnqueueInteger);
            EnqueueStackTrace();
        }

        public void EnqueueNumber(float number)
        {
            numbers.Enqueue(number);
            others.Enqueue(DeviceKw.EnqueueNumber);
            EnqueueStackTrace();
        }

        public void EnqueueOther(object message)
        {
            // EnqueueWork でないときに DeviceKw.EnqueueWork を追加してはならない。
            if (message == DeviceKw.EnqueueWork) throw new System.ArgumentException();

            others.Enqueue(message);
            EnqueueStackTrace();
        }

        public void EnqueueMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            others.Enqueue(DeviceKw.EnqueueMenu);
            menus.Enqueue(menu, self, user, arg);
            EnqueueStackTrace();
        }

        public void InsertHideCharacterWork(RogueObj obj)
        {
            hideCharacters.Push(obj);
            EnqueueStackTrace();
        }

        public void Clear()
        {
            works.Clear();
            others.Clear();
        }

        public void Dequeue(out object other, out RogueCharacterWork work, out int integer, out float number, out StackTrace stackTrace)
        {
            // アニメーション開始前のキャラの自動表示をキャンセルする CharacterWork を挿入する
            if (hideCharacters.TryPop(out var obj))
            {
                other = DeviceKw.EnqueueWork;
                work = RogueCharacterWork.CreateBoneMotion(obj, hideMotion, true);
                integer = default;
                number = default;
                stackTrace = null;
                return;
            }

            other = others.Dequeue();
            if (other == DeviceKw.EnqueueWork)
            {
                work = works.Dequeue();
                integer = default;
                number = default;
            }
            else if (other == DeviceKw.EnqueueInteger)
            {
                work = RogueCharacterWork.Identity;
                integer = integers.Dequeue();
                number = default;
            }
            else if (other == DeviceKw.EnqueueNumber)
            {
                work = RogueCharacterWork.Identity;
                integer = default;
                number = numbers.Dequeue();
            }
            else
            {
                work = RogueCharacterWork.Identity;
                integer = default;
                number = default;
            }

#if DEBUG
            stackTrace = stackTraces.Dequeue();
#else
            stackTrace = null;
#endif
        }

        public void DequeueMenu(out IModelsMenu menu, out RogueObj self, out RogueObj user, out RogueMethodArgument arg)
        {
            menus.Dequeue(out menu, out self, out user, out arg);
        }

        private void EnqueueStackTrace()
        {
#if DEBUG
            stackTraces.Enqueue(new StackTrace(true));
#endif
        }

        private class HideMotion : RogueSpriteMotion
        {
            public override IKeyword Keyword => null;

            public override void ApplyTo(
                ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion)
            {
                transform.Scale = Vector3.zero;
                endOfMotion = true;
            }
        }
    }
}
