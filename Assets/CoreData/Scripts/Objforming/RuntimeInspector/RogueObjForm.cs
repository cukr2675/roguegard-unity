using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using Objforming;
using Objforming.Unity.RuntimeInspector;

namespace Roguegard.Objforming.RuntimeInspector
{
    public class RogueObjForm : FormerForm
    {
        private readonly ButtonElement buttonElementPrefab;
        private readonly UnityAction<RogueObj> serializeAction;

        public RogueObjForm(Former former, LinkElement linkElementPrefab, ButtonElement buttonElementPrefab, UnityAction<RogueObj> serializeAction)
            : base(former, linkElementPrefab)
        {
            this.buttonElementPrefab = buttonElementPrefab;
            this.serializeAction = serializeAction;
        }

        public static RogueObjForm Create(LinkElement linkElementPrefab, ButtonElement buttonElementPrefab, UnityAction<RogueObj> serializeAction)
        {
            var type = typeof(RogueObj);
            var members = FormerMember.Generate(type);
            var former = new Former(type, members);
            return new RogueObjForm(former, linkElementPrefab, buttonElementPrefab, serializeAction);
        }

        public override void SetPageTo(FormInspector inspector, object value)
        {
            base.SetPageTo(inspector, value);

            // シリアル化ボタンを表示する
            var obj = (RogueObj)value;
            var serializeButtonElement = Object.Instantiate(buttonElementPrefab, inspector.Page);
            serializeButtonElement.Initialize("Serialize", () => serializeAction(obj));
        }
    }
}
