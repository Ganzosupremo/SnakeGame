using SnakeGame.VisualEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SnakeGame
{
    [CustomEditor(typeof(AmmoHitEffectSO))]
    public class AmmoHitEffectSOEditor : Editor
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
        }

        private bool ShouldSerializeProperty(SerializedProperty property, EffectType effectType)
        {
            if (property.propertyPath == "colorGradient" ||
                property.propertyPath == "Type" ||
                property.propertyPath == "particleDuration" ||
                property.propertyPath == "startParticleSize" ||
                property.propertyPath == "startParticleSpeed" ||
                property.propertyPath == "startLifetime" ||
                property.propertyPath == "maxParticles" ||
                property.propertyPath == "emissionRate" ||
                property.propertyPath == "burstNumber" ||
                property.propertyPath == "gravityEffect" ||
                property.propertyPath == "sprite" ||
                property.propertyPath == "minVelocityOverLifetime" ||
                property.propertyPath == "maxVelocityOverLifetime" ||
                property.propertyPath == "ammoHitEffectPrefab")
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
