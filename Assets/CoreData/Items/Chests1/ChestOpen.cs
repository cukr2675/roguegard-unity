using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;
using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class ChestOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private bool _isStorage;
        [SerializeField] private ScriptField<IApplyRogueMethod> _beOpened;
        [SerializeField] private ScriptField<IApplyRogueMethod> _takeIn;
        [SerializeField] private ScriptField<IApplyRogueMethod> _putOut;

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            var chestInfo = new ChestInfo(this);
            Roguegard.ChestInfo.SetInfoTo(self, chestInfo);
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            Roguegard.ChestInfo.RemoveFrom(self);
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            if (_isStorage) { Roguegard.ChestInfo.SetStorageTo(self); }
        }

        private class ChestInfo : IChestInfo
        {
            private readonly ChestOpen data;

            public IApplyRogueMethod BeOpened => data._beOpened.Ref ?? baseBeOpened;
            public IApplyRogueMethod TakeIn => data._takeIn.Ref ?? baseTakeIn;
            public IApplyRogueMethod PutOut => data._putOut.Ref ?? basePutOut;

            private static readonly IApplyRogueMethod baseBeOpened = new BeOpenedRogueMethod();
            private static readonly IApplyRogueMethod baseTakeIn = new TakeInRogueMethod();
            private static readonly IApplyRogueMethod basePutOut = new PutOutRogueMethod();

            public ChestInfo(ChestOpen data)
            {
                this.data = data;
            }
        }

        private class BeOpenedRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (RogueDevice.Primary.Player == user)
                {
                    if (arg.Count == 1) { RogueDevice.Add(StdKw.PutIntoChest, self); }
                    else { RogueDevice.Add(StdKw.TakeOutFromChest, self); }
                }
                return true;
            }
        }

        private class TakeInRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var item = arg.TargetObj;
                var chestInfo = Roguegard.ChestInfo.GetInfo(self);
                var storageObjs = Roguegard.ChestInfo.GetStorage(self);
                if (chestInfo != null && storageObjs != null)
                {
                    var result = this.Locate(item, user, null, activationDepth);
                    if (result)
                    {
                        var maxStack = item.GetMaxStack(StackOption.StackUnlimited);
                        storageObjs.Stack(item, Vector2Int.zero, maxStack);
                        if (item.Stack >= 1)
                        {
                            storageObjs.Add(item);
                        }
                    }
                    return result;
                }
                else
                {
                    var result = this.Locate(item, user, self, activationDepth);
                    return result;
                }
            }
        }

        private class PutOutRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var item = arg.TargetObj;
                var chestInfo = Roguegard.ChestInfo.GetInfo(self);
                var storageObjs = Roguegard.ChestInfo.GetStorage(self);
                if (chestInfo != null && storageObjs != null)
                {
                    var maxStack = item.GetMaxStack(StackOption.Default);
                    if (item.Stack > maxStack)
                    {
                        // 持ちきれないぶんはストレージに残す
                        item = SpaceUtility.Divide(item, maxStack);
                    }
                }
                var result = this.Locate(item, user, user, activationDepth);
                if (result)
                {
                    // 移動に成功したとき、ストレージ内のオブジェクトを消す。
                    // 持ちきれないぶんをストレージに残す場合は消さない。
                    storageObjs.Remove(item);
                }

                return result;
            }
        }
    }
}
