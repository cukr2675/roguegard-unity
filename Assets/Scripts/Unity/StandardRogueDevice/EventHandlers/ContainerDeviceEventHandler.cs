using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class ContainerDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;
        private readonly RogueListuiScreen putIntoContainerScreen;
        private readonly RogueListuiScreen takeOutOfContainerScreen;

        public ContainerDeviceEventHandler(StandardRogueDeviceComponentManager componentManager)
        {
            this.componentManager = componentManager;

            var putInCommandMenuScreen = new PutIntoContainerCommandMenuScreen();
            var takeOutCommandMenuScreen = new TakeOutOfContainerCommandMenuScreen();
            var objsMenu = new ObjsMenu(null, putInCommandMenuScreen, takeOutCommandMenuScreen);

            putIntoContainerScreen = objsMenu.PutIntoContainer;
            takeOutOfContainerScreen = objsMenu.TakeOutOfContainer;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            var player = componentManager.Player;
            if (keyword == StdKw.TakeOutOfContainer && obj is RogueObj takeContainer)
            {
                // 入れ物からアイテムを取り出す
                componentManager.EventManager.AddScreen(takeOutOfContainerScreen, player, null, new(targetObj: takeContainer));
                return true;
            }
            if (keyword == StdKw.PutIntoContainer && obj is RogueObj putContainer)
            {
                // 入れ物へアイテムを入れる
                componentManager.EventManager.AddScreen(putIntoContainerScreen, player, null, new(targetObj: putContainer));
                return true;
            }
            return false;
        }
    }
}
