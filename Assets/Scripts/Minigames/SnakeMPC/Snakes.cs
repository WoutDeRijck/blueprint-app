using System.Collections.Generic;
using UnityEngine;


namespace Snake
{

    /// <summary>
    /// Handles all snake objects in the scene (When the snake is parted by a wall, it consists of two snake objects)
    /// </summary>
    public class Snakes : MonoBehaviour
    {
        /// <summary>
        /// Returns a list of positions that are obstructed by the snake
        /// </summary>
        /// <returns></returns>
        public List<Vector2> GetOccupiedPositions()
        {
            List<Vector2> occupiedPositions = new List<Vector2>();

            // for every snake in snakes
            for (int i = 0; i < transform.childCount; i++)
            {
                Vector2 headPosition = Vector2Int.RoundToInt(transform.GetChild(i).GetChild(0).position);

                if (!occupiedPositions.Contains(headPosition))
                {
                    occupiedPositions.Add(headPosition);
                }

                // for every body part in the snake
                for (int j = 0; j < transform.GetChild(i).GetChild(1).childCount; j++)
                {
                    Vector2 bodyPosition = Vector2Int.RoundToInt(transform.GetChild(i).GetChild(1).GetChild(j).position);

                    if (!occupiedPositions.Contains(bodyPosition))
                    {
                        occupiedPositions.Add(bodyPosition);
                    }
                }
            }
            return occupiedPositions;
        }

        /// <summary>
        /// Returns the position of the snake head
        /// </summary>
        /// <returns></returns>
        public Vector2 GetHeadPosition()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform snakeHeadMovement = transform.GetChild(i).GetChild(0);

                // check if head of snake is rendered
                if (snakeHeadMovement.GetChild(1).GetComponent<SpriteRenderer>().enabled)
                {
                    return Vector2Int.RoundToInt(snakeHeadMovement.position);
                }
            }

            return new Vector2(-2, -2);
        }

        /// <summary>
        /// Shrinks the snake back to its original size
        /// </summary>
        public void ShrinkSnake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                SnakeTail snakeTail = transform.GetChild(i).GetChild(1).GetComponent<SnakeTail>();
                snakeTail.RemoveBody();
            }
        }

        /// <summary>
        /// Grows the snake with 1 unit
        /// </summary>
        public void GrowSnake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                SnakeTail snakeTail = transform.GetChild(i).GetChild(1).GetComponent<SnakeTail>();
                snakeTail.AddBody();
            }
        }

        /// <summary>
        /// Sets the snake's speed to 0
        /// </summary>
        public void PauseSnake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                SnakeMovement snakeMovement = transform.GetChild(i).GetChild(0).GetComponent<SnakeMovement>();
                snakeMovement.SetCurrentSpeed(0);
                snakeMovement.BlockInput();
            }
        }

        /// <summary>
        /// Sets the snake's general speed
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(float speed)
        {
            if (speed < 0)
            {
                speed = 1f;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                SnakeMovement snakeMovement = transform.GetChild(i).GetChild(0).GetComponent<SnakeMovement>();
                snakeMovement.SetSpeed(speed);
            }
        }

        /// <summary>
        /// Destroys all snake objects
        /// </summary>
        public void DestroyAllSnakes()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
