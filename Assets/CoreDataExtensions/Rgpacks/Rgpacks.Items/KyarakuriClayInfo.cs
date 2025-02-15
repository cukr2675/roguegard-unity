using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class KyarakuriClayInfo
    {
        public string Name { get; set; }

        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public float LoadCapacity { get; set; }

        public PropertiedCmnData Walk { get; set; }
        public PropertiedCmnData Wait { get; set; }
        public PropertiedCmnData Attack { get; set; }
        public PropertiedCmnData Throw { get; set; }
        public PropertiedCmnData PickUp { get; set; }
        public PropertiedCmnData Put { get; set; }
        public PropertiedCmnData Eat { get; set; }

        public PropertiedCmnData Hit { get; set; }
        public PropertiedCmnData BeDefeated { get; set; }
        public PropertiedCmnData Locate { get; set; }
        public PropertiedCmnData Polymorph { get; set; }

        public PropertiedCmnData BeApplied { get; set; }
        public PropertiedCmnData BeThrown { get; set; }
        public PropertiedCmnData BeEaten { get; set; }

        public PropertiedCmnData RaceSprite { get; set; }
        public PropertiedCmnData RaceWeight { get; set; }

        private KyarakuriClayInfo()
        {
            Walk = new PropertiedCmnData();
            Wait = new PropertiedCmnData();
            Attack = new PropertiedCmnData();
            Throw = new PropertiedCmnData();
            PickUp = new PropertiedCmnData();
            Put = new PropertiedCmnData();
            Eat = new PropertiedCmnData();
            Hit = new PropertiedCmnData();
            Locate = new PropertiedCmnData();
            Polymorph = new PropertiedCmnData();
            BeApplied = new PropertiedCmnData();
            BeThrown = new PropertiedCmnData();
            BeEaten = new PropertiedCmnData();
            RaceSprite = new PropertiedCmnData();
            RaceWeight = new PropertiedCmnData();
        }

        public static KyarakuriClayInfo Get(RogueObj obj)
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
            if (info.info != null) throw new RogueException();

            info.info = new KyarakuriClayInfo();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public KyarakuriClayInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo coming) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
