using UnityEditor;
using UnityEngine;

namespace SnakeGame.Dungeon.NoiseGenerator
{
    [CustomEditor(typeof(NoiseMap))]
    public class NoiseMapEditor : Editor
    {
        Editor settingsEditor;
        bool settingsFoldout;

        private void OnEnable()
        {
            settingsFoldout = EditorPrefs.GetBool(nameof(settingsFoldout), false);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            NoiseMap map = (NoiseMap)target;

            if (map.mapPreset != null)
            {
                DrawSettingsEditor(map.mapPreset, ref settingsFoldout, ref settingsEditor);
                EditorPrefs.SetBool(nameof(settingsFoldout), settingsFoldout);
            }
        }

        private void DrawSettingsEditor(Object mapPreset, ref bool settingsFoldout, ref Editor editor)
        {
            if (mapPreset != null)
            {
                settingsFoldout = EditorGUILayout.InspectorTitlebar(settingsFoldout, mapPreset);
                if (settingsFoldout)
                {
                    CreateCachedEditor(mapPreset, null, ref editor);
                    editor.OnInspectorGUI();
                }
            }
        }
    }
}
