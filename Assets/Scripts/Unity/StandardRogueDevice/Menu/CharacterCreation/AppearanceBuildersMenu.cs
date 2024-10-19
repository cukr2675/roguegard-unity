//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using SDSSprite;
//using Roguegard.CharacterCreation;
//using Roguegard.Device;
//using Roguegard;

//namespace RoguegardUnity
//{
//    public class AppearanceBuildersMenu : BaseScrollListMenu<object>
//    {
//        private readonly List<object> elms = new();
//        private static readonly object addLeftEyeElement = new object();
//        private static readonly object addRightEyeElement = new object();
//        private static readonly object addHairElement = new object();
//        private static readonly object addOtherElement = new object();

//        public CharacterCreationOptionMenu NextMenu { get; set; }
//        public CharacterCreationAddMenu AddMenu { get; set; }

//        protected override Spanning<object> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//        {
//            if (!(arg.Other is CharacterCreationDataBuilder builder)) throw new RogueException();

//            elms.Clear();

//            if (builder.Appearances.TryGetBuilder(BoneKeyword.LeftEye, out var leftEyeBuilder))
//            {
//                elms.Add(leftEyeBuilder);
//            }
//            else
//            {
//                elms.Add(addLeftEyeElement);
//            }

//            if (builder.Appearances.TryGetBuilder(BoneKeyword.RightEye, out var rightEyeBuilder))
//            {
//                elms.Add(rightEyeBuilder);
//            }
//            else
//            {
//                elms.Add(addRightEyeElement);
//            }

//            if (builder.Appearances.TryGetBuilder(BoneKeyword.Hair, out var hairBuilder))
//            {
//                elms.Add(hairBuilder);
//            }
//            else
//            {
//                elms.Add(addHairElement);
//            }

//            for (int i = 0; i < builder.Appearances.Count; i++)
//            {
//                var appearanceBuilder = builder.Appearances[i];
//                if (elms.Contains(appearanceBuilder)) continue;

//                elms.Add(appearanceBuilder);
//            }
//            elms.Add(addOtherElement);
//            return elms;
//        }

//        protected override string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//        {
//            if (element is AppearanceBuilder builder)
//            {
//                return builder.Name;
//            }
//            else
//            {
//                return "+ Œ©‚½–Ú‚ð’Ç‰Á";
//            }
//        }

//        protected override void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//        {
//            if (element is AppearanceBuilder builder)
//            {
//                manager.OpenMenu(NextMenu, self, null, new(other: builder));
//            }
//            else
//            {
//                manager.OpenMenu(AddMenu, self, null, new(other: typeof(AppearanceBuilder)));
//            }
//        }
//    }
//}
