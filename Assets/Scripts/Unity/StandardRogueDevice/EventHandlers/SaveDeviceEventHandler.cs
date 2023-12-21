using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SaveDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;
        private readonly TouchController touchController;

        public SaveDeviceEventHandler(StandardRogueDeviceComponentManager componentManager, TouchController touchController)
        {
            this.componentManager = componentManager;
            this.touchController = touchController;
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            if (keyword == DeviceKw.AutoSave)
            {
                // オートセーブ
                var savePointInfo = (ISavePointInfo)obj;
                Save(null, StandardRogueDeviceSave.RootDirectory + "/AutoSave.gard", savePointInfo, true);
                return true;
            }
            if (keyword == DeviceKw.SaveGame)
            {
                // 名前を付けてセーブ
                var savePointInfo = (ISavePointInfo)obj;
                touchController.OpenSelectFile(
                    (root, path) => Save(root, path, savePointInfo, false),
                    (root) => StandardRogueDeviceSave.GetNewNumberingPath(
                        RoguegardSettings.DefaultSaveFileName, path => Save(root, path, savePointInfo, false)));
                return true;
            }
            if (keyword == DeviceKw.LoadGame)
            {
                // ロード
                OpenLoadInGame();
                return true;
            }
            return false;
        }

        private void Save(IModelsMenuRoot root, string path, ISavePointInfo savePointInfo, bool autoSave)
        {
            // セーブ用データを生成
            var data = new StandardRogueDeviceData();
            data.Player = componentManager.Player;
            data.Subject = componentManager.Subject;
            data.World = componentManager.World;
            data.Options = componentManager.Options;
            data.CurrentRandom = RogueRandom.Primary;
            data.SavePointInfo = savePointInfo;
            data.SaveDateTime = System.DateTime.UtcNow.ToString();

            // セーブデータ容量を減らす
            var view = componentManager.Player.Get<ViewInfo>();
            if (componentManager.Player.Location != view.Location)
            {
                // 空間移動直後にセーブしたとき、移動前の空間の情報を保存しないよう処理する
                view.ReadyView(componentManager.Player.Location);
            }

            var name = RogueFile.GetName(path);
            var stream = RogueFile.Create(path);
            var save = new StandardRogueDeviceSave();
            save.SaveGame(stream, name, data); // ここでシリアル化
            stream.Save(() =>
            {
                stream.Close();
                root?.Done();

                // セーブ完了メッセージを表示
                if (autoSave)
                {
                    RogueDevice.Add(DeviceKw.AppendText, path);
                    RogueDevice.Add(DeviceKw.AppendText, "にオートセーブしました\n");
                }
                else
                {
                    RogueDevice.Add(DeviceKw.AppendText, path);
                    RogueDevice.Add(DeviceKw.AppendText, "にセーブしました\n");
                }
            });
        }

        private void OpenLoadInGame()
        {
            touchController.OpenSelectFile((root, path) =>
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // 入力されたパスの Stream を開く
                RogueFile.OpenRead(path, stream =>
                {
                    var save = new StandardRogueDeviceSave();
                    var loadDeviceData = save.LoadGameData(stream); // ここで逆シリアル化
                    stream.Close();
                    root.Done();

                    // ロードしたデータを適用
                    RogueRandom.Primary = loadDeviceData.CurrentRandom;
                    componentManager.Open(loadDeviceData);
                });
            });
        }
    }
}
