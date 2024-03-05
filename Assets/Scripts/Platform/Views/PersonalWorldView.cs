using UnityEngine;
using UnityEngine.UIElements;

using Platform.Controllers;
using Platform.Interfacing.Systems;

namespace Platform.Views
{

public class PersonalWorldView : MonoBehaviour
{

    private VisualElement root;
    private Button playButton;
    private Button logoutButton;
    private TextField lobbyField;
    private Label nameLabel;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        playButton = root.Q<Button>("button-play");
        logoutButton = root.Q<Button>("button-logout");
        lobbyField = root.Q<TextField>("field-lobby");
        nameLabel = root.Q<Label>("label-name");

        playButton.clicked += Play;
        logoutButton.clicked += Logout;

        nameLabel.text = "Hey, " + AuthenticationSystem.Instance.student.name;

    }

    public void Play()
    {
        if (lobbyField.text != "")
        {
            PersonalWorldController.Instance.Submit(lobbyField.text);
        }
        
    }

    public void Logout()
    {
        PersonalWorldController.Instance.Logout();
    }
}

}
