using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class BoneKw : ScriptableLoader
    {
        private static BoneKw instance;

        [SerializeField] private KeywordData _body;
        public static IKeyword Body => instance._body;

        [SerializeField] private KeywordData _effect;
        public static IKeyword Effect => instance._effect;

        [SerializeField] private KeywordData _hair;
        public static IKeyword Hair => instance._hair;

        [SerializeField] private KeywordData _head;
        public static IKeyword Head => instance._head;

        [SerializeField] private KeywordData _leftArm;
        public static IKeyword LeftArm => instance._leftArm;

        [SerializeField] private KeywordData _leftEar;
        public static IKeyword LeftEar => instance._leftEar;

        [SerializeField] private KeywordData _leftEye;
        public static IKeyword LeftEye => instance._leftEye;

        [SerializeField] private KeywordData _leftFoot;
        public static IKeyword LeftFoot => instance._leftFoot;

        [SerializeField] private KeywordData _leftHand;
        public static IKeyword LeftHand => instance._leftHand;

        [SerializeField] private KeywordData _leftLeg;
        public static IKeyword LeftLeg => instance._leftLeg;

        [SerializeField] private KeywordData _leftWing;
        public static IKeyword LeftWing => instance._leftWing;

        [SerializeField] private KeywordData _rightArm;
        public static IKeyword RightArm => instance._rightArm;

        [SerializeField] private KeywordData _rightEar;
        public static IKeyword RightEar => instance._rightEar;

        [SerializeField] private KeywordData _rightEye;
        public static IKeyword RightEye => instance._rightEye;

        [SerializeField] private KeywordData _rightFoot;
        public static IKeyword RightFoot => instance._rightFoot;

        [SerializeField] private KeywordData _rightHand;
        public static IKeyword RightHand => instance._rightHand;

        [SerializeField] private KeywordData _rightLeg;
        public static IKeyword RightLeg => instance._rightLeg;

        [SerializeField] private KeywordData _rightWing;
        public static IKeyword RightWing => instance._rightWing;

        [SerializeField] private KeywordData _tail;
        public static IKeyword Tail => instance._tail;

        [SerializeField] private KeywordData _upperBody;
        public static IKeyword UpperBody => instance._upperBody;

        public override IEnumerator LoadAsync()
        {
            instance = this;
            yield break;
        }

        public override void TestLoad()
        {
#if UNITY_EDITOR
            instance = this;
#else
            throw new RogueException("This method is Editor Only.");
#endif
        }
    }
}
