using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class ContainerDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;
        private readonly RogueMenuScreen putIntoContainerMenu;
        private readonly RogueMenuScreen takeOutOfContainerMenu;

        public ContainerDeviceEventHandler(StandardRogueDeviceComponentManager componentManager)
        {
            this.componentManager = componentManager;

            var putInCommandMenu = new PutIntoContainerCommandMenu();
            var takeOutCommandMenu = new TakeOutOfContainerCommandMenu();
            var objsMenu = new ObjsMenu(null, putInCommandMenu, takeOutCommandMenu);

            putIntoContainerMenu = objsMenu.PutIntoContainer;
            takeOutOfContainerMenu = objsMenu.TakeOutOfContainer;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            var player = componentManager.Player;
            if (keyword == StdKw.TakeOutOfContainer && obj is RogueObj takeContainer)
            {
                // 入れ物からアイテムを取り出す
                componentManager.EventManager.AddMenu(takeOutOfContainerMenu, player, null, new(targetObj: takeContainer));
                return true;
            }
            if (keyword == StdKw.PutIntoContainer && obj is RogueObj putContainer)
            {
                // 入れ物へアイテムを入れる
                componentManager.EventManager.AddMenu(putIntoContainerMenu, player, null, new(targetObj: putContainer));
                return true;
            }
            return false;
        }
    }
}
