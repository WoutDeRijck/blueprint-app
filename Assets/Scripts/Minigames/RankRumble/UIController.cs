using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RankRumble
{

/// <summary>
/// Controls UI Aspects of the Minigame
/// </summary>
public class UIController : MonoBehaviour
{

    /// <summary>
    /// Save labels for all adjustable fields
    /// </summary>
    private Label question;
    private Label leftAnswer;
    private Label topAnswer;
    private Label rightAnswer;
    private Label bottomAnswer;
    private Label streak;
    private Label timer;

    /// <summary>
    /// Setup default questions in order to initialize
    /// </summary>
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        GameHandler.OnAssignUI += ChangeName;
        question = root.Q<Label>("Question");
        leftAnswer = root.Q<Label>("LeftAnswer");
        rightAnswer = root.Q<Label>("RightAnswer");
        topAnswer = root.Q<Label>("TopAnswer");
        bottomAnswer = root.Q<Label>("BottomAnswer");
        streak = root.Q<Label>("StreakCounter");
        timer = root.Q<Label>("Timer");
        leftAnswer.text = "None";
        rightAnswer.text = "None";
        bottomAnswer.text = "None";
        topAnswer.text = "None";
        streak.text = "0";
        timer.text = "0";
    }

    private void OnDisable()
    {
        GameHandler.OnAssignUI -= ChangeName;
    }

    /// <summary>
    /// Function that is subscribed to OnAssignUI in GameHandler, changes UI of Text fields
    /// </summary>
    private void ChangeName(string location, string name)
    {
        if (location == "question") 
        {
            question.text = name;
        } else if (location == "TopAnswer") {
            topAnswer.text = name;
        } else if (location == "BottomAnswer") {
            bottomAnswer.text = name;
        } else if (location == "LeftAnswer") {
            leftAnswer.text = name;
        } else if (location == "RightAnswer") {
            rightAnswer.text = name;
        } else if (location == "Streak") {
            streak.text = name;
        } else if (location == "Timer") {
            timer.text = name;
        } else {
            Debug.Log("Invalid Input");
        }
    }

}
}
