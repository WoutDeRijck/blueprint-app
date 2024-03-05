using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby
{
    /// <summary>
    /// Creates a game manager and spawns a player if there is none found
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        public GameObject GameManagerPrefab;
        public GameObject NetworkPlayerPrefab;

        void Start()
        {
            if (GameObject.Find("GameManager") == null)
            {
                Debug.Log("no gamemanager found, creating one");
                Instantiate(GameManagerPrefab);
                GameObject networkPlayer = Instantiate(NetworkPlayerPrefab);
                networkPlayer.GetComponent<NetworkPlayer>().SpawnPlayer(false);
            }
        }
    }

}
