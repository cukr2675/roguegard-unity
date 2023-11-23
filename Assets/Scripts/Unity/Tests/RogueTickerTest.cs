using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using System.IO;
using System.Reflection;
using ObjectFormer;
using ObjectFormer.Serialization.Json;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using RoguegardUnity.Tests;

namespace RoguegardUnity
{
    public class RogueTickerTest : ScriptableObject
    {
        [SerializeField] private RoguegardSettingsData _settings = null;

        [SerializeField] private ScriptableCharacterCreationData _locateSelfTester = null;
        [SerializeField] private ScriptableCharacterCreationData _locateLocationTester = null;

        [Test]
        public void RecursiveCloneAndLocateSelf()
        {
            _settings.TestLoad();
            StaticID.Next();

            RogueDevice.NewGame(new Save() { data = _locateSelfTester });

            var ticker = new GameObject().AddComponent<RogueTicker>();
            ticker.UpdateOnce();
        }

        [Test]
        public void RecursiveCloneAndLocateLocation()
        {
            _settings.TestLoad();
            StaticID.Next();

            RogueDevice.NewGame(new Save() { data = _locateLocationTester });

            var ticker = new GameObject().AddComponent<RogueTicker>();
            ticker.UpdateOnce();
        }

        private class Save : IRogueDeviceSave<Device>
        {
            public ScriptableCharacterCreationData data;

            public Device NewGame()
            {
                var random = new RogueRandom();
                RogueRandom.Primary = random;

                // キャラクターを生成
                var world = data.CreateObj(null, Vector2Int.zero, random);
                var obj = data.CreateObj(world, Vector2Int.zero, random);

                // デバイスを設定
                var device = new Device();
                device.Player = obj;
                return device;
            }

            public Device LoadGame(Stream stream, string name)
            {
                throw new RogueTestException();
            }
        }

        private class Device : IRogueDevice
        {
            public RogueObj Player { get; set; }
            public bool CalledSynchronizedView { get; set; }
            public bool NextStay { get; set; }

            public void AddFloat(IKeyword keyword, float value)
            {
                CalledSynchronizedView = true;
            }

            public void Next()
            {
                Debug.Log($"{nameof(Next)} が実行されました");
                NextStay = true;
            }

            public string Name => "TestDevice";
            public string Version => "1.0.0";
            public string Description => "";
            public void AddInt(IKeyword keyword, int value) { }
            public void AddMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg) { }
            public void AddObject(IKeyword keyword, object obj) { }
            public void AddWork(IKeyword keyword, in RogueCharacterWork work) { }
            public void Close() { }
            public void Update() { }
            public void UpdateCharacters() { }
            public bool VisibleAt(RogueObj location, Vector2Int position) { return false; }
        }
    }
}
