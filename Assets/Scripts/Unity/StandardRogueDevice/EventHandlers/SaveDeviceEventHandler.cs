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
                // �I�[�g�Z�[�u
                SaveDelay(null, StandardRogueDeviceSave.RootDirectory + "/AutoSave.gard", true);
                return true;
            }
            if (keyword == DeviceKw.SaveGame)
            {
                // ���O��t���ăZ�[�u
                touchController.OpenSelectFile(
                    (root, path) => SaveDelay(root, path, false),
                    (root) => StandardRogueDeviceSave.GetNewNumberingPath(
                        RoguegardSettings.DefaultSaveFileName, path => SaveDelay(root, path, false)));
                return true;
            }
            if (keyword == DeviceKw.LoadGame)
            {
                // ���[�h
                OpenLoadInGame();
                return true;
            }
            return false;
        }

        private void SaveDelay(IModelsMenuRoot root, string path, bool autoSave)
        {
            FadeCanvas.StartCanvasCoroutine(Save(root, path, autoSave));
        }

        private IEnumerator Save(IModelsMenuRoot root, string path, bool autoSave)
        {
            // RogueMethodAspectState �̏����̊�����҂�
            yield return null;

            // �Z�[�u�O����
            var player = componentManager.Player;
            var maxTurns = 1000;
            yield return TickEnumerator.UpdateTurns(player, maxTurns, maxTurns * 100, true);
            RemoveNoLobbyMemberLocations(player);
            RemoveViewInfoHeldByLobbyMembers(player);

            // �Z�[�u�p�f�[�^�𐶐�
            var data = new StandardRogueDeviceData();
            data.Player = player;
            data.Subject = componentManager.Subject;
            data.World = componentManager.World;
            data.Options = componentManager.Options;
            data.CurrentRandom = RogueRandom.Primary;
            data.SaveDateTime = System.DateTime.UtcNow.ToString();

            // �Z�[�u�f�[�^�e�ʂ����炷
            var view = player.Get<ViewInfo>();
            if (player.Location != view.Location)
            {
                // ��Ԉړ�����ɃZ�[�u�����Ƃ��A�ړ��O�̋�Ԃ̏���ۑ����Ȃ��悤��������
                view.ReadyView(player.Location);
            }

            var name = RogueFile.GetName(path);
            var stream = RogueFile.Create(path);
            var save = new StandardRogueDeviceSave();
            save.SaveGame(stream, name, data); // �����ŃV���A����
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
            });

            componentManager.LoadSavePoint(player);
        }

        /// <summary>
        /// ���r�[�����o�[����l�����Ȃ���Ԃ��폜����
        /// </summary>
        private void RemoveNoLobbyMemberLocations(RogueObj player)
        {
            var lobbyMembers = LobbyMembers.GetMembersByCharacter(player);
            var world = RogueWorld.GetWorld(player);
            var lobby = RogueWorld.GetLobbyByCharacter(player);
            var locations = world.Space.Objs;
            for (int i = 0; i < locations.Count; i++)
            {
                var location = locations[i];
                if (location == null || location == lobby || ObjsIsIn(lobbyMembers, location)) continue;

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
        /// �v���C���[�ȊO�̃��r�[�����o�[������ <see cref="ViewInfo"/> ���폜����
        /// </summary>
        private void RemoveViewInfoHeldByLobbyMembers(RogueObj player)
        {
            var lobbyMembers = LobbyMembers.GetMembersByCharacter(player);
            for (int i = 0; i < lobbyMembers.Count; i++)
            {
                var member = lobbyMembers[i];
                if (member == null || member == player) continue;

                member.RemoveInfo(typeof(ViewInfo));
            }
        }

        private void OpenLoadInGame()
        {
            touchController.OpenSelectFile((root, path) =>
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // ���͂��ꂽ�p�X�� Stream ���J��
                RogueFile.OpenRead(path, stream =>
                {
                    var save = new StandardRogueDeviceSave();
                    var loadDeviceData = save.LoadGameData(stream); // �����ŋt�V���A����
                    stream.Close();
                    root.Done();

                    // ���[�h�����f�[�^��K�p
                    RogueRandom.Primary = loadDeviceData.CurrentRandom;
                    componentManager.OpenDelay(loadDeviceData);
                });
            });
        }
    }
}
