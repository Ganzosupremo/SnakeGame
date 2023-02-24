using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SnakeGame.PlayerSystem
{
    [CustomEditor(typeof(SnakeDetailsSO))]
    public class AbilitiesEditor : Editor
    {
        Editor abilitiesEditor;
        bool abilitiesFoldout;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SnakeDetailsSO details = target as SnakeDetailsSO;

            if (details.specialAbility != null)
            {
                DrawAbilitiesEditor(details.specialAbility, ref abilitiesFoldout, ref abilitiesEditor);
                EditorPrefs.SetBool(nameof(abilitiesFoldout), abilitiesFoldout);
            }
        }

        private void DrawAbilitiesEditor(SpecialAbilitySO specialAbility, ref bool foldout, ref Editor editor)
        {
            if (specialAbility != null)
            {
                foldout = EditorGUILayout.InspectorTitlebar(foldout, specialAbility);
                if (foldout)
                {
                    CreateCachedEditor(specialAbility, null, ref editor);
                    editor.OnInspectorGUI();
                }
            }
        }

        private void OnEnable()
        {
            abilitiesFoldout = EditorPrefs.GetBool(nameof(abilitiesFoldout), false);
        }
    }
}
