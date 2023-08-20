using UnityEngine;

namespace SnakeGame.PlayerSystem
{
    public class SpecialAbilityBar : MonoBehaviour
    {
        #region Tooltip
        [Tooltip("Populate with the gameobject containing the special ability bar")]
        #endregion
        [SerializeField] private GameObject specialAbilityBarContainer;
        #region Tooltip
        [Tooltip("Populate with the gameobject containing the special ability cooldown bar")]
        #endregion
        [SerializeField] private GameObject specialAbilityCooldownBarContainer;

        public void SetSpecialAbilityBar(float duration)
        {
            specialAbilityBarContainer.transform.localScale = new Vector3(duration, 1f, 1f);
        }

        public void SetSpecialAbilityCooldownBar(float cooldown)
        {
            specialAbilityCooldownBarContainer.transform.localScale = new Vector3(cooldown, 1f, 1f);
        }

        public void ResetSpecialAbilityBar()
        {
            specialAbilityBarContainer.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public void ResetSpecialAbilityCooldownBar()
        {
            specialAbilityCooldownBarContainer.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
