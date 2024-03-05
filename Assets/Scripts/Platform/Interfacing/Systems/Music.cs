using UnityEngine;
using UnityEngine.InputSystem;


namespace Platform.Interfacing.Systems
{

    public class Music : MonoBehaviour
    {
        [SerializeField]
        InputActionReference mute;

        private void OnEnable()
        {
            mute.action.performed += ToggleMute;
        }

        private void OnDisable()
        {
            mute.action.performed -= ToggleMute;
        }


        private void ToggleMute(InputAction.CallbackContext context)
        {
            Debug.Log("m pressed");
            transform.GetComponent<AudioSource>().mute = !transform.GetComponent<AudioSource>().mute;
        }
    }

}
