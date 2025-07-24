using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class EffectStickerInfo
    {
        public PropertiedCmnData Update { get; set; }

        public PropertiedCmnData PassiveAspect { get; set; }

        public string Sprite { get; set; }

        private EffectStickerInfo() { }

        public static EffectStickerInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上書き不可
        /// </summary>
        public static void SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // 上書き不可
            if (info.info != null) throw new System.InvalidOperationException();

            info.info = new EffectStickerInfo();
            info.info.Update = new PropertiedCmnData();
            info.info.PassiveAspect = new PropertiedCmnData();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public EffectStickerInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo coming) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
