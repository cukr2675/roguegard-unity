using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;
using Roguegard.Rgpacks;

namespace RoguegardUnity
{
    internal class SaveDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;
        private readonly TouchController touchController;
        private readonly SelectFileMenuScreen writeFileMenu;
        private readonly SelectFileMenuScreen readFileMenu;
        private readonly AutoSaveMenu autoSaveMenu;

        private IReadOnlyDictionary<string, object> scenarioRgpack;

        public SaveDeviceEventHandler(StandardRogueDeviceComponentManager componentManager, TouchController touchController)
        {
            this.componentManager = componentManager;
            this.touchController = touchController;

            writeFileMenu = SelectFileMenuScreen.Save(
                onSelectFile: (fileInfo, manager, arg) =>
                {
                    SaveDelay(manager, fileInfo.FullName, false, scenarioRgpack);
                    scenarioRgpack = null;
                },
                onNewFile: (manager, arg) =>
                {
                    manager.PopMenuScreen();

                    StandardRogueDeviceSave.GetNewNumberingPath(
                        RoguegardSettings.DefaultSaveFileName, path =>
                        {
                            SaveDelay(manager, path, false, scenarioRgpack);
                            scenarioRgpack = null;
                        });
                });

            readFileMenu = SelectFileMenuScreen.Load(
                onSelectFile: (fileInfo, manager, arg) =>
                {
                    manager.PopMenuScreen();

                    // 入力されたパスの Stream を開く
                    StandardRogueDeviceData loadDeviceData;
                    using (var stream = RogueFile.OpenRead(fileInfo.FullName))
                    {
                        var save = new StandardRogueDeviceSave();
                        loadDeviceData = save.LoadGameData(stream); // ここで逆シリアル化
                    }
                    manager.Done();

                    // ロードしたデータを適用
                    RogueRandom.Primary = loadDeviceData.CurrentRandom;
                    componentManager.OpenDelay(loadDeviceData);
                });

            autoSaveMenu = new AutoSaveMenu() { parent = this };
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
                touchController.OpenMenu(componentManager.Subject, autoSaveMenu, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.SaveGame)
            {
                if (componentManager.CantSave) return true;

                // 名前を付けてセーブ
                this.scenarioRgpack = null;
                touchController.OpenMenu(componentManager.Subject, writeFileMenu, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.LoadGame)
            {
                // ロード
                touchController.OpenMenu(componentManager.Subject, readFileMenu, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.StartPlaytest && obj is IReadOnlyDictionary<string, object> scenarioRgpack)
            {
                if (componentManager.CantSave) return true;

                // 名前を付けてテストプレイ
                this.scenarioRgpack = scenarioRgpack;
                touchController.OpenMenu(componentManager.Subject, writeFileMenu, null, null, RogueMethodArgument.Identity);
                return true;
            }
            return false;
        }

        private void SaveDelay(MMgr manager, string path, bool autoSave, IReadOnlyDictionary<string, object> scenarioRgpack)
        {
            manager.PopMenuScreen();
            SelectFileMenuScreen.ShowSaving(manager);
            manager.StartCoroutine(Save(manager, path, autoSave, scenarioRgpack));
        }

        private IEnumerator Save(MMgr manager, string path, bool autoSave, IReadOnlyDictionary<string, object> scenarioRgpack)
        {
            // RogueMethodAspectState の処理の完了を待つ
            yield return null;

            // セーブ前処理
            var player = componentManager.Player;
            var subject = componentManager.Subject;
            var maxTurns = 1000;
            var coroutine = TickEnumerator.UpdateTurns(player, subject, maxTurns, maxTurns * 100, true);
            var delayInterval = 250;
            while (coroutine.MoveNext())
            {
                if (coroutine.Current % delayInterval == 0)
                {
                    yield return null;
                }
            }
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

            stream.Close();
            manager?.Done();

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

            if (scenarioRgpack != null)
            {
                var rgpack = new Rgpack("Playtest", scenarioRgpack, Rgpacker.DefaultEvaluator);
                if (!rgpack.TryGetAsset<ScenarioMonolithAsset>("__main", out var monolith)) throw new RogueException();

                var random = new RogueRandom();
                var scenarioDeviceData = new StandardRogueDeviceData();
                scenarioDeviceData.CurrentRandom = random;
                scenarioDeviceData.World = RoguegardSettings.WorldGenerator.CreateObj(null, Vector2Int.zero, random);
                var preset = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0);
                preset.Name = "Playtest";
                var rgpackPlayer = preset.CreateObj(scenarioDeviceData.World, Vector2Int.zero, random);
                RogueDeviceEffect.SetTo(rgpackPlayer);
                ViewInfo.SetTo(rgpackPlayer);
                var worldInfo = RogueWorldInfo.Get(scenarioDeviceData.World);
                worldInfo.LobbyMembers.Add(rgpackPlayer);

                // パーティ・リーダーエフェクト・レベルアップボーナスの初期化
                var party = new RogueParty(rgpackPlayer.Main.InfoSet.Faction, rgpackPlayer.Main.InfoSet.TargetFactions);
                RoguePartyUtility.AssignWithPartyMembers(rgpackPlayer, party);

                RoguePartyUtility.Reset(party, new UseNutritionLeaderEffect());

                scenarioDeviceData.Player = rgpackPlayer;
                scenarioDeviceData.Subject = scenarioDeviceData.Player;
                scenarioDeviceData.Options = data.Options;

                RgpackReference.LoadRgpack(rgpack);
                manager.Done();

                worldInfo.ChartState.PushNext(monolith.MainChartSource);

                // ロードしたデータを適用
                RogueRandom.Primary = scenarioDeviceData.CurrentRandom;
                componentManager.OpenDelay(scenarioDeviceData);
            }
            else
            {
                componentManager.LoadSavePoint(player);
            }
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

        private class AutoSaveMenu : RogueMenuScreen
        {
            public SaveDeviceEventHandler parent;

            public override void OpenScreen(in MMgr inManager, in MArg arg)
            {
                var manager = inManager;
                SelectFileMenuScreen.ShowSaving(manager);
                StandardRogueDeviceSave.GetNewAutoSavePath("AutoSave.gard", path => parent.SaveDelay(manager, path, true, null));
            }
        }
    }
}
