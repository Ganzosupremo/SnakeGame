using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Food_", menuName = "Scriptable Objects/Food/Snake Food")]
public class FoodSO : ScriptableObject
{
    #region Tooltip
    [Tooltip("The name for this type of food")]
    #endregion
    public string foodName;

    #region Tooltip
    [Tooltip("The prefab for this food, this must be populated and the prefab must containt" +
        " the food script")]
    #endregion
    public GameObject prefab;
    
    #region Tooltip
    [Tooltip("The amount of damage that the current weapon will increase, when the " +
        "player eats this food.")]
    #endregion
    public int increaseDamageBy;
    
    #region Tooltip
    [Tooltip("The amount of health that will increase when the player eats" +
        " this food.")]
    #endregion
    public int increaseHealthBy;
    
    #region Tooltip
    [Tooltip("The minimum food of this type that will spawn for this level.")]
    #endregion
    public int minFoodToSpawn;

    #region Tooltip
    [Tooltip("The maximum food of this type that will spawn for this level.")]
    #endregion
    public int maxFoodToSpawn;

    #region Tooltip
    [Tooltip("the min number of this type of food that can exist on the current room.")]
    #endregion
    public int minNumberOfFoodToExist;

    #region Tooltip
    [Tooltip("the max number of this type of food that can exist on the current room.")]
    #endregion
    public int maxNumberOfFoodToExist;

    // This are used to delay the spawn of every food
    [HideInInspector] public float minSpawnInterval = 5f;
    [HideInInspector] public float maxSpawnInterval = 6f;

    #region Materialize Effect
    [Header("Materialize Effect")]
    [Space(10)]
    #endregion

    public float materializeTime;

    public Material defaultLitMaterial;

    public Shader materializeShader;

    [ColorUsage(true, true)]
    public Color materiliazeColor;
}
