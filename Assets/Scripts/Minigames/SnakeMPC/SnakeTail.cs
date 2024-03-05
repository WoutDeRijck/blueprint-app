using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


namespace Snake
{


    /// <summary>
    /// Holds information about the snake tail
    /// </summary>
    public class SnakeTail : MonoBehaviour
    {
        [SerializeField]
        private Transform snakeZeroBody; // This body is rendered underneath the head 

        private float diameter = 0.1f; // distance between two bodyparts

        private List<Transform> snakeBodyTransforms = new List<Transform>(); // transforms of the bodies (first body at index 0)
        private List<Vector2> snakePositions = new List<Vector2>(); // positions of the full snake (head at index 0)

        private int startingSize = 2;
        private int size = 0;

        /// <summary>
        /// Creates a snake tail of a certain size
        /// </summary>
        /// <param name="size"></param>
        public void Setup(int size)
        {
            snakePositions.Add(snakeZeroBody.position);

            for (int i = 0; i < size; i++)
            {
                AddBody();
            }
        }

        public void FixedUpdate()
        {
            // Calculate distance head traveled
            float distance = ((Vector2)snakeZeroBody.position - snakePositions[0]).magnitude;

            if (distance > diameter)
            {
                Vector2 direction = ((Vector2)snakeZeroBody.position - snakePositions[0]).normalized;

                snakePositions.Insert(0, snakePositions[0] + direction * diameter);
                snakePositions.RemoveAt(snakePositions.Count - 1);

                distance -= diameter;
            }

            UpdatePositions(distance);
        }

        /// <summary>
        /// Updates all tail positions
        /// </summary>
        /// <param name="distance"></param>
        private void UpdatePositions(float distance)
        {
            for (int i = snakeBodyTransforms.Count - 1; i >= 0; i--)
            {
                snakeBodyTransforms[i].position = Vector2.Lerp(snakePositions[i + 1], snakePositions[i], distance / diameter);
            }
        }

        /// <summary>
        /// Adds a new body segment to the snake tail
        /// </summary>
        public void AddBody()
        {
            for (int i = 0; i < 10; i++)
            {
                Transform body = Instantiate(snakeZeroBody, snakePositions[snakePositions.Count - 1], Quaternion.identity, transform);
                body.GetComponent<SpriteRenderer>().enabled = true;
                snakeBodyTransforms.Add(body);
                snakePositions.Add(body.position);
            }
            size++;
        }

        /// <summary>
        /// Reduces the snake's tail to the original size
        /// </summary>
        public void RemoveBody()
        {
            if (size == startingSize) return;

            for (int i = 0; i < size - startingSize; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Transform body = snakeBodyTransforms[snakeBodyTransforms.Count - 1];
                    snakeBodyTransforms.RemoveAt(snakeBodyTransforms.Count -1);
                    snakePositions.RemoveAt(snakePositions.Count - 1);
                    Destroy(body.gameObject);
                }
            }
            size = startingSize;
        }

        /// <summary>
        /// Returns the snake tail size
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return size;
        }
    }


}
