using Lysionium;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public static class RgpackMenuUtility
    {
        public static object[] AssetIDInputField =
            new object[]
            {
                "アセットID",
                InputFieldWidgetOption.Create<MMgr, MArg>(
                    (manager, arg) => NamingEffect.Get(arg.Arg.TargetObj)?.Naming,
                    (manager, arg, value) => {
                        var fairy = arg.Arg.TargetObj;
                        default(IActiveRogueMethodCaller).Affect(fairy, 1f, NamingEffect.Callback);
                        return NamingEffect.Get(fairy).Naming = value;
                    })
            };
    }
}
