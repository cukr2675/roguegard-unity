using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.Device;

namespace Roguegard
{
    /// <summary>
    /// 既存の <see cref="IRogueMethod"/> を呼び出す選択肢モデルクラス
    /// </summary>
    public abstract class BaseObjCommand : ReferableScript, IObjCommand
    {
        public abstract string Name { get; }

        IListMenuSelectOption IObjCommand.SelectOption => menuSelectOption ??= ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(Name, (manager, arg) =>
        {
            var deviceInfo = RogueDeviceEffect.Get(arg.Self);
            deviceInfo.SetDeviceCommand(this, arg.User, arg.Arg);
            manager.Done();
            RogueDevice.Add(DeviceKw.EnqueueSE, DeviceKw.Submit);
        });
        private ListMenuSelectOption<RogueMenuManager, ReadOnlyMenuArg> menuSelectOption;

        protected void EnqueueMessageRule(RogueObj self, IKeyword keyword)
        {
            if (!RoguegardSettings.KeywordsNotEnqueueMessageRule.Contains(keyword) &&
                MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(DeviceKw.HorizontalRule);
            }
        }

        public abstract bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        public abstract ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool);
    }
}
