using UnityEditor;
using UnityEngine;
using static SnakeGame.PoolManager;

namespace SnakeGame
{
    [CustomPropertyDrawer(typeof(PoolObject))]
    public class PoolDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        //{
        //    //// Don't make child fields be indented
        //    //var indent = EditorGUI.indentLevel;
        //    //EditorGUI.indentLevel = 0;

        //    //// Calculate rects
        //    //var poolSizeRect = new Rect(position.x - 50, position.y, 30, position.height);
        //    //var componentTypeRect = new Rect(position.x - 15, position.y, 180, position.height);
        //    //var prefabToUseRect = new Rect(componentTypeRect.x + componentTypeRect.width + 5, position.y, 
        //    //    position.width - componentTypeRect.width, position.height);

        //    //// Draw fields - pass GUIContent.none to each so they are drawn without labels
        //    //EditorGUI.PropertyField(poolSizeRect, property.FindPropertyRelative("PoolSize"), GUIContent.none);
        //    //EditorGUI.PropertyField(componentTypeRect, property.FindPropertyRelative("componentType"), GUIContent.none);
        //    //EditorGUI.PropertyField(prefabToUseRect, property.FindPropertyRelative("prefabToUse"), GUIContent.none);

        //    //// Set indent back to what it was
        //    //EditorGUI.indentLevel = indent;

        //    //EditorGUI.EndProperty();
        //}
    }
}
