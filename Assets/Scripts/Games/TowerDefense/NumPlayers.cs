using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// Counts the number of players in the game.
    /// </summary>
    public class NumPlayers : MonoBehaviour
    {
        [SerializeField]
        private Text num_players;
        [SerializeField]
        private int num_players_count;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        void Start()
        {
            num_players_count = 0;
            num_players.text = num_players_count.ToString();
        }

        /// <summary>
        /// Counts the number of players active in the game. 
        /// </summary>
        void Update()
        {
            num_players_count = GameObject.FindGameObjectsWithTag("Player").Length - 1;
            if (num_players_count < 0)
            {
                num_players_count = 0;
            }
            num_players.text = num_players_count.ToString();
        }
    }
}