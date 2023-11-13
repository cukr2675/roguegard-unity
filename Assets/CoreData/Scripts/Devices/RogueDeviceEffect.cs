using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    [ObjectFormer.Formable]
    public class RogueDeviceEffect : IRogueEffect, IRogueObjUpdater
    {
        [System.NonSerialized] private IDeviceCommandAction commandAction;
        [System.NonSerialized] private RogueObj commandUser;
        [System.NonSerialized] private RogueMethodArgument commandArg;

        float IRogueObjUpdater.Order => 0f;

        private RogueDeviceEffect() { }

        public static RogueDeviceEffect Get(RogueObj obj)
        {
            if (obj.Main.RogueEffects.TryGetEffect<RogueDeviceEffect>(out var effect)) return effect;
            else return null;
        }

        public static void SetTo(RogueObj obj)
        {
            var effect = new RogueDeviceEffect();
            obj.Main.RogueEffects.AddOpen(obj, effect);
        }

        public void Close(RogueObj obj)
        {
            RogueEffectUtility.RemoveClose(obj, this);
        }

        public void SetDeviceCommand(IDeviceCommandAction action, RogueObj user, in RogueMethodArgument arg)
        {
            if (commandAction != null) throw new RogueException($"{nameof(commandAction)} を上書きすることはできません。");

            commandAction = action;
            commandUser = user;
            commandArg = arg;
        }

        public void ClearDeviceCommand()
        {
            commandAction = null;
        }

        void IRogueEffect.Open(RogueObj self)
        {
            RogueEffectUtility.AddFromRogueEffect(self, this);
        }

        RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            RogueObjUpdaterContinueType result;
            if (sectionIndex == 0)
            {
                result = TickUtility.Section0Update(self, ref sectionIndex, true);
            }
            else
            {
                var activeResult = false;
                if (commandAction != null)
                {
                    // コマンドが設定されていたら実行する。
                    activeResult = commandAction.CommandInvoke(self, commandUser, activationDepth, commandArg);
                    commandAction = null;
                }
                if (activeResult)
                {
                    // Active の発動に成功したらインデックスを進める。
                    result = TickUtility.SectionAfter1LateUpdate(self, ref sectionIndex);
                }
                else
                {
                    // Active の発動に失敗したらインデックスを進めずに待機する。
                    // 待機する場合でも待機用 Active を発動する必要がある。（入力を待機するため）
                    RogueDevice.Add(DeviceKw.WaitForInput, null);
                    result = RogueObjUpdaterContinueType.Continue;
                }
            }
            return result;
        }

        bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
        IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
        IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => null;
    }
}
