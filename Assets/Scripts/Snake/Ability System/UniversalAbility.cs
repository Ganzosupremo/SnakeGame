using UnityEngine;

namespace SnakeGame.PlayerSystem.AbilitySystem
{
    public class UniversalAbility : ScriptableObject
    {
        public new string name = "";
        [Range(1f, 10f)]
        public float activeTime = 1f;
        [Range(1f, 10f)]
        public float cooldownTime = 1f;

        public virtual void Activate(GameObject parent) { }
        public virtual void BeginCooldown(GameObject parent) { }
    }
}
