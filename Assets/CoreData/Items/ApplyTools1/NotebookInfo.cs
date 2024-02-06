using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class NotebookInfo
    {
        public static string GetText(RogueObj obj)
        {
            obj.Main.TryOpenRogueEffects(obj);
            var info = obj.Get<Info>();
            return info.quote?.Text ?? info.text;
        }

        public static NotebookQuote GetQuote(RogueObj obj)
        {
            obj.Main.TryOpenRogueEffects(obj);
            var info = obj.Get<Info>();
            if (info.quote == null)
            {
                info.quote = new NotebookQuote(info.text);
                info.text = null;
            }
            return info.quote;
        }

        public static void Ready(RogueObj obj, string initialText = null)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }
            if (info.text == null) { info.text = initialText; }
        }

        public static void SetTo(RogueObj obj, string text)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }
            if (info.quote != null) throw new RogueException($"{obj} のテキストはロックされています。");

            info.text = text;
        }

        public static bool RemoveFrom(RogueObj obj)
        {
            return obj.RemoveInfo(typeof(Info));
        }

        [ObjectFormer.Formable]
        private class Info : IRogueObjInfo
        {
            public string text;
            public NotebookQuote quote;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other)
            {
                return false;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Info() { text = text, quote = quote };
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
