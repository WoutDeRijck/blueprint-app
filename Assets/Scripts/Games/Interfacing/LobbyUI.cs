using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Misc;
using GameData;
using GameManagement;

namespace Lobby
{

    /// <summary>
    /// Handles lobby UI
    /// </summary>
    public class LobbyUI : MonoBehaviour
    {
        public void Start()
        {
            SubscribeToGameDataChanges();
        }

        /// <summary>
        /// subscribes the GameData object to changes in the NetworkGameData dataString
        /// </summary>
        public void SubscribeToGameDataChanges()
        {
            string gameString;
            GameObject.Find("GameData").GetComponent<NetworkGameData>().GetDataString().OnValueChanged += (Netstring oldVal, Netstring newVal) =>
            {
                gameString = newVal.st;
                CreateGame(gameString);
            };
        }

        /// <summary>
        /// Creates a Game object from a JSON string and updates the game manager
        /// </summary>
        /// <param name="gameJSON"></param>
        private void CreateGame(string gameJSON)
        {
            Game game = FileHandler.ReadFromJSONString<Game>(gameJSON);
            GameManager.Instance.Initialize(game);
        }

        /// <summary>
        /// Attaches functionality to the lobby UI elements
        /// </summary>
        public void SetupUI()
        {
            // fetch lobby UI elements
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            Button startGameButton = root.Q<Button>("button-start-game");
            Label lobbyID = root.Q<Label>("lobby-id");

            // add functionality to buttons
            startGameButton.RegisterCallback<ClickEvent>(StartGameButton_clicked);
            SpawnLobbyCode(lobbyID);
        }

        /// <summary>
        /// Updates the Netstring dataString and starts the game
        /// </summary>
        /// <param name="evt"></param>
        private void StartGameButton_clicked(ClickEvent evt)
        {
            string gameString = FileHandler.SaveToJSONString(GameManager.Instance.game);

            GameObject.Find("GameData").GetComponent<NetworkGameData>().GetDataString().Value = new Netstring() { st = gameString };

            GameManager.Instance.changeStatus(GameManager.Status.inGame);
            NetworkManager.Singleton.SceneManager.LoadScene(GameManager.Instance.game.firstScene.ToString(), LoadSceneMode.Single);
        }

        /// <summary>
        /// Displays the lobby code
        /// </summary>
        /// <param name="lobbyID"></param>
        private void SpawnLobbyCode(Label lobbyID)
        {
            lobbyID.text = GameManager.Instance.joinCode;
        }
    }
}
