using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class LobbyOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private ScriptableStartingItem _lobbyMerchant = null;

        private Updater updater;

        public IRaceOption Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            updater ??= new Updater() { parent = this };

            RogueEffectUtility.AddFromInfoSet(self, updater);
            return raceOption;
        }

        public IRaceOption Reopen(RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            RogueEffectUtility.Remove(self, updater);
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        private class Updater : IRogueObjUpdater
        {
            public LobbyOpen parent;

            public float Order => 100f;

            public RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                var spaceObjs = self.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    var obj = spaceObjs[i];
                    if (obj == null) continue;

                    if (obj.Main.InfoSet.Equals(parent._lobbyMerchant.Option.PrimaryInfoSet)) return default;
                }

                // ¤l‚ª‚¢‚È‚©‚Á‚½‚ç¶¬‚·‚é
                var random = RogueRandom.Primary;
                var position = self.Space.GetRandomPositionInRoom(random);
                parent._lobbyMerchant.Option.CreateObj(parent._lobbyMerchant, self, position, random);
                return default;
            }
        }
    }
}
