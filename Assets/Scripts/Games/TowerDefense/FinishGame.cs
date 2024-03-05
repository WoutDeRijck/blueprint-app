using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// Sets the button for ending the game.
    /// </summary>
    public class FinishGame : MonoBehaviour
    {
        [SerializeField]
        private GameObject Intro;

        /// <summary>
        /// Disables the earlystopping button.
        /// </summary>
        void Start()
        {
            Intro.SetActive(false);
        }

        /// <summary>
        /// Shows the earlystopping button.
        /// </summary>
        public void ShowButton()
        {
            Intro.SetActive(true);
        }
    }
}
