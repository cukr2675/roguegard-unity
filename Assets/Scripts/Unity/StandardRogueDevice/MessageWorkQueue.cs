using Lysionium;
using OchalikeSprites;
using Roguegard;
using Roguegard.Device;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace RoguegardUnity
{
    internal class MessageWorkQueue
    {
        private readonly Queue<RogueCharacterWork> works;
        private readonly Queue<int> integers;
        private readonly Queue<float> numbers;

        private readonly Queue<object> others;

        private readonly ListuiScreenQueue screens;
        private readonly Stack<RogueObj> hideCharacters;

#if DEBUG
        private readonly Queue<StackTrace> stackTraces = new();
#endif

        public StackTrace PeekStackTrace
        {
#if DEBUG
            get => stackTraces.TryPeek(out var stackTrace) ? stackTrace : null;
#else
            get => throw new System.NotSupportedException($"{nameof(PeekStackTrace)} はランタイムではサポートされません。");
#endif
        }

        public int Count => others.Count;

        private static readonly HideMotion hideMotion = new();

        public MessageWorkQueue()
        {
            works = new Queue<RogueCharacterWork>();
            integers = new Queue<int>();
            numbers = new Queue<float>();
            others = new Queue<object>();
            screens = new ListuiScreenQueue();
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

        public void EnqueueScreen(IListuiScreen<MMgrBase, MArg> screen, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            others.Enqueue(DeviceKw.EnqueueMenu);
            screens.Enqueue(screen, self, user, arg);
            EnqueueStackTrace();
        }

        public void InsertHideCharacterWork(RogueObj obj)
        {
            hideCharacters.Push(obj);
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
                work = RogueCharacterWork.CreateSpriteMotion(obj, hideMotion, true);
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

        public void DequeueScreen(out IListuiScreen<MMgrBase, MArg> screen, out RogueObj self, out RogueObj user, out RogueMethodArgument arg)
        {
            screens.Dequeue(out screen, out self, out user, out arg);
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
                ISpriteMotionSet motionSet, int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion)
            {
                transform.Scale = Vector3.zero;
                endOfMotion = true;
            }
        }
    }
}
