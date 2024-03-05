using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

namespace GroupBomb
{

    /// <summary>
    /// holds GroupBomb sample data
    /// </summary>
    public class MockData : MonoBehaviour
    {

        public static MockData instance;
        public Grouping minigame;
        public int index = -1;

        /// <summary>
        /// Instantiate correctly upon Awake
        /// </summary>
        private void Awake()
        {

            instance = this;

            List<int> difficulties = new List<int>() { 1, 1, 1, 1, 2, 2, 3, 3 };
            List<string> groupNames = new List<string>() {
                "Group_A", "Group_B", "Group_C", "Group_D"
            };
            List<string> elements = new List<string>() {
                "A", "A", "A", "B", "B", "B", "C", "D"
            };
            List<int> correctGroups = new List<int>() {
                0, 0, 0, 1, 1, 1, 2, 3
            };

            minigame = new Grouping(
                Subject.Mathematics,
                "MockMinigame",
                difficulties,
                groupNames,
                correctGroups,
                elements);
        }

        /// <summary>
        /// Function used to avoid Adaptive learning
        /// </summary>
        public int GetNextQuestionIndex()
        {
            if (index < 7)
            {
                index++;
                return index;
            }
            else { return -1; 
            }
        }
    }
}