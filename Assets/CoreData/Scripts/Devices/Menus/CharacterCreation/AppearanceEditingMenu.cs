using Lysionium;
using Lysionium.MergeExtensions.R3;
using OchalikeSprites;
using Roguegard.CharacterCreation;
using System.Collections.Generic;

namespace Roguegard.Device
{
    public class AppearanceEditingMenu : RogueMenuScreen
    {
        private readonly List<object> list = new();
        private static readonly object addLeftEyeItem = new();
        private static readonly object addRightEyeItem = new();
        private static readonly object addHairItem = new();
        private static readonly object addOtherItem = new();

        public CharacterCreationOptionMenu NextMenu { get; set; }
        public CharacterCreationAddMenu AddMenu { get; set; }

        private readonly ScrollMenuViewData<object, MMgr, MArg> view = new()
        {
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            if (arg.Arg.Other is not CharacterCreationData characterCreationData) throw new System.InvalidOperationException();

            list.Clear();

            if (characterCreationData.Appearances.TryGetValue(BoneKeyword.LeftEye, out var leftEye)) { list.Add(leftEye); }
            else { list.Add(addLeftEyeItem); }

            if (characterCreationData.Appearances.TryGetValue(BoneKeyword.RightEye, out var rightEye)) { list.Add(rightEye); }
            else { list.Add(addRightEyeItem); }

            if (characterCreationData.Appearances.TryGetValue(BoneKeyword.Hair, out var hair)) { list.Add(hair); }
            else { list.Add(addHairItem); }

            for (int i = 0; i < characterCreationData.Appearances.Count; i++)
            {
                var appearance = characterCreationData.Appearances[i];
                if (list.Contains(appearance)) continue;

                list.Add(appearance);
            }
            list.Add(addOtherItem);

            view.Show(list, manager, arg)
                ?
                .Merge(out var merged)
                .Init(
                    () => merged
                    
                    .Case(
                        item => item is Appearance,
                        _ => _
                        .Select(item => (Appearance)item)
                        .NameFrom(appearance => appearance.Name)
                        .OnClick((appearance, manager, arg) => manager.PushMenuScreen(NextMenu, arg.Self, other: appearance)))

                    .Otherwise(
                        _ => _
                        .NameFrom(_ => "+ 見た目を追加")
                        .OnClick((_, manager, arg) => manager.PushMenuScreen(AddMenu, arg.Self, other: typeof(Appearance)))))

                .Build();
        }
    }
}
