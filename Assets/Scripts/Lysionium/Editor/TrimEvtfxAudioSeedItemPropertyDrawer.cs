using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Lysionium.Audio.Editor
{
    [CustomPropertyDrawer(typeof(TrimEvtfxAudioSeed.Item))]
    public class TrimEvtfxAudioSeedItemPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, true);

            if (GUILayout.Button("Test Play"))
            {
                var originalClip = (AudioClip)property.FindPropertyRelative("_originalClip").objectReferenceValue;
                var startSample = property.FindPropertyRelative("_startSample").intValue;
                var endSample = property.FindPropertyRelative("_endSample").intValue;
                startSample = GetSubSample(startSample, originalClip);
                endSample = GetSubSample(endSample, originalClip);
                TestPlay(originalClip, startSample, endSample);
            }
        }

        private void TestPlay(AudioClip originalClip, int startSample, int endSample)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var playPreviewClip = audioUtilClass.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );
            var stopAllPreviewClips = audioUtilClass.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public
            );
            if (playPreviewClip == null) { Debug.LogError("AudioUtil.PlayPreviewClip method not found. Unity version may have changed."); }
            if (stopAllPreviewClips == null) { Debug.LogError("AudioUtil.StopAllPreviewClips method not found. Unity version may have changed."); }

            playPreviewClip.Invoke(null, new object[] { originalClip, startSample, false });

            var durationSeconds = (float)(endSample - startSample) / originalClip.frequency;
            const float clippableMaxSeconds = 2f;
            if (durationSeconds <= clippableMaxSeconds)
            {
                Thread.Sleep(Mathf.RoundToInt(durationSeconds * 1000));
                stopAllPreviewClips.Invoke(null, null);
            }
        }

        private static int GetSubSample(int sample, AudioClip originalClip)
        {
            if (sample >= 0) return sample;
            else return originalClip.samples + sample;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property, true);
            return height;
        }
    }
}
