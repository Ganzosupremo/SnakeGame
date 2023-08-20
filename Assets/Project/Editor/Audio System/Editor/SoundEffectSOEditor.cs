#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SnakeGame.AudioSystem
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SoundEffectSO))]
    [CanEditMultipleObjects]
    public class SoundEffectSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SoundEffectSO soundEffect = (SoundEffectSO)target;

            if (GUILayout.Button("Play Sound"))
            {
                soundEffect.Play();
            }
        }
    }
#endif
}
