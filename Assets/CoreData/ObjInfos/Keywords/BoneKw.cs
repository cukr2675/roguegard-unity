using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class BoneKw : ScriptableLoader
    {
        private static BoneKw instance;

        [SerializeField] private SerializableKeywordData _body;
        public static ISerializableKeyword Body => instance._body;

        [SerializeField] private SerializableKeywordData _effect;
        public static ISerializableKeyword Effect => instance._effect;

        [SerializeField] private SerializableKeywordData _hair;
        public static ISerializableKeyword Hair => instance._hair;

        [SerializeField] private SerializableKeywordData _head;
        public static ISerializableKeyword Head => instance._head;

        [SerializeField] private SerializableKeywordData _leftArm;
        public static ISerializableKeyword LeftArm => instance._leftArm;

        [SerializeField] private SerializableKeywordData _leftEar;
        public static ISerializableKeyword LeftEar => instance._leftEar;

        [SerializeField] private SerializableKeywordData _leftEye;
        public static ISerializableKeyword LeftEye => instance._leftEye;

        [SerializeField] private SerializableKeywordData _leftFoot;
        public static ISerializableKeyword LeftFoot => instance._leftFoot;

        [SerializeField] private SerializableKeywordData _leftHand;
        public static ISerializableKeyword LeftHand => instance._leftHand;

        [SerializeField] private SerializableKeywordData _leftLeg;
        public static ISerializableKeyword LeftLeg => instance._leftLeg;

        [SerializeField] private SerializableKeywordData _leftWing;
        public static ISerializableKeyword LeftWing => instance._leftWing;

        [SerializeField] private SerializableKeywordData _rightArm;
        public static ISerializableKeyword RightArm => instance._rightArm;

        [SerializeField] private SerializableKeywordData _rightEar;
        public static ISerializableKeyword RightEar => instance._rightEar;

        [SerializeField] private SerializableKeywordData _rightEye;
        public static ISerializableKeyword RightEye => instance._rightEye;

        [SerializeField] private SerializableKeywordData _rightFoot;
        public static ISerializableKeyword RightFoot => instance._rightFoot;

        [SerializeField] private SerializableKeywordData _rightHand;
        public static ISerializableKeyword RightHand => instance._rightHand;

        [SerializeField] private SerializableKeywordData _rightLeg;
        public static ISerializableKeyword RightLeg => instance._rightLeg;

        [SerializeField] private SerializableKeywordData _rightWing;
        public static ISerializableKeyword RightWing => instance._rightWing;

        [SerializeField] private SerializableKeywordData _tail;
        public static ISerializableKeyword Tail => instance._tail;

        [SerializeField] private SerializableKeywordData _upperBody;
        public static ISerializableKeyword UpperBody => instance._upperBody;

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
