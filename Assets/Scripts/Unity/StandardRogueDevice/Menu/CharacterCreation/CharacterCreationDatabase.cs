using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using Roguegard;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class CharacterCreationDatabase
    {
        //[SerializeField] private PresetCreationData[] _presets = null;
        //public Spanning<PresetCreationData> Presets => _presets;

        private List<IAppearanceOption> _appearanceOptions = new List<IAppearanceOption>();
        public Spanning<IAppearanceOption> AppearanceOptions => _appearanceOptions;

        private List<IIntrinsicOption> _intrinsicOptions = new List<IIntrinsicOption>();
        public Spanning<IIntrinsicOption> IntrinsicOptions => _intrinsicOptions;

        public void AddAppearanceOption(IAppearanceOption appearanceOption)
        {
            _appearanceOptions.Add(appearanceOption);
        }

        public void AddIntrinsicOption(IIntrinsicOption intrinsicOption)
        {
            _intrinsicOptions.Add(intrinsicOption);
        }
    }
}
