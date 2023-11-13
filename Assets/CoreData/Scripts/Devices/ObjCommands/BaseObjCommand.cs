using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    /// <summary>
    /// 既存の <see cref="IRogueMethod"/> を呼び出す選択肢モデルクラス
    /// </summary>
    public abstract class BaseObjCommand : ReferableScript, IObjCommand
    {
        public abstract string Name { get; }

        IModelsMenuChoice IObjCommand.ModelsMenuChoice => menuChoice;
        private readonly MenuChoice menuChoice;

        protected BaseObjCommand()
        {
            menuChoice = new MenuChoice();
            menuChoice.parent = this;
        }

        protected void EnqueueMessageRule(IKeyword keyword)
        {
            if (!RoguegardSettings.KeywordsNotEnqueueMessageRule.Contains(keyword))
            {
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.HorizontalRule);
            }
        }

        public abstract bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        public abstract ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool);

        private class MenuChoice : IModelsMenuChoice
        {
            public BaseObjCommand parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return parent.Name;
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var deviceInfo = RogueDeviceEffect.Get(self);
                deviceInfo.SetDeviceCommand(parent, user, arg);
                root.Done();
                RogueDevice.Add(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }
    }
}
