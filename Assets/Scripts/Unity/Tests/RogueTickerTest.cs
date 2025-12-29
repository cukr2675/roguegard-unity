using Lysionium;
using NUnit.Framework;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using RoguegardUnity.Tests;
using System.IO;
using UnityEngine;

namespace RoguegardUnity
{
    public class RogueTickerTest : ScriptableObject
    {
        [SerializeField] private RoguegardSettingsData _settings = null;

        [SerializeField] private CharacterCreationDataAsset _locateSelfTester = null;
        [SerializeField] private CharacterCreationDataAsset _locateLocationTester = null;

        [Test]
        public void RecursiveCloneAndLocateSelf()
        {
            _settings.TestLoad();
            StaticId.Next();

            RogueDevice.NewGame(new Save() { data = _locateSelfTester });

            var ticker = new GameObject().AddComponent<RogueTicker>();
            ticker.UpdateOnce();
        }

        [Test]
        public void RecursiveCloneAndLocateLocation()
        {
            _settings.TestLoad();
            StaticId.Next();

            RogueDevice.NewGame(new Save() { data = _locateLocationTester });

            var ticker = new GameObject().AddComponent<RogueTicker>();
            ticker.UpdateOnce();
        }

        private class Save : IRogueDeviceSave<Device>
        {
            public CharacterCreationDataAsset data;

            public Device NewGame()
            {
                var random = new RogueRandom();
                RogueRandom.Primary = random;
                MessageWorkListener.ClearListeners();

                // キャラクターを生成
                var world = data.CreateObj(null, Vector2Int.zero, random);
                var obj = data.CreateObj(world, Vector2Int.zero, random);

                // デバイスを設定
                return new Device
                {
                    Player = obj
                };
            }

            public Device LoadGame(Stream stream)
            {
                throw new RogueTestException();
            }
        }

        private class Device : IRogueDevice
        {
            public RogueObj Player { get; set; }
            public RogueObj Subject { get; set; }
            public RogueObj World { get; set; }
            public bool HasSynchronizedWork { get; set; }
            private bool allowStepTurn = true;

            public void AddFloat(IKeyword keyword, float value)
            {
                HasSynchronizedWork = true;
            }

            public void AfterStepTurn()
            {
                Debug.Log($"{nameof(AfterStepTurn)} が実行されました");
                allowStepTurn = false;
            }

            public void AddInt(IKeyword keyword, int value) { }
            public void AddScreen(IListuiScreen<MMgrBase, MArg> screen, RogueObj self, RogueObj user, in RogueMethodArgument arg) { }
            public void AddObject(IKeyword keyword, object obj) { }
            public void AddWork(IKeyword keyword, in RogueCharacterWork work) { }
            public void Close() { }
            public bool UpdateAndGetAllowStepTurn() => allowStepTurn;
        }
    }
}
