using Lysionium;
using Roguegard;
using Roguegard.Device;
using UnityEngine.Audio;

namespace RoguegardUnity
{
    public class StandardRogueDevice : IRogueDevice
    {
        RogueObj IRogueDevice.Player => componentManager.Player;
        RogueObj IRogueDevice.Subject => componentManager.Subject;
        RogueObj IRogueDevice.World => componentManager.World;
        bool IRogueDevice.HasSynchronizedWork => componentManager.EventManager.HasSynchronizedWork;

        public RogueOptions Options => componentManager.Options;

        [System.NonSerialized] private StandardRogueDeviceComponentManager componentManager;
        [System.NonSerialized] private StandardRogueDeviceData data;

        public StandardRogueDevice(StandardRogueDeviceData data)
        {
            this.data = data;
        }

        public void GetInfo(out IRogueRandom random)
        {
            if (data == null) throw new System.InvalidOperationException();

            random = data.CurrentRandom;
        }

        public void Open(
            RogueSpriteRendererPool spriteRendererPool,
            RogueTilemapRenderer tilemapRendererPrefab,
            TouchController touchControllerPrefab,
            AudioMixer audioMixer,
            StandardRogueDeviceInspector runtimeInspectorPrefab)
        {
            var name = "StandardDevice";
            componentManager = new StandardRogueDeviceComponentManager();
            componentManager.Initialize(
                name, spriteRendererPool, tilemapRendererPrefab, touchControllerPrefab, audioMixer, runtimeInspectorPrefab);
            componentManager.OpenDelay(data);
            data = null;
        }

        void IRogueDevice.Close() => componentManager.Close();
        bool IRogueDevice.UpdateAndGetAllowStepTurn() => componentManager.UpdateAndGetAllowStepTurn();
        void IRogueDevice.AfterStepTurn() => componentManager.AfterStepTurn();

        private void Add(IKeyword keyword, int integer = 0, float number = 0f, object obj = null)
            => componentManager.EventManager.Add(keyword, integer, number, obj);
        void IRogueDevice.AddInt(IKeyword keyword, int value) => Add(keyword, integer: value);
        void IRogueDevice.AddFloat(IKeyword keyword, float value) => Add(keyword, number: value);
        void IRogueDevice.AddObject(IKeyword keyword, object obj) => Add(keyword, obj: obj);
        void IRogueDevice.AddWork(IKeyword keyword, in RogueCharacterWork work)
            => componentManager.EventManager.AddWork(componentManager.Player, keyword, work, componentManager.FastForward);
        void IRogueDevice.AddScreen(IListuiScreen<MMgrBase, MArg> screen, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            => componentManager.EventManager.AddScreen(screen, self, user, arg);
    }
}
