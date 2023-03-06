using UnityEditor;

namespace SnakeGame.PlayerSystem.AbilitySystem
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

            if (details.ability != null)
            {
                DrawAbilitiesEditor(details.ability, ref abilitiesFoldout, ref abilitiesEditor);
                EditorPrefs.SetBool(nameof(abilitiesFoldout), abilitiesFoldout);
            }
        }

        private void DrawAbilitiesEditor(UniversalAbility ability, ref bool foldout, ref Editor editor)
        {
            if (ability != null)
            {
                foldout = EditorGUILayout.InspectorTitlebar(foldout, ability);
                if (foldout)
                {
                    CreateCachedEditor(ability, null, ref editor);
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
