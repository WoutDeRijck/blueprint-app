using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TowerDefense
{
    /// <summary>
    /// Shows the text with the game explanation.
    /// </summary>
    public class Introduction : MonoBehaviour
    {
        [SerializeField]
        private GameObject Intro;

        /// <summary>
        /// Disables the intro game object if the current instance is not the host.
        /// </summary>
        void Start()
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                Intro.SetActive(false);
            }
        }
    }
}