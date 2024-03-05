using GameManagement;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TowerDefense;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Lobby
{
    /// <summary>
    /// Handles player input
    /// </summary>
    public class PlayerInputHandler : NetworkBehaviour
    {
        private Rigidbody2D playerRigidBody;
        private PlayerInputActionsTD playerInputActions;
        private Animator animator;

        private NetworkVariable<bool> online = new NetworkVariable<bool>(value: false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private NetworkVariable<Vector3> networkMoveDir = new NetworkVariable<Vector3>();
        private NetworkVariable<float> networkAnimatorX = new NetworkVariable<float>();
        private NetworkVariable<float> networkAnimatorY = new NetworkVariable<float>();
        private NetworkVariable<bool> networkAnimatorWalkingState = new NetworkVariable<bool>();

        private bool HUD_shown = true;
        public bool spaceAllowed = false;
        public int hotspot;
        private void Awake()
        {
            online.OnValueChanged += (prevValue, newValue) => { online.Value = newValue; };
            playerRigidBody = GetComponent<Rigidbody2D>();
            playerInputActions = new PlayerInputActionsTD();
            animator = GetComponent<Animator>();
            Subscribe();
        }

        /// <summary>
        /// Sets the player field in the game manager
        /// </summary>
        private void Start()
        {
            if (online.Value && IsOwner || !online.Value)
            {
                GameManager.Instance.SetPlayer(gameObject.GetComponent<NetworkObject>());
            }
        }

        public override void OnDestroy()
        {
            Unsubscribe();
        }

        /// <summary>
        /// Subscribes the player input actions
        /// </summary>
        public void Subscribe()
        {
            playerInputActions.Player.Enable();
            playerInputActions.Player.Movement.performed += Movement_performed;
            playerInputActions.Player.Movement.canceled += Movement_canceled;
            playerInputActions.Player.OpenMinigame.performed += PressedSpace;
        }

        /// <summary>
        /// Unsubscribes the player input actions
        /// </summary>
        public void Unsubscribe()
        {
            playerInputActions.Player.Movement.performed -= Movement_performed;
            playerInputActions.Player.Movement.canceled -= Movement_canceled;
            playerInputActions.Player.OpenMinigame.performed -= PressedSpace;
            playerInputActions.Player.Disable();
        }

        /// <summary>
        /// Handles spacebar input
        /// </summary>
        /// <param name="context"></param>
        private void PressedSpace(InputAction.CallbackContext context)
        {
            if (online.Value && IsOwner || !online.Value)
            {
                if (spaceAllowed)
                {
                    if (online.Value)
                    {
                        UpdateClientAvatarStateServerRpc(networkAnimatorX.Value, networkAnimatorY.Value, false);
                        UpdateClientAvatarPositionServerRpc(Vector3.zero);
                    }
                    else
                    {
                        UpdateClientAvatarState(networkAnimatorX.Value, networkAnimatorY.Value, false);
                        UpdateClientAvatarPosition(Vector3.zero);
                    }
                    GameManager.Instance.hotspotOpened = hotspot;
                    GameManager.Instance.StartMinigame();
                }
            }
            if (online.Value && NetworkManager.Singleton.IsHost)
            {
                if (HUD_shown)
                {
                    //unload HUD
                    Debug.Log("Unload HUD");
                    GameObject.Find("HUDUI").GetComponent<HUDUI>().unloadHUD();
                    HUD_shown = false;
                }
                else
                {
                    //load HUD
                    Debug.Log("Load HUD");
                    GameObject.Find("HUDUI").GetComponent<HUDUI>().loadHUD();
                    HUD_shown = true;
                }
            }
        
        }

        /// <summary>
        /// Handles Arrowkeys release
        /// </summary>
        /// <param name="context"></param>
        private void Movement_canceled(InputAction.CallbackContext context)
        {
            if (online.Value && IsOwner || !online.Value)
            {
                if (online.Value)
                {
                    UpdateClientAvatarStateServerRpc(networkAnimatorX.Value, networkAnimatorY.Value, false);
                    UpdateClientAvatarPositionServerRpc(Vector3.zero);
                }
                else
                {
                    UpdateClientAvatarState(networkAnimatorX.Value, networkAnimatorY.Value, false);
                    UpdateClientAvatarPosition(Vector3.zero);
                }
            }

        }

  


        private void FixedUpdate()
        {
            ClientMove();
        }

        /// <summary>
        /// Moves player
        /// </summary>
        private void ClientMove()
        {
            if (networkMoveDir.Value != Vector3.zero)
            {
                playerRigidBody.transform.position += networkMoveDir.Value * 3f * Time.deltaTime;

                animator.SetFloat("X", networkAnimatorX.Value);
                animator.SetFloat("Y", networkAnimatorY.Value);

                animator.SetBool("IsWalking", networkAnimatorWalkingState.Value);
            }
            if (networkMoveDir.Value == Vector3.zero && animator.GetBool("IsWalking") == true)
            {
                animator.SetBool("IsWalking", networkAnimatorWalkingState.Value);
            }

        }


        /// <summary>
        /// Handles Arrowkeys press
        /// </summary>
        /// <param name="context"></param>
        private void Movement_performed(InputAction.CallbackContext context)
        {
            if (online.Value && IsOwner || !online.Value)
            {
                Vector2 inputVector = context.ReadValue<Vector2>();
                Vector3 moveDir = new Vector3(inputVector.x, inputVector.y, 0);

                if (moveDir != Vector3.zero)
                {
                    if (online.Value)
                    {
                        UpdateClientAvatarPositionServerRpc(moveDir);
                        UpdateClientAvatarStateServerRpc(moveDir.x, moveDir.y, true);
                    }
                    else
                    {
                        UpdateClientAvatarPosition(moveDir);
                        UpdateClientAvatarState(moveDir.x, moveDir.y, true);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the online variable
        /// </summary>
        /// <param name="online"></param>
        public void SetOnline(bool online)
        {
            this.online.Value = online;
        }


        /// <summary>
        /// Updates Client's player position (Server RPC)
        /// </summary>
        /// <param name="moveDir"></param>
        [ServerRpc]
        public void UpdateClientAvatarPositionServerRpc(Vector3 moveDir)
        {
            networkMoveDir.Value = moveDir;
        }

        /// <summary>
        /// Updates Client's player position (offline)
        /// </summary>
        /// <param name="moveDir"></param>
        public void UpdateClientAvatarPosition(Vector3 moveDir)
        {
            networkMoveDir.Value = moveDir;
        }

        /// <summary>
        /// Updates Client's player animation state (Server RPC)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isWalking"></param>
        [ServerRpc]
        public void UpdateClientAvatarStateServerRpc(float x, float y, bool isWalking)
        {
            networkAnimatorX.Value = x;
            networkAnimatorY.Value = y;
            networkAnimatorWalkingState.Value = isWalking;
        }

        /// <summary>
        /// Updates Client's player animation state (offline)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isWalking"></param>
        public void UpdateClientAvatarState(float x, float y, bool isWalking)
        {
            networkAnimatorX.Value = x;
            networkAnimatorY.Value = y;
            networkAnimatorWalkingState.Value = isWalking;
        }

        /// <summary>
        /// Returns whether the spacebar input is enabled or not
        /// </summary>
        /// <returns></returns>
        public bool SpaceEnabled()
        {
            return spaceAllowed;
        }

        /// <summary>
        /// Enables spacebar input
        /// </summary>
        public void EnableSpace()
        {
            spaceAllowed = true;
        }

        /// <summary>
        /// Disables spacebar input
        /// </summary>
        public void DisableSpace()
        {
            spaceAllowed = false;
        }

        /// <summary>
        /// Sets the hotspot variable
        /// </summary>
        /// <param name="hotspot"></param>
        public void SetHotspot(int hotspot)
        {
            this.hotspot = hotspot;
        }
    }
}


