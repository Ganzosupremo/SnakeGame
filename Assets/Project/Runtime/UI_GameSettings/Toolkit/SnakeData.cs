using SnakeGame.PlayerSystem.AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame.PlayerSystem
{
    [CreateAssetMenu]
    [Obsolete]
    public class SnakeData : ScriptableObject
    {
        public string SnakeName;
        public UniversalAbility SnakeAbility;
        public Sprite PortraitImage;
    }
}
