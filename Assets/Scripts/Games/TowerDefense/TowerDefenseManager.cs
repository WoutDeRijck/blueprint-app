using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Misc;
using GameManagement;


namespace TowerDefense
{
    /// <summary>
    /// The towerdefense manager, starts the game clock and end boss also handles the return routine.
    /// </summary>
    public class TowerDefenseManager : Singleton<TowerDefenseManager>
    {
        [SerializeField]
        public List<GameObject> enemies;
        [SerializeField]
        public List<GameObject> enemiesUnderAttack;
        [SerializeField]
        public GameObject player;
        [SerializeField]
        public GameObject spawnLocation;
        [SerializeField]
        public GameObject endScreen;
        [SerializeField]
        public GameObject ClockText;
        [SerializeField]
        public GameObject networkObject;
        [SerializeField]
        public GameObject EnemySpawnObj;
        [SerializeField]
        public GameObject Introtext;
        [SerializeField]
        public GameObject Earlystopping;
        [SerializeField]
        public GameObject Finish;
        [SerializeField]
        public Transform transform_spawn;

        private Clock clock;
        private TowerDefenseNetwork network;
        private Earlystopping earlystopping;
        private FinishGame finish;

        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        void Start()
        {
            network = networkObject.GetComponent<TowerDefenseNetwork>();
            player = GameManager.Instance.player.gameObject;
            transform_spawn = player.GetComponent<Transform>();
            transform_spawn.position = spawnLocation.GetComponent<Transform>().position;
            clock = ClockText.GetComponent<Clock>();
            earlystopping = Earlystopping.GetComponent<Earlystopping>();
            finish = Finish.GetComponent<FinishGame>();
        }

        /// <summary>
        /// Called when the script instance is being enabled.
        /// </summary>
        private void OnEnable()
        {
            enemies = new List<GameObject>();
            enemiesUnderAttack = new List<GameObject>();
        }

        /// <summary>
        /// Starts the clock.
        /// </summary>
        public void startClock()
        {
            clock.activateClock();
            network.ClockClientRpc();
            network.IntrotextClientRpc();
            Introtext.SetActive(false);
            earlystopping.ShowButton();
            finish.ShowButton();
            GameObject.Find("Canvas Intro").SetActive(false);
        }

        /// <summary>
        /// Starts the end boss battle.
        /// </summary>
        public void startEndboss()
        {
            GameObject.Find("TowerDefenseNetwork").GetComponent<TowerDefenseNetwork>().StartEndBossClientRpc();
        }

        /// <summary>
        /// Ends the game to show victory or defeat depending on the win bool.
        /// This triggers a network call to all players
        /// </summary>
        /// <param name="win">Flag indicating whether the player won or lost.</param>

        public void EndGame(bool win)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                if (GameObject.FindWithTag("LastPrompt") != null)
                {
                    GameObject.FindWithTag("LastPrompt").SetActive(false);
                }
                if (win)
                {
                    Debug.Log("Endgame - win");
                    endScreen.GetComponent<EndGame>().ShowGameVictory();
                    network.EndScreenClientRpc(true);
                }
                else
                {
                    Debug.Log("Endgame - lose");
                    endScreen.GetComponent<EndGame>().ShowGameDefeat();
                    network.EndScreenClientRpc(false);
                }
            }
            Time.timeScale = 0;
        }

        /// <summary>
        /// Returns the players to the lobby and destroys gameobjects.
        /// </summary>
        public void Return()
        {
            //Todo in GameManager
            if (NetworkManager.Singleton.IsHost)
            {
                Destroy(GameObject.Find("NetworkManager"));
                Destroy(GameObject.Find("GameManager"));
                Destroy(GameObject.Find("Music"));
                Destroy(GameObject.Find("GameData"));
                Time.timeScale = 1;
                network.ReturnToLobbyClientRpc();
                SceneLoader.Load(SceneLoader.Scene.ClassroomDashboard);
                return;
            }
            Debug.Log("Return");
            Destroy(GameObject.Find("NetworkManager"));
            Destroy(GameObject.Find("GameManager"));
            Destroy(GameObject.Find("Music"));
            Destroy(GameObject.Find("GameData"));
            Time.timeScale = 1;
            SceneLoader.Load(SceneLoader.Scene.PersonalWorld);
        }
    }
}