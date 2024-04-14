using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class MessageWorkListener
    {
        private static readonly List<IMessageWorkListener> listeners = new();
        private static readonly Stack<Handler> handlerPool = new();

        public static void AddListener(IMessageWorkListener listener)
        {
            listeners.Add(listener);
        }

        public static bool RemoveListener(IMessageWorkListener listener)
        {
            return listeners.Remove(listener);
        }

        public static void ClearListeners()
        {
            listeners.Clear();
        }

        public static bool CanHandle(RogueObj location, Vector2Int position)
        {
            foreach (var listener in listeners)
            {
                if (listener.CanHandle(location, position)) return true;
            }
            return false;
        }

        public static bool TryOpenHandler(RogueObj location, Vector2Int position, out Handler handler)
        {
            if (!handlerPool.TryPeek(out var peekHandler))
            {
                peekHandler = new Handler();
                handlerPool.Push(peekHandler);
            }

            var any = false;
            peekHandler.ClearListeners();
            foreach (var listener in listeners)
            {
                if (listener.CanHandle(location, position))
                {
                    peekHandler.AddListener(listener);
                    any = true;
                }
            }

            if (any)
            {
                handler = handlerPool.Pop();
                return true;
            }
            else
            {
                handler = null;
                return false;
            }
        }

        public static Handler UnionHandlers(Handler a, Handler b)
        {
            if (a == null) return b;
            if (b == null) return a;

            a.AddListenersFrom(b);
            return a;
        }

        public class Handler : System.IDisposable
        {
            private readonly List<IMessageWorkListener> listeners = new();

            public void AddListener(IMessageWorkListener listener)
            {
                listeners.Add(listener);
            }

            public void AddListenersFrom(Handler handler)
            {
                foreach (var handlerListener in handler.listeners)
                {
                    if (!listeners.Contains(handlerListener)) { listeners.Add(handlerListener); }
                }
            }

            public void ClearListeners()
            {
                listeners.Clear();
            }

            public Handler AppendText(int integer)
            {
                foreach (var listener in listeners)
                {
                    listener.Handle(DeviceKw.AppendText, integer);
                }
                return this;
            }

            public Handler AppendText(float number)
            {
                foreach (var listener in listeners)
                {
                    listener.Handle(DeviceKw.AppendText, number);
                }
                return this;
            }

            public Handler AppendText(object text)
            {
                foreach (var listener in listeners)
                {
                    listener.Handle(DeviceKw.AppendText, text);
                }
                return this;
            }

            public void EnqueueSE(IKeyword seName)
            {
                foreach (var listener in listeners)
                {
                    listener.Handle(DeviceKw.EnqueueSE, seName);
                }
            }

            public void EnqueueWork(in RogueCharacterWork work)
            {
                foreach (var listener in listeners)
                {
                    listener.HandleWork(DeviceKw.EnqueueWork, work);
                }
            }

            public void Handle(IKeyword keyword, int integer)
            {
                foreach (var listener in listeners)
                {
                    listener.Handle(keyword, integer);
                }
            }

            public void Handle(IKeyword keyword, float number)
            {
                foreach (var listener in listeners)
                {
                    listener.Handle(keyword, number);
                }
            }

            public void Handle(IKeyword keyword, object other)
            {
                foreach (var listener in listeners)
                {
                    listener.Handle(keyword, other);
                }
            }

            public void HandleWork(IKeyword keyword, in RogueCharacterWork work)
            {
                foreach (var listener in listeners)
                {
                    listener.HandleWork(keyword, work);
                }
            }

            public void Dispose()
            {
                // 同じインスタンスを二個以上プーリングできないようにする
                if (listeners.Count == 0) return;

                listeners.Clear();
                handlerPool.Push(this);
            }
        }
    }
}
