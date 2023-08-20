using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using System.Threading.Tasks;
using SnakeGame.Enemies;
using SnakeGame.PlayerSystem;

namespace SnakeGame
{
    [DisallowMultipleComponent]
    public class RemoteConfigManager : SingletonMonoBehaviour<RemoteConfigManager>
    {
        public bool ShouldFetchRemoteConfig = false;

        EnemyDetailsSO[] enemies;
        SnakeDetailsSO snake;

        protected override void Awake()
        {
            base.Awake();

            enemies = GameResources.Instance.enemyDetailsList.ToArray();
            snake = GameResources.Instance.currentSnake.snakeDetails;

            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        async void Start()
        {
            if (ShouldFetchRemoteConfig)
            {
                await StartRemoteConfig();
            }
        }

        private void OnDisable()
        {
            RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteSettings;

            foreach (EnemyDetailsSO enemy in enemies)
            {
                float minFireDelay = RemoteConfigService.Instance.appConfig.GetFloat("EnemyFireDelayMin");
                float maxFireDelay = RemoteConfigService.Instance.appConfig.GetFloat("EnemyFireDelayMax");

                enemy.firingMinDelay -= minFireDelay;
                enemy.firingMaxDelay -= maxFireDelay;
            }
        }

        private void OnDestroy()
        {
            RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteSettings;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        async Task InitializeRemoteConfigAsync()
        {
            // initialize handlers for unity game services
            await UnityServices.InitializeAsync();

            // remote config requires authentication for managing environment information
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        async Task StartRemoteConfig()
        {
            // initialize Unity's authentication and core services, however check for internet connection
            // in order to fail gracefully without throwing exception if connection does not exist
            if (Utilities.CheckForInternetConnection())
            {
                await InitializeRemoteConfigAsync();
            }

            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
            RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
        }

        void ApplyRemoteSettings(Unity.Services.RemoteConfig.ConfigResponse configResponse)
        {
            foreach (EnemyDetailsSO enemy in enemies)
            {
                float minFireDelay = RemoteConfigService.Instance.appConfig.GetFloat("EnemyFireDelayMin");
                float maxFireDelay = RemoteConfigService.Instance.appConfig.GetFloat("EnemyFireDelayMax");

                enemy.firingMinDelay += minFireDelay;
                enemy.firingMaxDelay += maxFireDelay;
            }
        }

        public struct UserAttributes { }
        public struct AppAttributes { }
    }
}
