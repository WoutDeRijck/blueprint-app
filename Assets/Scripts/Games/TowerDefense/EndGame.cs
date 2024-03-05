using Misc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense
{
    /// <summary>
    /// The logic for ending the game.
    /// </summary>
    public class EndGame : MonoBehaviour
    {

        [SerializeField]
        private GameObject victory;
        [SerializeField]
        private GameObject defeat;
        [SerializeField]
        private GameObject ReturnButton;
        private bool MiniGameActive;
        private bool TimeToEnd;
        private bool Victory;

        /// <summary>
        /// Called when the script instance is being loaded.Set all GameObjects to false.
        /// </summary>
        void Start()
        {
            MiniGameActive = false;
            TimeToEnd = false;
            victory.SetActive(false);
            defeat.SetActive(false);
            ReturnButton.SetActive(false);
        }

        /// <summary>
        /// Shows the game victory UI and enables the return button.
        /// </summary>
        public void ShowGameVictory()
        {
            victory.SetActive(true);
            defeat.SetActive(false);
            ReturnButton.SetActive(true);
            Time.timeScale = 0;

        }

        /// <summary>
        /// Shows the game defeat UI and enables the return button.
        /// </summary>
        public void ShowGameDefeat()
        {
            defeat.SetActive(true);
            victory.SetActive(false);
            ReturnButton.SetActive(true);
            Time.timeScale = 0;
        }

        /// <summary>
        /// Shows the vicotry/defeat endscreen depending on the bool.
        /// </summary>
        /// <param name="win">Boolean for win or lose</param>
        public void EndScreen(bool win)
        {
            TimeToEnd = true;
            Victory = win;
        }

        /// <summary>
        /// Checks if there is still a minigame active. Calls endscreen function when minigame is ended and TimeToEnd is true.
        /// </summary>
        public void Update()
        {
            if (SceneManager.sceneCount > 1)
            {
                MiniGameActive = true;
            } else
            {
                MiniGameActive = false;
            }

            if(TimeToEnd && !MiniGameActive) {
                if (Victory)
                {
                    ShowGameVictory();
                }
                else
                {
                    ShowGameDefeat();
                }

            }
        }
    }
}