namespace Roguegard.Rgpacks
{
    public class KyarakuriClayBeApplied : BaseApplyRogueMethod
    {
        //private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var info = KyarakuriClayInfo.Get(self);
            if (info == null)
            {
                KyarakuriClayInfo.SetTo(self);
            }

            //menu ??= new();
            //RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self, other: info));
            return false;
        }

        //private class Menu : IListMenu, IViewItemHandler
        //{
        //    private static readonly List<object> list = new List<object>()
        //    {
        //        new AssetId(),
        //        new RaceWeight()
        //    };

        //    public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        var options = manager.GetView(DeviceKw.MenuOptions);
        //        options.OpenView(this, list, manager, self, user, arg);
        //        options.SetPosition(0f);
        //        ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
        //    }

        //    public string GetItemName(object item, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        return SelectOptionPresenter.Instance.GetItemName(item, manager, self, user, arg);
        //    }

        //    public void Click(object item, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //    {
        //        SelectOptionPresenter.Instance.ActivateItem(item, manager, self, user, arg);
        //    }

        //    private class AssetId : IOptionsMenuText
        //    {
        //        public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

        //        public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //            => "アセットID";

        //        public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        {
        //            var kyarakuriClay = arg.TargetObj;
        //            return NamingEffect.Get(kyarakuriClay)?.Naming;
        //        }

        //        public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
        //        {
        //            var kyarakuriClay = arg.TargetObj;
        //            default(IActiveRogueMethodCaller).Affect(kyarakuriClay, 1f, NamingEffect.Callback);
        //            NamingEffect.Get(kyarakuriClay).Naming = value;
        //        }
        //    }

        //    private class RaceWeight : IListMenuSelectOption
        //    {
        //        private static readonly PropertiedCmnMenu nextMenu = new();

        //        public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //            => "重さ";

        //        public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        //        {
        //            var kyarakuriClayInfo = (KyarakuriClayInfo)arg.Other;
        //            manager.OpenMenu(nextMenu, self, null, new(other: kyarakuriClayInfo.RaceWeight));
        //        }
        //    }
        //}
    }
}
