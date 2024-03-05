using System.Collections.Generic;
using Unity.Netcode;
using System;
using UnityEngine;
using Misc;
using UnityEngine.SceneManagement;
using GameManagement;

namespace TowerDefense
{
    /// <summary>
    /// This class serves as the handler for all data that needs to be networked
    /// </summary>
    public class TowerDefenseNetwork : NetworkBehaviour
    {
        // Subscribers to these Actions are called when the event occurs
        public Action<int, int> OnHealthChanged;
        public Action<NetworkListEvent<int>> OnTowerLevelChanged;
        public Action<NetworkListEvent<double>> OnTowerProgressChanged;

        // Health of the castle
        public NetworkVariable<int> health;

        // Health of endboss
        public NetworkVariable<int> endbossHealth;
        // List of the levels of the towers, indexed on the towerID
        public NetworkList<int> towerLevel;

        // List of the progresses of the towers, indexed on the towerID
        public NetworkList<double> towerProgress;

        public GameManager gameManager;
        //public HUDUI hud;

        public List<double> playedMinigames;

        /// <summary>
        /// Initialize variables on enable.
        /// </summary>
        public void OnEnable()
        {
            health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            endbossHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            towerProgress = new NetworkList<double>(new List<double>() { 0, 0, 0, 0, 0 }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            towerLevel = new NetworkList<int>(new List<int>() { 0, 0, 0, 0, 0 }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            //hud = GameObject.Find("Canvas HUD").GetComponent<HUDUI>();
            playedMinigames = new List<double>(new double[50]);
        }

        /// <summary>
        /// Initialize variables on network spawn.
        /// </summary>
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            endbossHealth.OnValueChanged += (prevHealth, newHealth) => { OnHealthChanged?.Invoke(prevHealth, newHealth); };
            health.OnValueChanged += (prevHealth, newHealth) => { OnHealthChanged?.Invoke(prevHealth, newHealth); };
            towerLevel.OnListChanged += (eventType) => { OnTowerLevelChanged?.Invoke(eventType); };
            towerProgress.OnListChanged += (eventType) => { OnTowerProgressChanged?.Invoke(eventType); };

            gameManager.OnChangeStatusMinigame += ChangeECSListServerRPC;

        }


        [ServerRpc(RequireOwnership = false)]
        public void ChangeECSListServerRPC(int studentID, int ECS_ID, string minigame, string name, float ALL)
        {
            if (ECS_ID == 0) this.playedMinigames[studentID] += 1; //how many minigames has every student played
            //var hud = GameObject.Find("Canvas HUD").GetComponent<HUDUI>();
            //hud.ChangeStatus(ECS_ID, (int)this.playedMinigames[studentID], minigame, name, ALL);
            //hud.TEST();
            float progress = GameObject.Find("Clock text").GetComponent<Clock>().GetProgress();
            GameObject.Find("HUDUI").GetComponent<HUDUI>().ChangeStatus(ECS_ID, (int)this.playedMinigames[studentID], minigame, name, ALL);
            GameObject.Find("HUDUI").GetComponent<HUDUI>().ChangeProgress(progress);
        }


        /// <summary>
        /// Set the health of the castle (Client RPC)
        /// </summary>
        [ClientRpc]
        public void ChangeHealthClientRpc(int health)
        {
            GameObject.Find("Castle Health").GetComponent<CastleUIhealth>().SetHealth(this.health.Value, health);
        }

        /// <summary>
        /// Set the health of the end boss (Client RPC)
        /// </summary>
        [ClientRpc]
        public void ChangeEndbossHealthClientRpc(int endbossHealth)
        {
            GameObject.Find("Endboss Health").GetComponent<EndbossHealth>().SetEndbossHealth(endbossHealth);
            GameObject.Find("Endboss Health").GetComponent<EndbossUIhealth>().SetHealth(this.endbossHealth.Value, endbossHealth);
            
        }

        /// <summary>
        /// Set the health of the end boss (Server RPC)
        /// </summary>
        /// <param name="endbossHealth"></param>
        [ServerRpc(RequireOwnership = false)]
        public void DamageEndbossServerRpc(int endbossHealth)
        {
            GameObject.Find("Endboss Health").GetComponent<EndbossHealth>().SetEndbossHealth(endbossHealth);
            GameObject.Find("Endboss Health").GetComponent<EndbossUIhealth>().SetHealth(this.endbossHealth.Value, endbossHealth);
        }

        /// <summary>
        /// Set the progresses of the towers, indexed on the tower IDs (Server RPC)
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void ChangeProgressListServerRpc(int index, double value)
        {
            this.towerProgress[index] = value;
        }

        /// <summary>
        /// Set the level of the towers, indexed on the tower IDs (Server RPC)
        /// </summary>

        [ServerRpc(RequireOwnership = false)]
        public void ChangeLevelListServerRpc(int index, int value)
        {
            this.towerLevel[index] = value;
        }

        // <summary>
        /// Spawns an enemy at the specified spawn position. 
        /// </summary>
        /// <param name="spawnposition">The spawn position of the enemy.</param>
        [ClientRpc]
        public void SpawnEnemyClientRpc(Vector3 spawnposition)
        {
            GameObject enemy = Resources.Load<GameObject>("Games/EnemyTD");
            GameObject enemyObject = Instantiate(enemy, spawnposition, Quaternion.identity);
            TowerDefenseManager.Instance.enemies.Add(enemyObject);
        }

        /// <summary>
        /// Spawns an end boss at the specified spawn position. (Client RPC)
        /// </summary>
        /// <param name="spawnposition">The spawn position of the end boss.</param>
        [ClientRpc]
        public void SpawnEndBossClientRpc(Vector3 spawnposition)
        {
            GameObject enemy = Resources.Load<GameObject>("Games/Endboss");
            GameObject enemyObject = Instantiate(enemy, spawnposition, Quaternion.identity);
            TowerDefenseManager.Instance.enemies.Add(enemyObject);
        }

        /// <summary>
        /// Starts the end boss battle. (Client RPC)
        /// </summary>
        [ClientRpc]
        public void StartEndBossClientRpc()
        {
            GameObject.Find("EnemySpawner").GetComponent<EnemySpawner2>().EndBoss();
        }

        /// <summary>
        /// Hides the intro text. (Client RPC)
        /// </summary>
        [ClientRpc]
        public void IntrotextClientRpc()
        {
            GameObject.Find("Canvas Intro text").SetActive(false);
        }

        /// <summary>
        /// Activates the clock. (Client RPC)
        /// </summary>
        [ClientRpc]
        public void ClockClientRpc()
        {
            GameObject.Find("Clock text").GetComponent<Clock>().activateClock();
        }

        /// <summary>
        /// Displays the end screen based on the specified flag. (Client RPC)
        /// </summary>
        /// <param name="b">Flag indicating whether the player won or lost.</param>
        [ClientRpc]
        public void EndScreenClientRpc(bool b)
        {
            GameObject.Find("EndscreenVictory").GetComponent<EndGame>().EndScreen(b);
        }

        /// <summary>
        /// Returns to the lobby. (Client RPC)
        /// </summary>
        [ClientRpc]
        public void ReturnToLobbyClientRpc()
        {
            if(!IsHost)
            {
                Destroy(GameObject.Find("NetworkManager"));
                Destroy(GameObject.Find("GameManager"));
                Destroy(GameObject.Find("Music"));
                Destroy(GameObject.Find("GameData"));
                Time.timeScale = 1;
                SceneLoader.Load(SceneLoader.Scene.PersonalWorld);
            }
        }
    }
}