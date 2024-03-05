using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// Sets the button for early stopping.
    /// </summary>
    public class Earlystopping : MonoBehaviour
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