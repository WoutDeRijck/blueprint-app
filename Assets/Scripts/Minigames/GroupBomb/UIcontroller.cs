using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GroupBomb
{

/// <summary>
/// Controls UI Aspects of the Minigame
/// </summary>
public class UI : MonoBehaviour
{

    //Save references to Group texts
    private Label group1;
    private Label group2;
    private Label group3;
    private Label group4;

    //TextElement text;
    //Grouping minigame = MinigameManager.instance.GetMinigame<Grouping>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Assign correct label for further reference
    /// </summary>
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        GameHandler.OnAssignGroupName += ChangeName;
        group1 = root.Q<Label>("Group1");
        group2 = root.Q<Label>("Group2");
        group3 = root.Q<Label>("Group3");
        group4 = root.Q<Label>("Group4");
        //Debug.Log(group1.text + group2.text + group3.text + group4.text);
    }

    private void OnDisable()
    {
        GameHandler.OnAssignGroupName -= ChangeName;
    }

    /// <summary>
    /// Function used to setup correct Groupnames of the UI
    /// </summary>
    private void ChangeName(int i, string name)
    {
        switch (i)
        {
            case 0:
                group1.text = name; 
                break;
            case 1:
                group2.text = name;
                break;
            case 2:
                group3.text = name;
                break;
            case 3:
                group4.text = name;
                break;

            default:
                Debug.Log("Invalid Input (int i in ChangeName)");
                break;
        }
    }

}
}