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
        private readonly SelectFileMenu writeFileMenu;
        private readonly SelectFileMenu readFileMenu;
        private readonly AutoSaveMenu autoSaveMenu;

        private IReadOnlyDictionary<string, object> spQuestRgpack;

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

                // ���͂��ꂽ�p�X�� Stream ���J��
                RogueFile.OpenRead(path, (stream, errorMsg) =>
                {
                    if (errorMsg != null)
                    {
                        Debug.LogError(errorMsg);
                        return;
                    }

                    var save = new StandardRogueDeviceSave();
                    var loadDeviceData = save.LoadGameData(stream); // �����ŋt�V���A����
                    stream.Close();
                    root.Done();

                    // ���[�h�����f�[�^��K�p
                    RogueRandom.Primary = loadDeviceData.CurrentRandom;
                    componentManager.OpenDelay(loadDeviceData);
                });
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
                    // �v���C���[�L�����N�^�[�ȊO�ɒ��ڂ��Ă���Ƃ��A5�̔{���̊K�w�̂݃I�[�g�Z�[�u�ł���
                    // ����ȊO�͉��������I��
                    return true;
                }

                // �I�[�g�Z�[�u
                touchController.OpenMenu(componentManager.Subject, autoSaveMenu, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.SaveGame)
            {
                if (componentManager.CantSave) return true;

                // ���O��t���ăZ�[�u
                this.spQuestRgpack = null;
                touchController.OpenMenu(componentManager.Subject, writeFileMenu, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.LoadGame)
            {
                // ���[�h
                touchController.OpenMenu(componentManager.Subject, readFileMenu, null, null, RogueMethodArgument.Identity);
                return true;
            }
            if (keyword == DeviceKw.StartPlaytest && obj is IReadOnlyDictionary<string, object> spQuestRgpack)
            {
                if (componentManager.CantSave) return true;

                // ���O��t���ăe�X�g�v���C
                this.spQuestRgpack = spQuestRgpack;
                touchController.OpenMenu(componentManager.Subject, writeFileMenu, null, null, RogueMethodArgument.Identity);
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
            // RogueMethodAspectState �̏����̊�����҂�
            yield return null;

            // �Z�[�u�O����
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

            // �Z�[�u�p�f�[�^�𐶐�
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
            save.SaveGame(stream, name, data); // �����ŃV���A����
            var loadRgpack = spQuestRgpack;
            spQuestRgpack = null;
            stream.Save(() =>
            {
                stream.Close();
                root?.Done();

                // �Z�[�u�������b�Z�[�W��\��
                if (autoSave)
                {
                    RogueDevice.Add(DeviceKw.AppendText, path);
                    RogueDevice.Add(DeviceKw.AppendText, "�ɃI�[�g�Z�[�u���܂���\n");
                }
                else
                {
                    RogueDevice.Add(DeviceKw.AppendText, path);
                    RogueDevice.Add(DeviceKw.AppendText, "�ɃZ�[�u���܂���\n");
                }

                if (loadRgpack != null)
                {
                    var rgpack = new Rgpack("Playtest", loadRgpack, Rgpacker.DefaultEvaluator);
                    if (!rgpack.TryGetAsset<SpQuestMonolithAsset>("__main", out var monolith)) throw new RogueException();

                    var save = new StandardRogueDeviceSave();
                    var random = new RogueRandom();
                    var spQuestDeviceData = new StandardRogueDeviceData();
                    spQuestDeviceData.CurrentRandom = random;
                    spQuestDeviceData.World = RoguegardSettings.WorldGenerator.CreateObj(null, Vector2Int.zero, random);
                    var player = RoguegardSettings.CharacterCreationDatabase.LoadPreset(0).CreateObj(spQuestDeviceData.World, Vector2Int.zero, random);
                    RogueDeviceEffect.SetTo(player);
                    ViewInfo.SetTo(player);
                    var worldInfo = RogueWorldInfo.GetByCharacter(player);
                    worldInfo.LobbyMembers.Add(player);

                    // �p�[�e�B�E���[�_�[�G�t�F�N�g�E���x���A�b�v�{�[�i�X�̏�����
                    var party = new RogueParty(player.Main.InfoSet.Faction, player.Main.InfoSet.TargetFactions);
                    RoguePartyUtility.AssignWithPartyMembers(player, party);

                    RoguePartyUtility.Reset(party, new UseNutritionLeaderEffect());

                    spQuestDeviceData.Player = player;
                    spQuestDeviceData.Subject = spQuestDeviceData.Player;
                    spQuestDeviceData.Options = data.Options;

                    RgpackReference.LoadRgpack(rgpack);
                    root.Done();

                    worldInfo.ChartState.PushNext(monolith.MainChartSource);

                    // ���[�h�����f�[�^��K�p
                    RogueRandom.Primary = spQuestDeviceData.CurrentRandom;
                    componentManager.OpenDelay(spQuestDeviceData);
                }
            });

            if (loadRgpack == null) { componentManager.LoadSavePoint(player); }
        }

        /// <summary>
        /// ���r�[�����o�[����l�����Ȃ���Ԃ��폜����
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
                Debug.LogError($"���r�[�����o�[�����Ȃ���� {location} ���폜���܂����B");
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
        /// �v���C���[�ȊO�Ɣ�ʑ̈ȊO�̃��r�[�����o�[������ <see cref="ViewInfo"/> ���폜����
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
        /// ��Ԉړ�����ɃZ�[�u�����Ƃ��A�ړ��O�̋�Ԃ̏���ۑ����Ȃ��悤��������
        /// </summary>
        private void ClearViewInfoAfterLocate(RogueObj obj)
        {
            if (ViewInfo.TryGet(obj, out var view) && obj.Location != view.Location)
            {
                // ��Ԉړ�����ɃZ�[�u�����Ƃ��A�ړ��O�̋�Ԃ̏���ۑ����Ȃ��悤��������
                view.ReadyView(obj.Location);
            }
        }

        private class AutoSaveMenu : IModelsMenu
        {
            public SaveDeviceEventHandler parent;

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                SelectFileMenu.ShowSaving(root);
                StandardRogueDeviceSave.GetNewAutoSavePath("AutoSave.gard", path => parent.SaveDelay(root, path, true));
            }
        }
    }
}
