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

        protected void EnqueueMessageRule(RogueObj self, IKeyword keyword)
        {
            if (!RoguegardSettings.KeywordsNotEnqueueMessageRule.Contains(keyword) &&
                MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.HorizontalRule);
            }
        }

        public abstract bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        public abstract ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool);

        private class MenuChoice : BaseModelsMenuChoice
        {
            public BaseObjCommand parent;

            public override string Name => parent.Name;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var deviceInfo = RogueDeviceEffect.Get(self);
                deviceInfo.SetDeviceCommand(parent, user, arg);
                root.Done();
                RogueDevice.Add(DeviceKw.EnqueueSE, DeviceKw.Submit);
            }
        }
    }
}
