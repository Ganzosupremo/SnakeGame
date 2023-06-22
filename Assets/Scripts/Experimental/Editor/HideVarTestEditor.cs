using UnityEditor;
using UnityEngine;

namespace SnakeGame
{
    [CustomEditor(typeof(HideVarTest))]
    [CanEditMultipleObjects]
    public class HideVarTestEditor : Editor
    {
        private SerializedProperty testProperty;
        private SerializedProperty radiusProperty;
        private SerializedProperty radiusThicknessProperty;
        private SerializedProperty spreadProperty;
        private SerializedProperty angleProperty;
        private SerializedProperty speedProperty;


        private SerializedObject _SerializedObject;


        private void OnEnable()
        {
            _SerializedObject = new SerializedObject(targets);

            testProperty = _SerializedObject.FindProperty("test");
            radiusProperty = _SerializedObject.FindProperty("Radius");
            radiusThicknessProperty = _SerializedObject.FindProperty("RadiusThickness");
            spreadProperty = _SerializedObject.FindProperty("Spread");
            angleProperty = _SerializedObject.FindProperty("Angle");
            speedProperty = _SerializedObject.FindProperty("Speed");
        }

        public override void OnInspectorGUI()
        {
            //serializedObject.Update();

            //HideVarTest.Test testValue = (HideVarTest.Test)EditorGUILayout.EnumPopup("Test", ((HideVarTest)target).test);

            //HideVarTest hideVarTest = (HideVarTest)target;

            //if (hideVarTest.test != testValue)
            //{
            //    hideVarTest.test = testValue;
            //    UpdateSerializedPropertiesVisibility();
            //}

            //EditorGUILayout.PropertyField(timeProperty, new GUIContent("Time"), IsVisible("One", "Two"));
            //EditorGUILayout.PropertyField(speedProperty, new GUIContent("Speed"), IsVisible("Three", "Four"));
            //EditorGUILayout.PropertyField(rangeProperty, new GUIContent("Range"), IsVisible("Five"));
            //EditorGUILayout.PropertyField(consProperty, new GUIContent("Cons"), IsVisible("None"));

            //serializedObject.ApplyModifiedProperties();

            _SerializedObject.Update();

            EditorGUILayout.PropertyField(testProperty);

            HideVarTest.Test testValue = (HideVarTest.Test)testProperty.enumValueIndex;

            SerializedProperty iterator = _SerializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (ShouldSerializeProperty(iterator, testValue))
                {
                    EditorGUILayout.PropertyField(iterator);
                }
            }

            //EditorGUILayout.PropertyField(testProperty);

            //HideVarTest.Test testValue = (HideVarTest.Test)testProperty.enumValueIndex;

            //SerializedProperty iterator = _SerializedObject.GetIterator();
            //bool enterChildren = true;

            //while (iterator.NextVisible(enterChildren))
            //{
            //    enterChildren = false;

            //    if (ShouldSerializeProperty(iterator, testValue))
            //    {
            //        EditorGUILayout.PropertyField(iterator);
            //    }
            //}

            //EditorGUILayout.PropertyField(radiusProperty);
            //EditorGUILayout.PropertyField(radiusThicknessProperty);
            //EditorGUILayout.PropertyField(spreadProperty);

            //EditorGUILayout.Space();

            //if (testValue == HideVarTest.Test.PingPong || testValue == HideVarTest.Test.Loop)
            //{
            //    EditorGUILayout.PropertyField(speedProperty);
            //}

            //if (testValue == HideVarTest.Test.BurstSpread || testValue == HideVarTest.Test.Whirpool)
            //{
            //    EditorGUILayout.PropertyField(angleProperty);
            //}

            _SerializedObject.ApplyModifiedProperties();
        }

        private bool ShouldSerializeProperty(SerializedProperty property, HideVarTest.Test testValue)
        {
            // Always serialize properties marked with the Tooltip attribute
            if (property.propertyPath == "ColorGradient" || property.propertyPath == "Type" || property.propertyPath == "ParticleDuration" ||
                property.propertyPath == "StartParticleSize" || property.propertyPath == "StartParticleSpeed" || property.propertyPath == "StartLifetime" ||
                property.propertyPath == "MaxParticles" || property.propertyPath == "EmissionRate" || property.propertyPath == "BurstNumber")
            {
                return true;
            }

            // Serialize properties based on the enum type selected
            if (testValue == HideVarTest.Test.Random || testValue == HideVarTest.Test.Loop)
            {
                if (property.propertyPath == "Angle")
                {
                    return true;
                }
            }

            if (testValue == HideVarTest.Test.BurstSpread || testValue == HideVarTest.Test.Whirpool)
            {
                if (property.propertyPath == "Speed")
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class ShowIfAttribute : PropertyAttribute
    {
        public string[] EnumValues { get; private set; }

        public ShowIfAttribute(params object[] enumValues)
        {
            EnumValues = new string[enumValues.Length];
            for (int i = 0; i < enumValues.Length; i++)
            {
                EnumValues[i] = enumValues[i].ToString();
            }
        }
    }
}
