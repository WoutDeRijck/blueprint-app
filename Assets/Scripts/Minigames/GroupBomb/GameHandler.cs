using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using GameData;
using Misc;
using GameManagement;
using System.Transactions;

namespace GroupBomb
{

    /// <summary>
    /// Brings all objects together and controls them
    /// </summary>
    public class GameHandler : MonoBehaviour
    {
        //Store the levelGrid that is being used
        private LevelGrid levelGrid;
        //Store the Bomb Prefab for correct instantiation
        public GameObject bombprefab;

        //bool to check if it the first bomb, seems unneeded but avoids bugs
        private bool first = true;

        //Track Time since last bomb spawning took place
        private float SpawnTimer;
        //Track the speed at which the bombs spawn
        public float SpawnTimerMax = 3f;
        //Store flag to know if explosion was because of a bad answer or general explosion
        private bool wrongAnswer = false;
        //Variable to save remaining lives
        [SerializeField]
        public int lives = 3;
        //Variable to see the amount of explosions that has happened, for testing purposes
        public int explosions = 0;

        //variable to generate random numbers for questioning
        System.Random groupSeed = new System.Random();
        System.Random questionSeed = new System.Random();

        //Event to update Static UI
        public delegate void AssignGroupName(int i, string name);

        public static event AssignGroupName OnAssignGroupName;

        private HashSet<int> inScene = new HashSet<int>();
        // inScene checks which question indexes are floating in the scene at the moment


        private bool inProgress = false;
        [SerializeField]
        private GameObject eventSystem;

        /// <summary>
        /// At the start, initiate new levelGrid, set correct prefab and summon a spawnbeacon to spawn a bomb
        /// </summary>
        void Start()
        {
            if (GameObject.Find("EventSystem") == null)
            {
                GameObject eventSystemGameObject = Instantiate(eventSystem);
                eventSystemGameObject.SetActive(true);
            }

            
        }

        public void StartGame()
        {
            if (inProgress)
            {
                return;
            }

            SpawnTimer = 0;

            // remove canvas
            GameObject.Find("Explanation Canvas").GetComponent<Canvas>().enabled = false;

            inProgress = true;
            levelGrid = new LevelGrid(28, 17);
            levelGrid.SetBombPrefab(bombprefab);

            SetupGroupNames();
        }

        /// <summary>
        /// Get function for the levelgrid, for testing
        /// </summary>
        public LevelGrid GetLevelGrid()
        {
            return levelGrid;
        }


        /// <summary>
        /// Method to set all the groupnames in the UI
        /// </summary>
        private void SetupGroupNames()
        {

            if (GameManager.Instance != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    OnAssignGroupName?.Invoke(i, GameManager.Instance.GetMinigame<Grouping>().groupNames[i]);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    OnAssignGroupName?.Invoke(i, MockData.instance.minigame.groupNames[i]);
                }
            }
        }

        // Update is called once per frame
        /// <summary>
        /// Every Frame check if new bomb spawning needs to happen
        /// </summary>
        void Update()
        {
            if (!inProgress)
            {
                return;
            }

            if (first)
            {
                levelGrid.SpawnBeacon();
                first = false;
            }
            else
            {
                SpawnTimer += Time.deltaTime;
                double speed = (GameManager.Instance != null) ? GameManager.Instance.minigame.speed : 0.7;
                if (SpawnTimer >= (SpawnTimerMax + 2f) - (SpawnTimerMax * speed ))
                {
                    if (levelGrid.endGame) return;
                    levelGrid.SpawnBeacon();
                    SpawnTimer = 0;
                }

            }
        }

        //Subscribe to OnBombPlaced so answers can be checked
        private void OnEnable()
        {
            Bomb.OnBombPlaced += CheckAnswer;
            Bomb.OnBombExplosion += Explode;
            Bomb.OnSpawn += AssignValue;
        }

        //Unsubscribe to OnBombPlace when disabled
        private void OnDisable()
        {
            Bomb.OnBombPlaced -= CheckAnswer;
            Bomb.OnBombExplosion -= Explode;
            Bomb.OnSpawn -= AssignValue;
        }

        /// <summary>
        ///     Adjust Variable, for testing
        /// </summary>
        public void Explode()
        {
            explosions += 1;
            LoseLive();
        }

        /// <summary>
        ///     Assign a value to this bomb.
        /// </summary>
        public void AssignValue(GameObject bomb)
        {
            int index;
            int count = 0;
            do
            {
                count++;
                if (GameManager.Instance != null)
                {
                    index = GameManager.Instance.AL.GetNextQuestionIndex();
                    Debug.Log(index);
                }
                else
                {
                    index = MockData.instance.GetNextQuestionIndex();
                }

                if (index == -1)
                {
                    StopGame();
                    return;
                }
            } while (inScene.Contains(index) && count < 50);

            inScene.Add(index);

            if (GameManager.Instance != null)
            {
                bomb.GetComponent<Bomb>().value = GameManager.Instance.GetMinigame<Grouping>().elements[index];
                bomb.GetComponent<Bomb>().corrAnswer = GameManager.Instance.GetMinigame<Grouping>().correctGroups[index];
            }
            else
            {
                bomb.GetComponent<Bomb>().value = MockData.instance.minigame.elements[index];
                bomb.GetComponent<Bomb>().corrAnswer = MockData.instance.minigame.correctGroups[index];
            }

            bomb.GetComponent<Bomb>().givenIndex = index;
        }

        /// <summary>
        /// Function to check if given answer is correct
        /// </summary>
        public void CheckAnswer(int place, GameObject go)
        {

            if (place == go.GetComponent<Bomb>().corrAnswer)
            {
                if (GameManager.Instance != null)
                {
                    // CORRECT ANSWER
                    GameManager.Instance.AL.AnswerQuestion(go.GetComponent<Bomb>().givenIndex, true, go.GetComponent<Bomb>().totCountDown);
                }
                go.GetComponent<Bomb>().state = Bomb.State.stored;
                Debug.Log("state changed");

            }
            else
            {
                inScene.Remove(go.GetComponent<Bomb>().givenIndex);

                // WRONG ANSWER
                //Debug.Log("Incorrect Answer, Answer given to question " + go.GetComponent<Bomb>().value +  " was Group:" + GameManager.Instance.GetMinigame<Grouping>().groupNames[place] + ". But the correct Answer was:" + GameManager.Instance.GetMinigame<Grouping>().groupNames[GameManager.Instance.GetMinigame<Grouping>().correctGroups[(go.GetComponent<Bomb>().corrAnswer)]]);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AL.AnswerQuestion(go.GetComponent<Bomb>().givenIndex, false, go.GetComponent<Bomb>().totCountDown);
                }

                Destroy(go);
                LoseLive();
            }

        }

        /// <summary>
        /// Function to trigger endgame when a wrong answer has been given
        /// </summary>
        private void WrongAnswer()
        {

            wrongAnswer = true;
            StopGame();
        }

        /// <summary>
        /// Function to end game, count and display the score
        /// </summary>
        private void StopGame()
        {

            DeleteGameObjectsByName("Bomb");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AL.Score();
                GameObject.Find("Minigame Camera").GetComponent<AudioListener>().enabled = false; //disable minigame camera
                GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = true; //enable game camera
                GameManager.Instance.changeStatus(GameManager.Status.inGame);
                SceneLoader.Unload("GroupBomb");
            }

            SpawnTimer = 0;
            levelGrid.EndGame();
        }

        public void ForceStopGame()
        {
            levelGrid.ForceStopGame();
        }

        public static void DeleteGameObjectsByName(string objectName)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(objectName);

            foreach (GameObject gameObject in gameObjects)
            {
                Destroy(gameObject);
            }
        }

        private void LoseLive()
        {
            if (lives == 3)
            {
                lives -= 1;
                Destroy(GameObject.Find("Heart3"));
            }
            else if (lives == 2)
            {
                lives -= 1;
                Destroy(GameObject.Find("Heart2"));
            }
            else if (lives == 1)
            {
                lives -= 1;
                Destroy(GameObject.Find("Heart1"));
                WrongAnswer();
            }
            else
            {
                lives -= 1;
            }
        }



    }
}