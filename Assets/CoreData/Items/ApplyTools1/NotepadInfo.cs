using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class NotepadInfo
    {
        public static string GetText(RogueObj obj)
        {
            obj.Main.TryOpenRogueEffects(obj);
            if (obj.TryGet<Info>(out var info)) return info.text;
            else return null;
        }

        public static void SetTo(RogueObj obj, string text)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            info.text = text;
        }

        public static bool RemoveFrom(RogueObj obj)
        {
            return obj.RemoveInfo(typeof(Info));
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public string text;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other)
            {
                return false;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Info() { text = text };
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
