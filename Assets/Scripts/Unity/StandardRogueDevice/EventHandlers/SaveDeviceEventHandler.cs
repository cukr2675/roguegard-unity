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
        private readonly SelectFileMenu writeFileMenu;
        private readonly SelectFileMenu readFileMenu;

        public SaveDeviceEventHandler(StandardRogueDeviceComponentManager componentManager, TouchController touchController)
        {
            this.componentManager = componentManager;
            this.touchController = touchController;

            writeFileMenu = new SelectFileMenu(
                SelectFileMenu.Type.Write,
                (root, path) => SaveDelay(root, path, false),
                (root) =>
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                    root.Back();
                    SelectFileMenu.ShowSaving(root);

                    StandardRogueDeviceSave.GetNewNumberingPath(
                        RoguegardSettings.DefaultSaveFileName, path => SaveDelay(root, path, false));
                });

            readFileMenu = new SelectFileMenu(SelectFileMenu.Type.Read, (root, path) =>
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Back();
                SelectFileMenu.ShowLoading(root);

                // 入力されたパスの Stream を開く
                RogueFile.OpenRead(path, (stream, errorMsg) =>
                {
                    if (errorMsg != null)
                    {
                        Debug.LogError(errorMsg);
                        return;
                    }

                    var save = new StandardRogueDeviceSave();
                    var loadDeviceData = save.LoadGameData(stream); // ここで逆シリアル化
                    stream.Close();
                    root.Done();

                    // ロードしたデータを適用
                    RogueRandom.Primary = loadDeviceData.CurrentRandom;
                    componentManager.OpenDelay(loadDeviceData);
                });
            });
        }

        bool IStandardRogueDeviceEventHandler.TryHandle(IKeyword keyword, int integer, float number, object obj)
        {
            if (keyword == DeviceKw.AutoSave)
            {
                if (componentManager.CantSave) return true;

                var subject = componentManager.Subject;
                if (subject != componentManager.Player && (subject.Location == null || subject.Location.Main.Stats.Lv % 5 != 0))
                {
                    // プレイヤーキャラクター以外に注目しているとき、5の倍数の階層のみオートセーブできる
                    // それ以外は何もせず終了
                    return true;
                }

                // オートセーブ
                StandardRogueDeviceSave.GetNewAutoSavePath(
                    "AutoSave.gard", path => SaveDelay(null, path, true));
                return true;
            }
            if (keyword == DeviceKw.SaveGame)
            {
                if (componentManager.CantSave) return true;

                // 名前を付けてセーブ
                touchController.OpenMenu(componentManager.Subject, writeFileMenu, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.LoadGame)
            {
                // ロード
                touchController.OpenMenu(componentManager.Subject, readFileMenu, null, null, RogueMethodArgument.Identity);
                return true;
            }
            return false;
        }

        private void SaveDelay(IModelsMenuRoot root, string path, bool autoSave)
        {
            if (root != null)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Back();
                SelectFileMenu.ShowSaving(root);
            }

            FadeCanvas.StartCanvasCoroutine(Save(root, path, autoSave));
        }

        private IEnumerator Save(IModelsMenuRoot root, string path, bool autoSave)
        {
            // RogueMethodAspectState の処理の完了を待つ
            yield return null;

            // セーブ前処理
            var player = componentManager.Player;
            var subject = componentManager.Subject;
            var maxTurns = 1000;
            yield return TickEnumerator.UpdateTurns(player, subject, maxTurns, maxTurns * 100, true);
            RemoveNoLobbyMemberLocations(player);
            RemoveViewInfoHeldByLobbyMembers(player, subject);
            ClearViewInfoAfterLocate(player);
            ClearViewInfoAfterLocate(subject);

            // セーブ用データを生成
            var data = new StandardRogueDeviceData();
            data.Player = player;
            data.Subject = componentManager.Subject;
            data.World = componentManager.World;
            data.Options = componentManager.Options;
            data.CurrentRandom = RogueRandom.Primary;
            data.SaveDateTime = System.DateTime.UtcNow.ToString();

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

            componentManager.LoadSavePoint(player);
        }

        /// <summary>
        /// ロビーメンバーが一人もいない空間を削除する
        /// </summary>
        private void RemoveNoLobbyMemberLocations(RogueObj player)
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(player);
            var lobbyMembers = worldInfo.LobbyMembers.Members;
            var world = RogueWorldInfo.GetWorld(player);
            var locations = world.Space.Objs;
            for (int i = 0; i < locations.Count; i++)
            {
                var location = locations[i];
                if (location == null || location == worldInfo.Lobby || ObjsIsIn(lobbyMembers, location)) continue;

                location.TrySetStack(0);
                Debug.LogError($"ロビーメンバーがいない空間 {location} を削除しました。");
            }

            bool ObjIsIn(RogueObj obj, RogueObj space)
            {
                var objLocation = obj;
                while (objLocation != null)
                {
                    if (objLocation == space) return true;

                    objLocation = objLocation.Location;
                }
                return false;
            }

            bool ObjsIsIn(Spanning<RogueObj> objs, RogueObj space)
            {
                for (int j = 0; j < objs.Count; j++)
                {
                    if (ObjIsIn(objs[j], space)) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// プレイヤー以外と被写体以外のロビーメンバーが持つ <see cref="ViewInfo"/> を削除する
        /// </summary>
        private void RemoveViewInfoHeldByLobbyMembers(RogueObj player, RogueObj subject)
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(player);
            var lobbyMembers = worldInfo.LobbyMembers.Members;
            for (int i = 0; i < lobbyMembers.Count; i++)
            {
                var member = lobbyMembers[i];
                if (member == null || member == player || member == subject) continue;

                ViewInfo.RemoveFrom(member);
            }
        }

        /// <summary>
        /// 空間移動直後にセーブしたとき、移動前の空間の情報を保存しないよう処理する
        /// </summary>
        private void ClearViewInfoAfterLocate(RogueObj obj)
        {
            if (ViewInfo.TryGet(obj, out var view) && obj.Location != view.Location)
            {
                // 空間移動直後にセーブしたとき、移動前の空間の情報を保存しないよう処理する
                view.ReadyView(obj.Location);
            }
        }
    }
}
