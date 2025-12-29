using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;
using Roguegard.Rgpacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    internal class SaveDeviceEventHandler : IStandardRogueDeviceEventHandler
    {
        private readonly StandardRogueDeviceComponentManager componentManager;
        private readonly TouchController touchController;
        private readonly FileSelectionScreen writeFileScreen;
        private readonly FileSelectionScreen readFileScreen;
        private readonly AutoSaveScreen autoSaveScreen;

        private IReadOnlyDictionary<string, object> scenarioRgpack;

        public SaveDeviceEventHandler(StandardRogueDeviceComponentManager componentManager, TouchController touchController)
        {
            this.componentManager = componentManager;
            this.touchController = touchController;

            writeFileScreen = FileSelectionScreen.Save(
                onSelectFile: (fileInfo, manager, arg) =>
                {
                    SaveDelay(manager, fileInfo.FullName, false, scenarioRgpack);
                    scenarioRgpack = null;
                },
                onNewFile: (manager, arg) =>
                {
                    manager.PopScreen();

                    StandardRogueDeviceSave.GetNewNumberingPath(
                        RoguegardSettings.DefaultSaveFileName, path =>
                        {
                            SaveDelay(manager, path, false, scenarioRgpack);
                            scenarioRgpack = null;
                        });
                });

            readFileScreen = FileSelectionScreen.Load(
                onSelectFile: (fileInfo, manager, arg) =>
                {
                    manager.PopScreen();

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

            autoSaveScreen = new AutoSaveScreen() { parent = this };
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
                touchController.OpenScreen(componentManager.Subject, autoSaveScreen, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.SaveGame)
            {
                if (componentManager.CantSave) return true;

                // 名前を付けてセーブ
                this.scenarioRgpack = null;
                touchController.OpenScreen(componentManager.Subject, writeFileScreen, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.LoadGame)
            {
                // ロード
                touchController.OpenScreen(componentManager.Subject, readFileScreen, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.StartPlaytest && obj is IReadOnlyDictionary<string, object> scenarioRgpack)
            {
                if (componentManager.CantSave) return true;

                // 名前を付けてテストプレイ
                this.scenarioRgpack = scenarioRgpack;
                touchController.OpenScreen(componentManager.Subject, writeFileScreen, null, null, RogueMethodArgument.Identity);
                return true;
            }
            return false;
        }

        private void SaveDelay(MMgr manager, string path, bool autoSave, IReadOnlyDictionary<string, object> scenarioRgpack)
        {
            manager.PopScreen();
            FileSelectionScreen.ShowSaving(manager);
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
            var data = new StandardRogueDeviceData
            {
                Player = player,
                Subject = componentManager.Subject,
                World = componentManager.World,
                Options = componentManager.Options,
                CurrentRandom = RogueRandom.Primary,
                SaveDateTime = System.DateTime.UtcNow.ToString()
            };

            var name = RogueFile.GetName(path);
            var stream = RogueFile.Create(path);
            var save = new StandardRogueDeviceSave();
            save.SaveGame(stream, name, data); // ここでシリアル化

            stream.Close();
            if (manager != null) { manager.Done(); }

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
                if (!rgpack.TryGetAsset<ScenarioMonolithAsset>("__main", out var monolith)) throw new System.InvalidOperationException();

                var random = new RogueRandom();
                var scenarioDeviceData = new StandardRogueDeviceData
                {
                    CurrentRandom = random,
                    World = RoguegardSettings.WorldGenerator.CreateObj(null, Vector2Int.zero, random)
                };
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
            foreach (var location in locations)
            {
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
                foreach (var obj in objs)
                {
                    if (ObjIsIn(obj, space)) return true;
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
            foreach (var member in worldInfo.LobbyMembers.Members)
            {
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

        private class AutoSaveScreen : RogueListuiScreen
        {
            public SaveDeviceEventHandler parent;

            public override void OpenScreen(MMgr inManager, MArg arg)
            {
                var manager = inManager;
                FileSelectionScreen.ShowSaving(manager);
                StandardRogueDeviceSave.GetNewAutoSavePath("AutoSave.gard", path => parent.SaveDelay(manager, path, true, null));
            }
        }
    }
}
