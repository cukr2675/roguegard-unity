using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class StandardRogueDevice : IRogueDevice
    {
        public string Name => "StandardRogueDevice";

        public string Version => "0.1";

        public string Description => "";

        RogueObj IRogueDevice.Player => componentManager.Player;
        bool IRogueDevice.CalledSynchronizedView => componentManager.EventManager.CalledSynchronizedView;
        bool IRogueDevice.NextStay => componentManager.NextStay;

        public RogueOptions Options => componentManager.Options;

        [System.NonSerialized] private StandardRogueDeviceComponentManager componentManager;
        [System.NonSerialized] private StandardRogueDeviceData data;

        public StandardRogueDevice(StandardRogueDeviceData data)
        {
            this.data = data;
        }

        public void GetInfo(out IRogueRandom random)
        {
            if (data == null) throw new RogueException();

            random = data.CurrentRandom;
        }

        public void Open(
            RogueSpriteRendererPool spriteRendererPool,
            RogueTilemapRenderer tilemapRendererPrefab,
            TouchController touchControllerPrefab,
            SoundTable soundTable,
            AudioMixer audioMixer,
            AudioSource seAudioSourcePrefab,
            AudioSource bgmAudioSourcePrefab)
        {
            if (!data.Player.TryGet<ViewInfo>(out _))
            {
                data.Player.SetInfo(new ViewInfo());
            }

            var name = "StandardDevice";
            componentManager = new StandardRogueDeviceComponentManager();
            componentManager.Initialize(
                name, spriteRendererPool, tilemapRendererPrefab, touchControllerPrefab,
                soundTable, audioMixer, seAudioSourcePrefab, bgmAudioSourcePrefab);
            componentManager.Open(data);
            data = null;
        }

        void IRogueDevice.Close() => componentManager.Close();
        void IRogueDevice.Next() => componentManager.Next();
        void IRogueDevice.Update() => componentManager.Update();
        void IRogueDevice.UpdateCharacters() => componentManager.UpdateCharacters();

        private void Add(IKeyword keyword, int integer = 0, float number = 0f, object obj = null)
            => componentManager.EventManager.Add(keyword, integer, number, obj);
        void IRogueDevice.AddInt(IKeyword keyword, int value) => Add(keyword, integer: value);
        void IRogueDevice.AddFloat(IKeyword keyword, float value) => Add(keyword, number: value);
        void IRogueDevice.AddObject(IKeyword keyword, object obj) => Add(keyword, obj: obj);
        void IRogueDevice.AddWork(IKeyword keyword, in RogueCharacterWork work)
            => componentManager.EventManager.AddWork(componentManager.Player, keyword, work, componentManager.FastForward);
        void IRogueDevice.AddMenu(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            => componentManager.EventManager.AddMenu(menu, self, user, arg);

        bool IRogueDevice.VisibleAt(RogueObj location, Vector2Int position)
        {
            if (!componentManager.TargetObj.TryGet<ViewInfo>(out var view))
            {
                return componentManager.TargetObj.Location == location;
            }
            if (view.Location != location || view.Location.Space.Tilemap == null) return false;

            view.GetTile(position, out var visible, out _, out _);
            if (visible) return true;

            // ペイントされているオブジェクトがいる位置は見える
            var obj = view.Location.Space.GetColliderObj(position);
            if (obj != null)
            {
                var objStatusEffectState = obj.Main.GetStatusEffectState(obj);
                if (objStatusEffectState.TryGetStatusEffect<PaintStatusEffect>(out _)) return true;
            }

            // 視界範囲外の判定が出ても、更新してもう一度試す
            // 出会いがしらの敵を表示する際に有効
            view.AddView(componentManager.TargetObj);
            view.GetTile(position, out visible, out _, out _);
            return visible;
        }
    }
}
