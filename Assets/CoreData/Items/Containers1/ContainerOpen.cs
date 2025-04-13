using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;
using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class ContainerOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private bool _isStorage;
        [SerializeField] private ScriptField<IApplyRogueMethod> _beOpened;
        [SerializeField] private ScriptField<IApplyRogueMethod> _takeIn;
        [SerializeField] private ScriptField<IApplyRogueMethod> _putOut;

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            var containerInfo = new ContainerInfo(this);
            Roguegard.ContainerInfo.SetInfoTo(self, containerInfo);
            if (_isStorage) { RogueEffectUtility.AddFromInfoSet(self, ValueEffect.Instance); }
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            Roguegard.ContainerInfo.RemoveFrom(self);
            if (_isStorage) { RogueEffectUtility.Remove(self, ValueEffect.Instance); }
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        private class ContainerInfo : IContainerInfo
        {
            private readonly ContainerOpen data;

            public IApplyRogueMethod BeOpened => data._beOpened.Ref ?? baseBeOpened;
            public IApplyRogueMethod TakeIn => data._takeIn.Ref ?? baseTakeIn;
            public IApplyRogueMethod PutOut => data._putOut.Ref ?? basePutOut;

            private static readonly IApplyRogueMethod baseBeOpened = new BeOpenedRogueMethod();
            private static readonly IApplyRogueMethod baseTakeIn = new TakeInRogueMethod();
            private static readonly IApplyRogueMethod basePutOut = new PutOutRogueMethod();

            public ContainerInfo(ContainerOpen data)
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
                    if (arg.Count == 1) { RogueDevice.Add(StdKw.PutIntoContainer, self); }
                    else { RogueDevice.Add(StdKw.TakeOutOfContainer, self); }
                }
                return true;
            }
        }

        private class TakeInRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var item = arg.TargetObj;
                var movement = MovementCalculator.Get(self);

                var result = this.Locate(item, user, self, activationDepth);
                if (!result) return false;

                if (movement.AsStorage)
                {
                    SpaceUtility.Restack(item, StackOption.StackUnlimited);
                }
                return true;
            }
        }

        private class PutOutRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var item = arg.TargetObj;
                var movement = MovementCalculator.Get(self);

                if (movement.AsStorage)
                {
                    var maxStack = item.GetMaxStack(StackOption.Default);
                    if (item.Stack > maxStack)
                    {
                        // 持ちきれないぶんはストレージに残す
                        item = SpaceUtility.Divide(item, maxStack);
                    }
                }
                var result = this.Locate(item, user, user, activationDepth);
                return result;
            }
        }

        private class ValueEffect : IValueEffect
        {
            public static ValueEffect Instance { get; } = new();

            public float Order => 0f;

            public void AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.Movement)
                {
                    value.SubValues[StatsKw.AsStorage] = 1f;
                }
            }
        }
    }
}
