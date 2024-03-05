using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Misc;


namespace GameData
{
    /// <summary>
    /// Constains all data to play a game
    /// </summary>
    [Serializable]
    public class Game
    {
        public SceneLoader.Scene firstScene;

        // Data of all ECSs available in the game
        public List<Minigame> ecsList = new List<Minigame>();

        /// <summary>
        /// Constructor: creates Game instance
        /// </summary>
        public Game(SceneLoader.Scene firstScene, List<Minigame> ecsList)
        {
            this.firstScene = firstScene;
            this.ecsList = ecsList;
        }
    }

}
