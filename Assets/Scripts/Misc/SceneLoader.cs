using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Misc
{

    public static class SceneLoader
    {
        public enum Scene
        {
            LoginScene,
            ClassroomDashboard,
            PersonalWorld,
            Lobby,
            SnakeMPC,
            SnakeMPC2,
            TowerDefenseNew,
            GroupBomb,
            RankRumble
        }

        public static void Load(Scene scene, bool additive = false)
        {
            if (additive)
            {
                Debug.Log("load additive scene");
                SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
                return;
            }
            SceneManager.LoadScene(scene.ToString());
        }

        /// <summary>
        /// Get all scenes concerning Multiple Choice questions
        /// </summary>
        public static List<Scene> GetMultipleChoiceScenes()
        {
            return new List<Scene>() { Scene.SnakeMPC2 };
        }

        /// <summary>
        /// Get all scenes concerning Grouping questions
        /// </summary>
        public static List<Scene> GetGroupingScenes()
        {
            return new List<Scene>() { Scene.GroupBomb };
        }

        /// <summary>
        /// Get all scenes concerning Ranking questions
        /// </summary>
        public static List<Scene> GetRankingScenes()
        {
            return new List<Scene>() { Scene.RankRumble };
        }

        public static void LoadSceneOnTop(string scene)
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }

        public static void Unload(string lastsSceneName)
        {
            SceneManager.UnloadSceneAsync(lastsSceneName);
        }
    }
}
