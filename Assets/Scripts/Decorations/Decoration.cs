using UnityEngine;

namespace SnakeGame.Decorations
{
    public class Decoration : MonoBehaviour
    {
        #region Header References
        [Header("REFERENCES")]
        #endregion

        [Tooltip("Populate with the sprite renderer of the prefab")]
        public SpriteRenderer spriteRenderer;

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(spriteRenderer), spriteRenderer);
        }
#endif
        #endregion
    }
}
