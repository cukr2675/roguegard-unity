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
                var savePointInfo = (ISavePointInfo)obj;
                Save(null, StandardRogueDeviceSave.RootDirectory + "/AutoSave.gard", savePointInfo, true);
                return true;
            }
            if (keyword == DeviceKw.SaveGame)
            {
                // ���O��t���ăZ�[�u
                var savePointInfo = (ISavePointInfo)obj;
                touchController.OpenSelectFile(
                    (root, path) => Save(root, path, savePointInfo, false),
                    (root) => StandardRogueDeviceSave.GetNewNumberingPath(
                        RoguegardSettings.DefaultSaveFileName, path => Save(root, path, savePointInfo, false)));
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

        private void Save(IModelsMenuRoot root, string path, ISavePointInfo savePointInfo, bool autoSave)
        {
            // �Z�[�u�p�f�[�^�𐶐�
            var data = new StandardRogueDeviceData();
            data.Player = componentManager.Player;
            data.Subject = componentManager.Subject;
            data.World = componentManager.World;
            data.Options = componentManager.Options;
            data.CurrentRandom = RogueRandom.Primary;
            data.SavePointInfo = savePointInfo;
            data.SaveDateTime = System.DateTime.UtcNow.ToString();

            // �Z�[�u�f�[�^�e�ʂ����炷
            var view = componentManager.Player.Get<ViewInfo>();
            if (componentManager.Player.Location != view.Location)
            {
                // ��Ԉړ�����ɃZ�[�u�����Ƃ��A�ړ��O�̋�Ԃ̏���ۑ����Ȃ��悤��������
                view.ReadyView(componentManager.Player.Location);
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
                    componentManager.Open(loadDeviceData);
                });
            });
        }
    }
}
