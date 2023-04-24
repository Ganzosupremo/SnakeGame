using UnityEngine;
using SnakeGame;
using SnakeGame.AudioSystem;
using SnakeGame.GameUtilities;
using Cysharp.Threading.Tasks;

namespace SnakeGame.ProceduralGenerationSystem
{
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class Door : MonoBehaviour
    {
        #region Header Object References
        [Space(10)]
        [Header("Object References")]
        #endregion
        #region Tooltip
        [Tooltip("Populate With The BoxCollider2D In The DoorCollider GameObject")]
        #endregion
        [SerializeField] private BoxCollider2D doorCollider;

        [HideInInspector] public bool isBossRoomDoor = false;
        [SerializeField] private BoxCollider2D doorTrigger;
        [SerializeField] private ParticleSystem doorEffect;

        private bool isOpen = false;
        private bool previouslyOpened = false;
        private Animator doorAnimator;

        private void Awake()
        {
            doorCollider.enabled = true;

            doorAnimator = GetComponent<Animator>();
            doorTrigger = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Settings.PlayerTag) || other.CompareTag(Settings.playerWeapon))
            {
                //playRandomSound = Random.Range(0, GameResources.Instance.openCloseDoorSoundEffect.Length);
                OpenDoor();
            }
        }

        private void OnEnable()
        {
            //When the player gets far enough from a room, the room will be disabled and all the things in it too, this means
            //the animator state for the door gets reset, therefore we need to restore the animator state
            doorAnimator.SetBool(Settings.openDoor, isOpen);
        }

        /// <summary>
        /// Opens The Door When The Player Triggers The Door Collider
        /// </summary>
        public void OpenDoor()
        {
            if (!isOpen)
            {
                isOpen = true;
                previouslyOpened = true;
                doorCollider.enabled = false;
                doorTrigger.enabled = false;

                //Set the 'open' parameter in the animator
                doorAnimator.SetBool(Settings.openDoor, true);

                if (doorEffect != null)
                {
                    doorEffect.Play();
                }

                // In the future every door will have it's unique open sound effect
                SoundEffectManager.CallOnSoundEffectSelectedEvent(GameResources.Instance.doorSoundEffect);
            }
        }

        public void LockDoors()
        {
            //Set the open parameter to false to lock the door
            doorAnimator.SetBool(Settings.openDoor, false);

            isOpen = false;
            doorCollider.enabled = true;
            doorTrigger.enabled = false;
        }

        /// <summary>
        /// Locks The Doors When The Player Enters The Room
        /// </summary>
        public async UniTask LockDoorsAsync()
        {
            await UniTask.Delay(500);
            //Set the open parameter to false to lock the door
            doorAnimator.SetBool(Settings.openDoor, false);

            isOpen = false;
            doorCollider.enabled = true;
            doorTrigger.enabled = false;
            await UniTask.Yield();
        }

        /// <summary>
        /// Unlocks The Door
        /// </summary>
        public void UnlockDoor()
        {
            doorCollider.enabled = false;
            doorTrigger.enabled = true;

            if (previouslyOpened)
            {
                isOpen = false;
                OpenDoor();
            }
        }

        #region Validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
        }
#endif
        #endregion
    }
}
