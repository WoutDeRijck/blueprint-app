using GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Snake
{
    /// <summary>
    /// Holds Snake game assets
    /// </summary>
    public class MockData : MonoBehaviour
    {
        public static MockData instance;
        public MultipleChoice minigame;
        private int index = -1;

        private void Awake()
        {
            instance = this;

            List<int> difficulties = new List<int>() { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3 };
            List<string> questions = new List<string>() {   "easy_1", "easy_2", "easy_3", "easy_4", "easy_5",
                                                        "medium_1", "medium_2", "medium_3", "medium_4", "medium_5",
                                                        "hard_1", "hard_2", "hard_3", "hard_4", "hard_5" };
            List<string> correctAnswers = new List<string>() { "x", "x", "x", "x", "x", "x", "x", "x", "x", "x", "x", "x", "x", "x", "x" };
            List<List<string>> wrongAnswers = new List<List<string>>();

            for (int i = 0; i < 15; i++)
            {
                wrongAnswers.Add(new List<string>() { "o", "o", "o" });
            }

            minigame = new MultipleChoice(
                Subject.Mathematics,
                "MockMinigame",
                difficulties,
                questions,
                correctAnswers,
                wrongAnswers);
        }

        /// <summary>
        /// Returns a new question index to index the minigame arrays
        /// </summary>
        /// <returns></returns>
        public int GetNextQuestionIndex()
        {
            if (index < 14)
            {
                index++;
                return index;
            }
            else { return -1; }
        }
    }
}
