using UnityEditor;
using UnityEngine;

namespace SnakeGame.SoundsSystem
{
    [CustomEditor(typeof(SoundEffectSO))]
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
}
