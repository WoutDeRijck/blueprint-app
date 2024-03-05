using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


namespace Snake
{


    /// <summary>
    /// Handles snake Movement
    /// </summary>
    public class SnakeMovement : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference movement;

        private SnakeGameHandler gameHandler;

        // snake properties
        private float speed;
        private float currentSpeed;
        private float rotationSpeed = 400f;

        private bool blocked = false;

        private Vector2 rotationInput;


        /// <summary>
        /// Changes snake properties
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void Setup(float speed, float currentSpeed, Vector2 position, Quaternion rotation)
        {
            this.speed = speed;
            this.currentSpeed = currentSpeed;
            transform.position = position;
            transform.rotation = rotation;
        }

        public void OnEnable()
        {
            gameHandler = GameObject.Find("GameHandler").GetComponent<SnakeGameHandler>();

            movement.action.performed += ArrowkeyPerformed;
            movement.action.canceled += ArrowkeyReleased;

        }

        public void OnDisable()
        {
            movement.action.performed -= ArrowkeyPerformed;
            movement.action.canceled -= ArrowkeyReleased;
        }

        public void FixedUpdate()
        {
            MoveForward();
            HandleInput();
        }

        private void ArrowkeyPerformed(InputAction.CallbackContext context)
        {
            rotationInput = movement.action.ReadValue<Vector2>();
        }

        private void ArrowkeyReleased(InputAction.CallbackContext context)
        {
            rotationInput = Vector2.zero;
        }

        /// <summary>
        /// Handles Input
        /// </summary>
        private void HandleInput()
        {
            if (!(gameHandler.GetState() == SnakeGameHandler.State.Alive || gameHandler.GetState() == SnakeGameHandler.State.Paused))
            {
                return;
            }

            if (blocked && rotationInput == Vector2.zero)
            {
                blocked = false;
            }

            if (!blocked && rotationInput != Vector2.zero)
            {
                if (currentSpeed == 0)
                {
                    currentSpeed = speed;
                    gameHandler.SetState(SnakeGameHandler.State.Alive);
                }
                Rotate(rotationInput);
            }
        }

        /// <summary>
        /// Rotates snake head with rotation speed
        /// </summary>
        /// <param name="rotationInput"></param>
        private void Rotate(Vector2 rotationInput)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, rotationInput);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        /// <summary>
        /// Moves snake forward with speed
        /// </summary>
        private void MoveForward()
        {
            Vector2 positionIncrease = Vector2.up * currentSpeed * Time.fixedDeltaTime;
            transform.Translate(positionIncrease, Space.Self);
        }

        /// <summary>
        /// Returns the general speed value of the snake
        /// </summary>
        /// <returns></returns>
        public float GetSpeed()
        {
            return speed;
        }

        /// <summary>
        /// Sets the general speed value of the snake
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        /// <summary>
        /// Returns the current speed of the snake
        /// </summary>
        /// <returns></returns>
        public float GetCurrentSpeed()
        {
            return currentSpeed;
        }

        /// <summary>
        /// Sets the current speed of the snake
        /// </summary>
        /// <param name="speed"></param>
        public void SetCurrentSpeed(float speed)
        {
            this.currentSpeed = speed;
        }

        /// <summary>
        /// Blocks arrowkeys input
        /// </summary>
        public void BlockInput()
        {
            blocked = true;
        }
    }


}
