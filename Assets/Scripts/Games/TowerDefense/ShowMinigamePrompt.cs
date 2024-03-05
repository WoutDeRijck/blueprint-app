using Lobby;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace TowerDefense
{
    /// <summary>
    /// Shows an instruction message ("press space to play minigame") when a player enters the trigger region.
    /// </summary>
    public class ShowMinigamePrompt : MonoBehaviour
    {
        private UIDocument minigamePromptUIDocument; // "press space to play minigame"
        PlayerInputHandler input;
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private bool inZone = false;

        private void OnEnable()
        {
            SetupUI();
            minigamePromptUIDocument.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && inZone)
            {
                minigamePromptUIDocument.enabled = false;
            }
        }

        /// <summary>
        /// Show minigamePromptUIDocument on screen ("press space to play minigame")
        /// </summary>
        /// <param name="playerCollider"></param>
        public void OnTriggerEnter2D(Collider2D playerCollider)
        {
            if (playerCollider.tag == "Player")
            {
                if (!playerCollider.gameObject.GetComponent<NetworkObject>().IsOwner)
                {
                    return;
                }
                input = playerCollider.GetComponent<PlayerInputHandler>();
                input.EnableSpace();
                input.SetHotspot((int)Variables.Object(gameObject).Get("hotspot"));
                minigamePromptUIDocument.enabled = true;

                inZone = true;
            }
        }

        /// <summary>
        /// Remove minigamePromptUIDocument from screen ("press space to play minigame")
        /// </summary>
        /// <param name="playerCollider"></param>
        private void OnTriggerExit2D(Collider2D playerCollider)
        {
            if (playerCollider.tag == "Player")
            {
                if (!playerCollider.gameObject.GetComponent<NetworkObject>().IsOwner)
                {
                    return;
                }
                input = playerCollider.GetComponent<PlayerInputHandler>();
                input.DisableSpace();
                minigamePromptUIDocument.enabled = false;

                inZone = false;
            }
        }

        /// <summary>
        /// Sets up UI
        /// </summary>
        private void SetupUI()
        {
            minigamePromptUIDocument = GetComponent<UIDocument>();
        }
    }
}