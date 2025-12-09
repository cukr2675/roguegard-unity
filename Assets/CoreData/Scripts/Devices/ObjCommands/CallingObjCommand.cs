using Lysionium;
using Roguegard.Device;

namespace Roguegard
{
    /// <summary>
    /// 既存の <see cref="IRogueMethod"/> を呼び出す選択肢モデルクラス
    /// </summary>
    public abstract class CallingObjCommand : ReferableScript, IObjCommand
    {
        public abstract string Name { get; }

        ISelectOption<MMgr, MArg> IObjCommand.SelectOption => menuSelectOption ??= SelectOption.Create<MMgr, MArg>(Name, (manager, arg) =>
        {
            var deviceInfo = RogueDeviceEffect.Get(arg.Self);
            deviceInfo.SetDeviceCommand(this, arg.User, arg.Arg);
            manager.Done();
        });
        private SelectOption<MMgr, MArg> menuSelectOption;

        protected void EnqueueMessageRule(RogueObj self, IKeyword keyword)
        {
            if (!RoguegardSettings.KeywordsNotEnqueueMessageRule.Contains(keyword) &&
                MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(DeviceKw.HorizontalRule);
            }
        }

        public abstract bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);

        public abstract ISkillDescribable GetSkillDescribable(RogueObj self, RogueObj tool);
    }
}
