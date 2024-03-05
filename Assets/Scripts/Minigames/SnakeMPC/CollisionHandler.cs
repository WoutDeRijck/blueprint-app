using UnityEngine;
using UnityEngine.InputSystem;



namespace Snake
{


    /// <summary>
    /// Handles collisions between snake bodyparts and walls
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        private SnakeSpawner spawner;
        private SnakeGameHandler gameHandler;

        public void OnEnable()
        {
            spawner = GameObject.Find("SnakeSpawner").GetComponent<SnakeSpawner>();
            gameHandler = GameObject.Find("GameHandler").GetComponent<SnakeGameHandler>();
        }

        /// <summary>
        /// Handles collision
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag.Equals("Obstacle"))
            {
                HandleWallCollision(other);
            }

            if (other.tag.Equals("Food") && transform.tag.Equals("SnakeHead"))
            {
                HandleFoodCollision(other);
            }            
        }

        /// <summary>
        /// Handles wall collision: snake wraps around the map
        /// </summary>
        /// <param name="other"></param>
        private void HandleWallCollision(Collider2D other)
        {
            SetVisibility(false);

            if (transform.tag == "SnakeHead")
            {
                // Disable Player Input and collider
                transform.parent.GetComponent<PlayerInput>().enabled = false;
                transform.GetComponent<CircleCollider2D>().enabled = false;

                // Calculate variables of new snake to be spawned
                Transform snakes = transform.parent.parent.parent;
                float speed = transform.parent.GetComponent<SnakeMovement>().GetSpeed();
                float currentSpeed = transform.parent.GetComponent<SnakeMovement>().GetCurrentSpeed();
                int size = transform.parent.parent.GetChild(1).GetComponent<SnakeTail>().GetSize();
                Vector2 position = calculatePositionAfterWallHit(transform.parent.position, other.name);
                Quaternion rotation = transform.parent.rotation;

                // Spawn the new snake
                spawner.SpawnSnake(size, speed, currentSpeed, position, rotation);
            }

            if (entireSnakeIsInvisible())
            {
                Destroy(transform.parent.parent.gameObject);
            }
        }

        /// <summary>
        /// Handles food collistion: snake eats food (correct or wrong)
        /// </summary>
        /// <param name="other"></param>
        private void HandleFoodCollision(Collider2D other)
        {
            bool snakeAteCorrectApple = other.GetComponent<FoodInformation>().correct;

            if (snakeAteCorrectApple)
            {
                other.gameObject.SetActive(false);
            }

            gameHandler.Eat(other.gameObject);

            gameHandler.AnswerQuestion(snakeAteCorrectApple);
        }


        /// <summary>
        /// Disables the SpriteRenderer of the gameObject
        /// </summary>
        /// <param name="visible"></param>
        private void SetVisibility(bool visible)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = visible;
        }

        /// <summary>
        /// Checks if the entire snake is not being Rendered
        /// </summary>
        /// <returns></returns>
        private bool entireSnakeIsInvisible()
        {
            Transform snake = transform.parent.parent;
            for (int i = 0; i < snake.childCount; i++)
            {
                for (int j = 0; j < snake.GetChild(i).childCount; j++)
                {
                    if (snake.GetChild(i).GetChild(j).GetComponent<SpriteRenderer>().enabled)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates the position where a new snake should be spawned after hitting a wall
        /// </summary>
        /// <param name="position"></param>
        /// <param name="wallName"></param>
        /// <returns></returns>
        private Vector2 calculatePositionAfterWallHit(Vector2 position, string wallName)
        {
            Vector2 newPosition = position;
            if (wallName == "RightWall")
            {
                newPosition = new Vector2(-0.5f, position.y);
            }
            if (wallName == "LeftWall")
            {
                newPosition = new Vector2(15.5f, position.y);
            }
            if (wallName == "TopWall")
            {
                newPosition = new Vector2(position.x, -0.5f);
            }
            if (wallName == "BottomWall")
            {
                newPosition = new Vector2(position.x, 7.5f);
            }
            return newPosition;
        }
    }


}
