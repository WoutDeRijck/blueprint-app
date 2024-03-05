//using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Netcode;

namespace TowerDefense
{
    /// <summary>
    /// Controls the enemies waves / end boss and spawn locations. 
    /// </summary>
    public class EnemySpawner2 : MonoBehaviour
    {
        [SerializeField]
        private GameObject enemy;
        [SerializeField]
        private Vector3 spawnposition;
        [SerializeField]
        private float spawnrate = 0.3f;
        [SerializeField]
        private float spawnrate_increase;
        [SerializeField]
        private float rangeX;
        [SerializeField]
        private float rangeY;
        [SerializeField]
        private int num_waves = 2;
        [SerializeField]
        private bool wave_active;
        [SerializeField]
        private int wavenumber;
        [SerializeField]
        private int enemy_count;
        [SerializeField]
        private int spawn_size;
        [SerializeField]
        private int spawn_increase;
        [SerializeField]
        private GameObject Healthbar;
        [SerializeField]
        private GameObject Helper;
        [SerializeField]
        private GameObject clockObject;
        [SerializeField]
        private GameObject EndbossBar;
        [SerializeField]
        private GameObject ButtonEarlyStopping;
        [SerializeField]
        private Clock clock;
        [SerializeField]
        private int num_players;
        [SerializeField]
        private int spawn_increase_player;
        [SerializeField]
        private int active_enemies;
        [SerializeField]
        private bool EndbossActive;
        private float spawnside;
        private TowerDefenseNetwork network;
        private float posX;
        private float posY;
        private float nextspawn = 0.0f;

        [SerializeField]
        private GameObject EndbossPrompt;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        void Start()
        {
            GameObject networkObject = GameObject.Find("TowerDefenseNetwork");
            network = networkObject.GetComponent<TowerDefenseNetwork>();
            Healthbar.SetActive(true);
            clockObject = GameObject.Find("Clock text");
            clock = clockObject.GetComponent<Clock>();
            wave_active = false;
            wavenumber = 0;
            num_players = GameObject.FindGameObjectsWithTag("Player").Length;
            EndbossBar.SetActive(false);
            EndbossActive = false;
        }

        /// <summary>
        /// Starts a new wave.
        /// </summary>
        public void startWave()
        {
            //Healthbar.SetActive(true);
            //Helper.SetActive(false);
            num_players = GameObject.FindGameObjectsWithTag("Player").Length;
            Debug.Log("Wave " + wavenumber + " started");
            if (wavenumber != num_waves - 1)
            {
                spawn_size = spawn_size + spawn_increase * wavenumber + spawn_increase_player * wavenumber * num_players;
                spawnrate = spawnrate - spawnrate_increase;
            }
            else
            {
                spawn_size = 0;
                GameObject.Find("TowerDefenseManager").GetComponent<TowerDefenseManager>().startEndboss();
            }
            enemy_count = 0;
            wave_active = true;
            wavenumber++;
        }

        /// <summary>
        /// Starts the end boss wave.
        /// </summary>
        public void EndBoss()
        {
            EndbossActive = true;
            wavenumber = num_waves;
            spawn_size = 0;
            GameObject.Find("ClockUI").SetActive(false);
            EndbossBar.SetActive(true);
            if (NetworkManager.Singleton.IsHost)
            {
                ButtonEarlyStopping.SetActive(false);
            }
            wave_active = true;

            //deactivate all towers:
            GameObject[] promptObjects = GameObject.FindGameObjectsWithTag("Prompt");
            foreach (GameObject promptObject in promptObjects)
            {
                promptObject.SetActive(false);
            }
            
        }

        /// <summary>
        /// Update every frame, if wave active spawn enemies or endboss.
        /// If the number of enemies is zero, the wave is done and if the wavenumber equals the max, the game ends.
        /// </summary>
        void Update()
        {
            active_enemies = TowerDefenseManager.Instance.enemies.Count;
            if (wave_active && NetworkManager.Singleton.IsHost)
            {
                if (EndbossActive && GameObject.FindGameObjectsWithTag("Endboss").Length == 0)//&& TowerDefenseManager.Instance.enemies.Count == 0)
                {
                    Debug.Log("Spawn Endboss");
                    spawnposition = new Vector3(0, 0) + transform.position;
                    GameObject.Find("TowerDefenseNetwork").GetComponent<TowerDefenseNetwork>().SpawnEndBossClientRpc(spawnposition);
                }
                else
                {
                    if (Time.time > nextspawn)
                    {
                        nextspawn = Time.time + spawnrate;
                        spawnside = Random.Range(0, 1f);
                        //spawn randomly left or spawn randomly right 
                        posX = 0.0f;
                        posY = 0.0f;

                        if (spawnside > 0.5)
                        {
                            posX = Random.Range(0, rangeX);
                        }
                        else
                        {
                            posY = Random.Range(0, rangeY);
                        }
                        spawnposition = new Vector3(posX, posY) + transform.position;
                        if (enemy_count < spawn_size)
                        {
                            GameObject.Find("TowerDefenseNetwork").GetComponent<TowerDefenseNetwork>().SpawnEnemyClientRpc(spawnposition);
                            enemy_count++;

                        }
                    }
                }

                if (TowerDefenseManager.Instance.enemies.Count == 0)
                {
                    wave_active = false;
                    if (wavenumber == num_waves)
                    {
                        TowerDefenseManager.Instance.EndGame(true);
                    }
                    else
                    {
                        clock.activateClock();
                        network.ClockClientRpc();
                    }

                }
            }
            if (EndbossActive)
            {
                EndbossPrompt.GetComponent<Transform>().position = GameObject.FindGameObjectWithTag("Endboss").GetComponent<Transform>().position;
            }

        }
    }
}