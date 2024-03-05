using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// Controls the castle health.
    /// </summary>
    public class CastleHealth : MonoBehaviour
    {
        [SerializeField]
        private int maxhealth;
        [SerializeField]
        public GameObject castle;
        [SerializeField]
        private int damage;
        [SerializeField]
        private int endbossdamage;
        [SerializeField]
        private int health;

        private TowerDefenseNetwork network;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        void Start()
        {
            GameObject networkObject = GameObject.Find("TowerDefenseNetwork");
            network = networkObject.GetComponent<TowerDefenseNetwork>();
            network.ChangeHealthClientRpc(maxhealth);
            health = maxhealth;
            CastleUIhealth castleUI = castle.GetComponent<CastleUIhealth>();
            castleUI.SetMaxHealth(maxhealth);

            // Listen to changes on health
            network.OnHealthChanged += castleUI.SetHealth;
        }

        /// <summary>
        /// Called once per frame for every Collider2D other that is touching the trigger (2D physics only). 
        /// Does damage to the castle and synchronize the updated health with clients. Checks if the castle's health is below zero than end the game
        /// </summary>
        /// <param name="collider">The Collider2D data associated with this collision.</param>

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if ((collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "Endboss") && NetworkManager.Singleton.IsHost)
            {
                if (collider.gameObject.tag == "Endboss")
                {
                    health -= endbossdamage;
                }
                else
                {
                    health -= damage;
                }
                network.ChangeHealthClientRpc(health);
                Debug.Log(health);
                if (health < 0)
                {
                    Debug.Log("GAME OVER");
                    TowerDefenseManager.Instance.EndGame(false);
                }
            }
        }
        /// <summary>
        /// Called once per frame for every Collider2D other that is touching the trigger (2D physics only). 
        /// Does damage to the castle and synchronize the updated health with clients. Checks if the castle's health is below zero than end the game
        /// </summary>
        /// <param name="collider">The Collider2D data associated with this collision.</param>
        private void OnTriggerStay2D(Collider2D collider)
        {
            if ((collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "Endboss") && NetworkManager.Singleton.IsHost)
            {
                health -= damage;
                network.ChangeHealthClientRpc(health);
                Debug.Log(health);
                if (health < 0)
                {
                    Debug.Log("GAME OVER");
                    TowerDefenseManager.Instance.EndGame(false);
                }
            }
        }
    }
}