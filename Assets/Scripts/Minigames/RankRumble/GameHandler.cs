using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GameData;
using Misc;
using GameManagement;
using UnityEngine.EventSystems;

namespace RankRumble
{

    /// <summary>
    /// Brings all objects together and controls them
    /// </summary>
    public class GameHandler : MonoBehaviour
    {

        //Make event to Setup correct UI values
        public delegate void AssignUI(string location, string name);
        public static event AssignUI OnAssignUI;

        //booleans to make sure every answer is only given once
        private bool topPressed = false;
        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool bottomPressed = false;

        //Save where answer values are located
        public string topValue;
        public string bottomValue;
        public string leftValue;
        public string rightValue;

        //Save current questionindex
        public int index;

        //Keep the winning streak
        public int streak = 0;

        //save answer given
        public List<string> answers = new List<string>();

        //save time taken to answer the question
        private float timer;

        //Save Max Time
        private float MaxTimer = 40f;

        //Keep reference to player
        public Player player;

        //Save Countdown for UI
        private decimal Countdown;
        //Variable to save remaining lives
        private int lives = 3;
        //bool for testing
        private bool countedAlready = false;

        private bool inProgress = false;

        [SerializeField]
        GameObject eventSystem;


        private void Start()
        {
            if (GameObject.Find("EventSystem") == null)
            {
                GameObject eventSystemGameObject = Instantiate(eventSystem);
                eventSystemGameObject.SetActive(true);
            }
        }

            public void StartGame()
        {
            if (inProgress) return;
            inProgress = true;

            // remove canvas
            GameObject.Find("Explanation Canvas").GetComponent<Canvas>().enabled = false;

            SetupUI();
        }


        // Update is called once per frame
        void Update()
        {
            if (!inProgress) return;

            HandleInput();
            CheckAnswer();
            TimeCheck();
            SetCountdown();
        }

        /// <summary>
        /// Check for Player input, valid input is a arrowkey and input is added to answers list
        /// </summary>
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && !topPressed)
            {
                ProcessAnswer("top");
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && !bottomPressed)
            {
                ProcessAnswer("bottom");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && !rightPressed)
            {
                ProcessAnswer("right");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && !leftPressed)
            {
                ProcessAnswer("left");
            }
            else
            {
                //Debug.Log("Invalid Input");
            }
        }

        /// <summary>
        /// Function to process the answer accordingly
        /// </summary>
        public void ProcessAnswer(string place)
        {
            if (place == "top")
            {
                player.SetSprite("top");
                answers.Add(topValue);
                topPressed = true;
            }
            else if (place == "right")
            {
                player.SetSprite("right");
                answers.Add(rightValue);
                rightPressed = true;
            }
            else if (place == "left")
            {
                player.SetSprite("left");
                answers.Add(leftValue);
                leftPressed = true;
            }
            else
            {
                player.SetSprite("bottom");
                answers.Add(bottomValue);
                bottomPressed = true;
            }
        }

        /// <summary>
        /// Set Timer to UI
        /// </summary>
        public void SetCountdown()
        {
            OnAssignUI?.Invoke("Timer", Countdown.ToString());
        }

        /// <summary>
        /// Check if time has ran out
        /// </summary>
        private void TimeCheck()
        {
            timer += Time.deltaTime;
            Countdown -= Math.Round((decimal)Time.deltaTime, 2);

            if (GameManager.Instance != null)
            {
                if (timer >= MaxTimer - MaxTimer * GameManager.Instance.GetMinigame<Ranking>().speed)
                {
                    OutOfTime();
                }
            }
            else
            {
                if (timer >= MaxTimer)
                {
                    OutOfTime();
                }
            }
        }

        /// <summary>
        /// Check if answer (all 4 of them together) are correct
        /// </summary>
        private void CheckAnswer()
        {

            if (topPressed && bottomPressed && leftPressed && rightPressed)
            {
                if (GameManager.Instance != null)
                {
                    if (Enumerable.SequenceEqual(answers, GameManager.Instance.GetMinigame<Ranking>().correctRankings[index]))
                    {
                        streak += 1;
                        GameManager.Instance.AL.AnswerQuestion(index, true, timer);
                        player.SetSprite("happy");
                        LoadNextQuestion();
                    }
                    else
                    {
                        streak = 0;
                        GameManager.Instance.AL.AnswerQuestion(index, false, timer);
                        player.SetSprite("sad");
                        LoseLive();
                    }
                }
                else
                {
                    if (!countedAlready)
                    {
                        countedAlready = true;
                        if (Enumerable.SequenceEqual(answers, MockData.instance.minigame.correctRankings[index]))
                        {
                            streak += 1;
                            player.SetSprite("happy");
                        }
                        else
                        {
                            streak = 0;
                            player.SetSprite("sad");
                            LoseLive();
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Function to update adaptive learning and load new question after the countdown has reached 0
        /// </summary>
        private void OutOfTime()
        {
            if (GameManager.Instance != null)
            {
                streak = 0;
                GameManager.Instance.AL.AnswerQuestion(index, false, timer);
                player.SetSprite("sad");
                LoseLive();
            }
            else
            {
                streak = 0;
                player.SetSprite("sad");
                LoseLive();
            }
        }

        /// <summary>
        /// Sets up the UI
        /// Updates the timer, question and all answers
        /// Next index gets loaded in here
        /// </summary>
        private void SetupUI()
        {
            if (GameManager.Instance != null)
            {
                Countdown = Math.Round((decimal)(MaxTimer - (float)(MaxTimer * (float)GameManager.Instance.GetMinigame<Ranking>().speed)), 2);
                timer = 0f;
                index = GameManager.Instance.AL.GetNextQuestionIndex();
                if (index == -1)
                {
                    EndGame();
                }
                else
                {
                    OnAssignUI?.Invoke("question", GameManager.Instance.GetMinigame<Ranking>().questions[index]);
                    OnAssignUI?.Invoke("Streak", streak.ToString());
                    RandomlyAssignAnswers(GameManager.Instance.GetMinigame<Ranking>().correctRankings[index]);
                }
            }
            else
            {
                Countdown = Math.Round((decimal)(MaxTimer), 2);
                timer = 0f;
                index = MockData.instance.GetNextQuestionIndex();
                if (index == -1)
                {
                    EndGame();
                }
                else
                {
                    OnAssignUI?.Invoke("question", MockData.instance.minigame.questions[index]);
                    OnAssignUI?.Invoke("Streak", streak.ToString());
                    RandomlyAssignAnswers(MockData.instance.minigame.correctRankings[index]);
                }
            }
        }

        /// <summary>
        /// Function to return to Game after the ending of the game
        /// </summary>
        private void EndGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AL.Score();
                GameObject.Find("Minigame Camera").GetComponent<AudioListener>().enabled = false; //disable minigame camera
                GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = true; //enable game camera
                GameManager.Instance.changeStatus(GameManager.Status.inGame);
                SceneLoader.Unload("RankRumble");
            }
            else
            {
                Debug.Break();
            }
        }

        /// <summary>
        /// Debug Function
        /// </summary>
        public void DebugList(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Debug.Log(list[i]);
            }
        }

        /// <summary>
        /// Function that resets booleans and calls SetupUI()
        /// </summary>
        public void LoadNextQuestion()
        {
            countedAlready = false;
            topPressed = false;
            bottomPressed = false;
            leftPressed = false;
            rightPressed = false;
            answers = new List<string>();
            SetupUI();
        }

        /// <summary>
        /// Function to distribute answers randomly in every direction
        /// </summary>
        private void RandomlyAssignAnswers(List<string> answers)
        {
            if (GameManager.Instance != null)
            {
                int left = UnityEngine.Random.Range(0, 4);
                int right = UnityEngine.Random.Range(0, 4);
                int bottom = UnityEngine.Random.Range(0, 4);
                int top = UnityEngine.Random.Range(0, 4);

                while (right == left)
                {
                    right = UnityEngine.Random.Range(0, 4);
                    //right = answerSeed.Next(0, 4);
                }
                while (top == left || top == right)
                {
                    top = UnityEngine.Random.Range(0, 4);
                    //top = answerSeed.Next(0, 4);
                }

                while (bottom == left || bottom == right || bottom == top)
                {
                    bottom = UnityEngine.Random.Range(0, 4);
                    //bottom = answerSeed.Next(0, 4);
                }

                AssignNumberToLabel(left, answers, "left");
                AssignNumberToLabel(right, answers, "right");
                AssignNumberToLabel(bottom, answers, "bottom");
                AssignNumberToLabel(top, answers, "top");
            }
            else
            {
                AssignNumberToLabel(0, answers, "left");
                AssignNumberToLabel(1, answers, "right");
                AssignNumberToLabel(2, answers, "top");
                AssignNumberToLabel(3, answers, "bottom");
            }
        }

        /// <summary>
        /// Function to update certain answer fields in the UI
        /// </summary>
        private void AssignNumberToLabel(int i, List<string> answers, string location)
        {

            if (location == "left")
            {
                leftValue = answers[i];
                OnAssignUI?.Invoke("LeftAnswer", leftValue);
            }
            else if (location == "right")
            {
                rightValue = answers[i];
                OnAssignUI?.Invoke("RightAnswer", rightValue);
            }
            else if (location == "top")
            {
                topValue = answers[i];
                OnAssignUI?.Invoke("TopAnswer", topValue);
            }
            else if (location == "bottom")
            {
                bottomValue = answers[i];
                OnAssignUI?.Invoke("BottomAnswer", bottomValue);
            }
        }

        /// <summary>
        /// Function to process the loss of live, adjusts hearts and loads next questions if there are still more lives left
        /// </summary>
        private void LoseLive()
        {
            if (lives == 3)
            {
                lives -= 1;
                Destroy(GameObject.Find("Heart3"));
                LoadNextQuestion();
            }
            else if (lives == 2)
            {
                lives -= 1;
                Destroy(GameObject.Find("Heart2"));
                LoadNextQuestion();
            }
            else
            {
                lives -= 1;
                Destroy(GameObject.Find("Heart1"));
                EndGame();
            }
        }


    }
}
