using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// Manages the damage of the end boss.
    /// </summary>
    public class EndbossHealth : MonoBehaviour
    {
        [SerializeField]
        private int maxhealth;
        [SerializeField]
        private GameObject Endboss;
        [SerializeField]
        private int damage;
        [SerializeField]
        private int health;

        private TowerDefenseNetwork network;

        /// <summary>
        /// Called when the script instance is being loaded. Initializes all objects.
        /// </summary>
        void Start()
        {
            int num_players_count = GameObject.FindGameObjectsWithTag("Player").Length - 1;
            maxhealth = maxhealth * (1+ num_players_count)/2;
            GameObject networkObject = GameObject.Find("TowerDefenseNetwork");
            network = networkObject.GetComponent<TowerDefenseNetwork>();

            network.ChangeEndbossHealthClientRpc(maxhealth);
            health = maxhealth;
            EndbossUIhealth EndbossUI = Endboss.GetComponent<EndbossUIhealth>();
            EndbossUI.SetMaxHealth(maxhealth);

            // Listen to changes on health
            network.OnHealthChanged += EndbossUI.SetHealth;
        }

        /// <summary>
        /// Deals damage to the end boss.
        /// </summary>
        public void takeDamage()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                health -= damage;
                network.ChangeEndbossHealthClientRpc(health);
                Debug.Log("health endboss : " + health);
                if (health < 0)
                {
                    Debug.Log("boss is death");
                    TowerDefenseManager.Instance.EndGame(true);
                }
            }
        }

        /// <summary>
        /// Return the health of the endboss.
        /// </summary>
        public int GetEndbossHealth()
        {
            return health;
        }

        /// <summary>
        /// Set the endboss health.
        /// </summary>
        /// <param name="SetHealth">Health</param>
        public void SetEndbossHealth(int SetHealth)
        {
            health = SetHealth;
        }

    }
}