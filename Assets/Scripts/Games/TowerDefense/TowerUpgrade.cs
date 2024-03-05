using GameManagement;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace TowerDefense
{
    /// <summary>
    /// The logic for upgrading towers.
    /// </summary>
    public class TowerUpgrade : MonoBehaviour
    {
        [SerializeField]
        private Sprite level0;
        [SerializeField]
        private Sprite level1;
        [SerializeField]
        private Sprite level2;
        [SerializeField]
        private Sprite level3;
        [SerializeField]
        private Sprite level4;
        [SerializeField]
        private Sprite level5;
        private List<Sprite> sprites;
        private int current;
        private int max;
        private SpriteRenderer spriteRenderer;
        private TowerDefenseNetwork network;

        /// <summary>
        /// Initializes the gameobjects and variables.
        /// </summary>
        void Start()
        {
            GameManager.Instance.AL.OnScoreUpdated += AddProgress;
            //GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); unnecessary OVERHEAD
            //gameManager.OnScoreUpdated += AddProgress;tow

            GameObject networkObject = GameObject.Find("TowerDefenseNetwork");
            network = networkObject.GetComponent<TowerDefenseNetwork>();
            network.OnTowerProgressChanged += Upgrade;
            network.OnTowerProgressChanged += ChangeProgressBar;
            max = 5;
            sprites = new List<Sprite>
        {
            level0,
            level1,
            level2,
            level3,
            level4,
            level5
        };

        }

        /// <summary>
        /// Adds progress to the tower based on the score.
        /// </summary>
        /// <param name="hotspotplace">The hotspot place.</param>
        /// <param name="score">The score.</param>
        public void AddProgress(int hotspotplace, float score)
        {
            if (GameObject.FindWithTag("Endboss")  != null)
            {
                int damage = (int)(score * 200);
                Debug.Log("Endboss damage : " + damage);
                int health = GameObject.Find("Endboss Health").GetComponent<EndbossHealth>().GetEndbossHealth();
                Debug.Log("Endboss lives : " + health);
                network.DamageEndbossServerRpc(health - damage);
                GameObject.Find("Endboss Health").GetComponent<EndbossUIhealth>().SetHealth(health, health-damage);
            }
            else
            {
                Debug.Log("ADDED PROGRESS : " + ((score) / (1 + network.towerLevel[hotspotplace] / 3.5)));
                Debug.Log("TOTAL PROGRESS : " + (network.towerProgress[hotspotplace] + ((score) / (1 + network.towerLevel[hotspotplace] / 3.5))));
                network.ChangeProgressListServerRpc(hotspotplace, network.towerProgress[hotspotplace] + ((score) / (1 + network.towerLevel[hotspotplace] / 3.5)));
            }
            
        }

        /// <summary>
        /// Upgrades the tower based on the tower progress.
        /// </summary>
        /// <param name="eventList">The network event list.</param>
        public void Upgrade(NetworkListEvent<double> eventList)
        {
            Debug.Log("Eventlist " + eventList.Value);
            if (network.towerProgress[eventList.Index] >= 2 && network.towerLevel[eventList.Index] < max)
            {
                Debug.Log("UPGRADE TOWER progress reset : " + (network.towerProgress[eventList.Index] - 2));
                spriteRenderer = GameObject.Find("Tower" + eventList.Index).GetComponent<SpriteRenderer>();
                network.ChangeProgressListServerRpc(eventList.Index, (network.towerProgress[eventList.Index] - 2));
                network.ChangeLevelListServerRpc(eventList.Index, network.towerLevel[eventList.Index] + 1);
                current = network.towerLevel[eventList.Index];
                spriteRenderer.sprite = sprites[network.towerLevel[eventList.Index]];

            }
        }

        /// <summary>
        /// Changes the progress bar of the tower UI.
        /// </summary>
        /// <param name="eventList">The network event list.</param>
        public void ChangeProgressBar(NetworkListEvent<double> eventList)
        {
            GameObject.Find("Tower" + eventList.Index).GetComponentInChildren<TowerUIprogress>().SetHealth(network.towerProgress[eventList.Index]);
        }

        /// <summary>
        /// Gets the current upgrade level of the tower.
        /// </summary>
        public int GetUpgradeLevel()
        {
            return current;
        }
    }
}