using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SnakeGame.VisualEffects
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DeathEffectSO))]
    public class DeathEffectSOEditor : Editor
    {
        private SerializedProperty _TypeProperty;
        private SerializedObject _SerializedObject;

        private void OnEnable()
        {
            _SerializedObject = new SerializedObject(targets);

            _TypeProperty = _SerializedObject.FindProperty("Type");
        }

        public override void OnInspectorGUI()
        {
            _SerializedObject.Update();

            EffectType effectType = (EffectType)_TypeProperty.enumValueIndex;

            SerializedProperty iterator = _SerializedObject.GetIterator();
            bool enterChildren = true;
            
            EditorGUILayout.Space();

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (ShouldSerializeProperty(iterator, effectType))
                {
                    EditorGUILayout.PropertyField(iterator);
                }
            }

            _SerializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            DeathEffectSO deathEffectSO = (DeathEffectSO)target;

            if (GUILayout.Button("Preview Effect"))
            {
                deathEffectSO.PreviewParticleSystem();
            }
        }

        private bool ShouldSerializeProperty(SerializedProperty property, EffectType effectType)
        {
            if (property.propertyPath == "ColorGradient" || 
                property.propertyPath == "Type" || 
                property.propertyPath == "ParticleDuration" ||
                property.propertyPath == "StartParticleSize" || 
                property.propertyPath == "StartParticleSpeed" || 
                property.propertyPath == "StartLifetime" ||
                property.propertyPath == "MaxParticles" || 
                property.propertyPath == "EmissionRate" || 
                property.propertyPath == "BurstNumber" ||
                property.propertyPath == "MinGravityEffect" ||
                property.propertyPath == "MaxGravityEffect" ||
                property.propertyPath == "SpriteArray" ||
                property.propertyPath == "MinVelocityOverLifetime" ||
                property.propertyPath == "MaxVelocityOverLifetime" ||
                property.propertyPath == "DeathEffectPrefab")
                return true;

            if (effectType == EffectType.None)
                return false;

            // Just serialize the angle is the effect is Cone_Upwards
            if (property.name == "Angle" && effectType == EffectType.Cone_Upwards)
                return true;

            if ((effectType == EffectType.Cone_Upwards ||
                effectType == EffectType.Circle_Explosion ||
                effectType == EffectType.Circle_Whirpool ||
                effectType == EffectType.Sphere_Burst) && property.propertyPath != "Angle")
                return true;

            return false;
        }
    }
}
