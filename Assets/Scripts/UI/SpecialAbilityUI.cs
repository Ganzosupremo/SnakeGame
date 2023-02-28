using SnakeGame.PlayerSystem;
using System.Collections;

namespace SnakeGame.UI
{
    public class SpecialAbilityUI : SingletonMonoBehaviour<SpecialAbilityUI>
    {
        SpecialAbilityBar abilityBar;

        protected override void Awake()
        {
            base.Awake();

            abilityBar = GetComponentInChildren<SpecialAbilityBar>();
        }

        public IEnumerator UpdateSpecialAbilityBar(float duration)
        {
            abilityBar.SetSpecialAbilityBar(duration);
            yield return null;
        }

        public IEnumerator UpdateSpecialAbilityCooldownBar(float cooldown)
        {
            abilityBar.SetSpecialAbilityCooldownBar(cooldown);
            yield return null;
        }

        public void ResetSpecialAbilityBar()
        {
            abilityBar.ResetSpecialAbilityBar();
        }

        public void ResetSpecialAbilityCooldownBar()
        {
            abilityBar.ResetSpecialAbilityCooldownBar();
        }
    }
}
