using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    public class CharacterCreationPresetAsset
    {
        private readonly CharacterCreationData preset;

        public CharacterCreationPresetAsset(CharacterCreationData preset)
        {
            this.preset = preset;
        }

        public CharacterCreationData LoadPreset()
        {
            return new CharacterCreationData(preset);
        }
    }
}
