using Lobby;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;


namespace Lobby
{
    /// <summary>
    /// Handles online player spawning 
    /// </summary>
    public class NetworkPlayer : NetworkBehaviour
    {
        /// <summary>
        /// Student player prefab
        /// </summary>
        public GameObject studentPrefab;
        /// <summary>
        /// Teacher player prefab
        /// </summary>
        public GameObject teacherPrefab;
        private NetworkObject playerNetworkObject;
        private int clientID;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            clientID = (int)OwnerClientId;
            Debug.Log(clientID);

            if (IsServer)
            {
                UIDocument UIdoc = GameObject.Find("UIDocument").GetComponent<UIDocument>();

                if (UIdoc.enabled == false)
                {
                    GameObject lobbyInfo = GameObject.Find("LobbyInformation");
                    UIdoc.enabled = true;
                    lobbyInfo.SetActive(true);
                    lobbyInfo.GetComponent<Canvas>().enabled = true;
                    UIdoc.GetComponent<LobbyUI>().SetupUI();
                }

                SpawnPlayer(true);
            }
        }

        /// <summary>
        /// Spawns player (with ownership if networked)
        /// </summary>
        /// <param name="networked"></param>
        public void SpawnPlayer(bool networked)
        {
            if (networked)
            {
                GameObject playerObject;
                if (OwnerClientId != 0)
                {
                    playerObject = Instantiate(studentPrefab);

                }
                else
                {
                    playerObject = Instantiate(teacherPrefab);
                }

                playerObject.GetComponent<PlayerInputHandler>().SetOnline(true);
                playerNetworkObject = playerObject.GetComponent<NetworkObject>();
                playerNetworkObject.SpawnWithOwnership(OwnerClientId);
            }
            else
            {
                // spawn a player object that is not networked
                GameObject playerObject = Instantiate(studentPrefab);
            }
        }


    }
}

