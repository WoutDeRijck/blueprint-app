using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

namespace RankRumble
{

/// <summary>
/// Holds RankRumble game data preset
/// </summary>
public class MockData : MonoBehaviour
{
    public static MockData instance;
    public Ranking minigame;
    public int index = -1;

    /// <summary>
    /// Instantiate correctly upon Awake
    /// </summary>
    private void Awake()
    {

        instance = this;

        List<string> arg1 = new List<string>() {"left", "right", "top", "bottom"};
        List<string> arg2 = new List<string>() {"right", "left", "top", "bottom"};
        List<string> arg3 = new List<string>() {"top", "right", "left", "bottom"};

        List<int> difficulties = new List<int>() { 1, 1, 1, 1, 2, 2, 3, 3 };
        List<string> questions = new List<string>() {   "easy_1", "easy_2", "easy_3", "easy_4", "medium_1",
                                                        "medium_2", "hard_1", "hard_2" };
        List<List<string>> correctRankings = new List<List<string>>();

        correctRankings.Add(arg1);
        correctRankings.Add(arg1);
        correctRankings.Add(arg1);
        correctRankings.Add(arg1);
        correctRankings.Add(arg2);
        correctRankings.Add(arg2);
        correctRankings.Add(arg3);
        correctRankings.Add(arg3);

        minigame = new Ranking(
            Subject.Mathematics,
            "MockMinigame",
            difficulties,
            questions,
            correctRankings);
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
        else { return -1; }
    }
}
}
